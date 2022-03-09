using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using TenmoServer.Models;
using TenmoServer.Security;
using TenmoServer.Security.Models;

namespace TenmoServer.DAO
{
    public class AccountSqlDao : IAccountDao
    {
        private readonly string connectionString;

        public AccountSqlDao(string dbConnectionString)
        {
            connectionString = dbConnectionString;
        }

        public decimal GetBalance(int userId)
        {
            decimal returnBalance = 0m;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("SELECT * FROM account WHERE user_id = @userId", conn);
                    cmd.Parameters.AddWithValue("@userId", userId);
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        returnBalance = GetAccountFromReader(reader).Balance;
                    }
                }
            }
            catch (SqlException)
            {
                throw;
            }

            return returnBalance;
        }

        private Account GetAccountFromReader(SqlDataReader reader)
        {
            Account account = new Account()
            {
                AcctId = Convert.ToInt32(reader["account_id"]),
                UserId = Convert.ToInt32(reader["user_id"]),
                Balance = Convert.ToDecimal(reader["balance"])
            };

            return account;
        }
    }
}
