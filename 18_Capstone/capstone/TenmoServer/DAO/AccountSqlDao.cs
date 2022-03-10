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
            decimal returnBalance = GetAccount(userId).Balance;

            return returnBalance;
        }

        public Account GetAccount(int userId)
        {
            Account account = new Account();
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
                        account = GetAccountFromReader(reader);
                    }
                }
            }
            catch (SqlException)
            {
                throw;
            }
            return account;
        }

        public bool SendMoney(int fromUserId, int toUserId, decimal transferAmount)
        {
            Transfer transfer = new Transfer();

            Account fromAcct = GetAccount(fromUserId);
            Account toAcct = GetAccount(toUserId);
            int fromAcctId = fromAcct.AcctId;
            int toAcctId = toAcct.AcctId;
            if (transferAmount <= fromAcct.Balance)
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();

                        SqlCommand cmd = new SqlCommand("INSERT INTO transfer (transfer_type_id, transfer_status_id, account_from, account_to, amount) VALUES(2, 2, @fromAcctId, @toAcctId, @amount); UPDATE account SET balance -= @amount WHERE account_id = @fromAcctId; UPDATE account SET balance += @amount WHERE account_id = @toAcctId", conn);
                        cmd.Parameters.AddWithValue("@fromAcctId", fromAcctId);
                        cmd.Parameters.AddWithValue("@toAcctId", toAcctId);
                        cmd.Parameters.AddWithValue("@amount", transferAmount);

                        int rows = cmd.ExecuteNonQuery();

                        if (rows > 0)
                        {
                            return true;
                        }
                    }
                }
                catch (SqlException)
                {
                    throw;
                }
            }
            return false;
        }

        public List<CompletedTransfer> GetTransfers(int userId)
        {
            List<CompletedTransfer> completedTransfers = new List<CompletedTransfer>();

            List<CompletedTransfer> transfersFrom = GetTransfersFrom(userId);

            return completedTransfers;
        }

        public List<CompletedTransfer> GetTransfersFrom(int userId)
        {
            List<CompletedTransfer> transfers = new List<CompletedTransfer>();

            Account acct = GetAccount(userId);
            int acctId = acct.AcctId;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand($"SELECT transfer_id, account_from, amount FROM transfer WHERE account_to = {acctId} AND transfer_status_id = 2", conn);

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        CompletedTransfer transfer = GetTransfersReceivedFromReader(reader);
                        transfers.Add(transfer);
                    }
                }
            }
            catch (SqlException)
            {
                throw;
            }
            return transfers;
        }

        private CompletedTransfer GetTransfersReceivedFromReader(SqlDataReader reader)
        {
            CompletedTransfer transfers = new CompletedTransfer()
            {
                TransferId = Convert.ToInt32(reader["transfer_id"]),
                Type = "From: ",
                UserId = Convert.ToInt32(reader["account_from"]),
                Amount = Convert.ToDecimal(reader["amount"])
            };
            return transfers;
        }

        private CompletedTransfer GetTransfersToFromReader(SqlDataReader reader)
        {
            CompletedTransfer transfers = new CompletedTransfer()
            {
                TransferId = Convert.ToInt32(reader["transfer_id"]),
                Type = "To: ",
                UserId = Convert.ToInt32(reader["account_to"]),
                Amount = Convert.ToDecimal(reader["amount"])
            };
            return transfers;
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
