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
        private List<CompletedTransfer> completedTransfers = new List<CompletedTransfer>();

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

        public Transfer GetTransferById(int id)
        {
            Transfer transfer = new Transfer();

            string sql = "SELECT transfer_id, uf.username from_username, ut.username to_username, transfer_type_desc, transfer_status_desc, amount FROM transfer t JOIN transfer_type tt on t.transfer_type_id = tt.transfer_type_id JOIN transfer_status ts on t.transfer_status_id = ts.transfer_status_id JOIN account af on account_from = af.account_id JOIN account a on account_to = a.account_id JOIN tenmo_user uf ON af.user_id = uf.user_id JOIN tenmo_user ut ON a.user_id = ut.user_id WHERE t.transfer_id = @transfer_id;";

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@transfer_id", id);

                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        transfer = (GetTransferDetailsFromReader(reader));
                    }
                }
            }
            catch (SqlException)
            {
                throw;
            }

            return transfer;
        }

        public List<CompletedTransfer> GetTransfers(int userId)
        {

            List<CompletedTransfer> transfersFrom = GetTransfersFrom(userId);
            foreach (CompletedTransfer transfer in transfersFrom)
            {
                completedTransfers.Add(transfer);
            }

            List<CompletedTransfer> transfersTo = GetTransfersTo(userId);
            foreach (CompletedTransfer transfer in transfersTo)
            {
                completedTransfers.Add(transfer);
            }
            OrganizeList();

            return completedTransfers;
        }

        public Transfer GetTransferDetailsFromReader(SqlDataReader reader)
        {
            Transfer transfer = new Transfer()
            {
                TransferId = Convert.ToInt32(reader["transfer_id"]),
                FromUsername = Convert.ToString(reader["from_username"]),
                ToUsername = Convert.ToString(reader["to_username"]),
                TransferTypeDescription = Convert.ToString(reader["transfer_type_desc"]),
                TransferStatusDescription = Convert.ToString(reader["transfer_status_desc"]),
                TransferAmount = Convert.ToDecimal(reader["amount"])
            };
            return transfer;
        }

        public List<CompletedTransfer> GetTransfersFrom(int userId)
        {
            List<CompletedTransfer> transfers = new List<CompletedTransfer>();

            Account acct = GetAccount(userId);
            int acctId = acct.AcctId;
            string sql = $"SELECT transfer_id, username, amount FROM transfer t JOIN account a ON t.account_from = a.account_id JOIN tenmo_user u ON u.user_id = a.user_id WHERE account_to = {acctId} AND transfer_status_id = 2";

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(sql, conn);

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

        public List<CompletedTransfer> GetTransfersTo(int userId)
        {
            List<CompletedTransfer> transfers = new List<CompletedTransfer>();

            Account acct = GetAccount(userId);
            int acctId = acct.AcctId;
            string sql = $" SELECT transfer_id, username, amount FROM transfer t JOIN account a ON t.account_to = a.account_id JOIN tenmo_user u ON u.user_id = a.user_id WHERE account_from = {acctId} AND transfer_status_id = 2";

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(sql, conn);

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        CompletedTransfer transfer = GetTransfersToFromReader(reader);
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
                Username = Convert.ToString(reader["username"]),
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
                Username = Convert.ToString(reader["username"]),
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

        public void OrganizeList()
        {
            List<int> order = new List<int>();

            List<CompletedTransfer> result = new List<CompletedTransfer>();

            foreach (CompletedTransfer item in completedTransfers)
            {
                order.Add(item.TransferId);
            }
            order.Sort();
            foreach (int item in order)
            {
                foreach (CompletedTransfer transfer in completedTransfers)
                {
                    if (item == transfer.TransferId)
                    {
                        result.Add(transfer);
                        break;
                    }

                }
            }
            completedTransfers = result;
        }
    }
}
