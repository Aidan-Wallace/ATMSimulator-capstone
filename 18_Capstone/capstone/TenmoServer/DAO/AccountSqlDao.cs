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
        private List<AllTransfers> completedTransfers = new List<AllTransfers>();

        /// <summary>
        /// Gets current user's account balance
        /// </summary>
        /// <param name="userId">Current user's Id.</param>
        public decimal GetBalance(int userId)
        {
            decimal returnBalance = GetAccount(userId).Balance;

            return returnBalance;
        }

        /// <summary>
        /// Gets account info by userId
        /// </summary>
        /// <param name="userId"></param>
        private Account GetAccount(int userId)
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

        /// <summary>
        /// Sends money to another user
        /// </summary>
        /// <param name="fromUserId"></param>
        /// <param name="toUserId"></param>
        /// <param name="transferAmount"></param>
        /// <returns>True if transfer successful</returns>
        public bool SendMoney(int fromUserId, int toUserId, decimal transferAmount)
        {
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

                        SqlCommand cmd = new SqlCommand($"INSERT INTO transfer (transfer_type_id, transfer_status_id, account_from, account_to, amount) VALUES(2, 2, @fromAcctId, @toAcctId, @amount); UPDATE account SET balance -= @amount WHERE account_id = @fromAcctId; UPDATE account SET balance += @amount WHERE account_id = @toAcctId", conn);

                        cmd.Parameters.AddWithValue("@fromAcctId", fromAcctId);
                        cmd.Parameters.AddWithValue("@toAcctId", toAcctId);
                        cmd.Parameters.AddWithValue("@amount", transferAmount);

                        int rows = cmd.ExecuteNonQuery();

                        if (rows > 0) return true;
                    }
                }
                catch (SqlException)
                {
                    throw;
                }
            }
            return false;
        }

        
        /// <summary>
        /// Requests money from another user
        /// </summary>
        /// <param name="fromUserId"></param>
        /// <param name="toUserId"></param>
        /// <param name="transferAmount"></param>
        /// <returns>Returns true if successful</returns>
        public bool RequestMoney(int fromUserId, int toUserId, decimal transferAmount)
        {
            Account fromAcct = GetAccount(fromUserId);
            Account toAcct = GetAccount(toUserId);
            int fromAcctId = fromAcct.AcctId;
            int toAcctId = toAcct.AcctId;

                try
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();

                        SqlCommand cmd = new SqlCommand($"INSERT INTO transfer (transfer_type_id, transfer_status_id, account_from, account_to, amount) VALUES(1, 1, @fromAcctId, @toAcctId, @amount)", conn);

                        cmd.Parameters.AddWithValue("@fromAcctId", fromAcctId);
                        cmd.Parameters.AddWithValue("@toAcctId", toAcctId);
                        cmd.Parameters.AddWithValue("@amount", transferAmount);

                        int rows = cmd.ExecuteNonQuery();

                        if (rows > 0) return true;
                    }
                }
                catch (SqlException)
                {
                    throw;
                }
            return false;
        }

        /// <summary>
        /// Get Transfer details by transferId
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Transfer details</returns>
        public Transfer GetTransferById(int transferId, int userId)
        {
            Transfer transfer = new Transfer();
            Account account = GetAccount(userId);
            int acctId = account.AcctId;

            string sql = $"SELECT transfer_id, uf.username from_username, ut.username to_username, transfer_type_desc, transfer_status_desc, amount FROM transfer t JOIN transfer_type tt on t.transfer_type_id = tt.transfer_type_id JOIN transfer_status ts on t.transfer_status_id = ts.transfer_status_id JOIN account af on account_from = af.account_id JOIN account a on account_to = a.account_id JOIN tenmo_user uf ON af.user_id = uf.user_id JOIN tenmo_user ut ON a.user_id = ut.user_id WHERE t.transfer_id = @transfer_id AND (account_from = {acctId} OR account_to = {acctId});";

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@transfer_id", transferId);

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

        /// <summary>
        /// Gets list of all the logged in users completed transfers
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>Transfers from or to the current user</returns>
        public List<AllTransfers> GetTransfers(int userId)
        {
            Account userAcct = GetAccount(userId);
            int acctId = userAcct.AcctId;

            List<AllTransfers> transfers = new List<AllTransfers>();

            string sql = $"SELECT transfer_id, ut.username to_username, uf.username from_username, amount, transfer_status_desc FROM transfer t JOIN account af ON t.account_from = af.account_id JOIN account ato ON t.account_to = ato.account_id JOIN tenmo_user uf ON af.user_id = uf.user_id JOIN tenmo_user ut ON ato.user_id = ut.user_id JOIN transfer_status ts on t.transfer_status_id = ts.transfer_status_id WHERE (account_from = {acctId} OR account_to = {acctId})";

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(sql, conn);

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        AllTransfers transfer = GetCompletedTransferFromReader(reader);
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

        public List<PendingTransfer> GetPendingTransfers(int userId)
        {
            Account userAcct = GetAccount(userId);
            int acctId = userAcct.AcctId;

            List<PendingTransfer> transfers = new List<PendingTransfer>();

            string sql = $"SELECT transfer_id, ut.username to_username, amount FROM transfer t JOIN account af ON t.account_from = af.account_id JOIN account ato ON t.account_to = ato.account_id JOIN tenmo_user uf ON af.user_id = uf.user_id JOIN tenmo_user ut ON ato.user_id = ut.user_id WHERE account_from = {acctId} AND transfer_status_id = 1;";

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(sql, conn);

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        PendingTransfer transfer = GetPendingTransferFromReader(reader);
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

        /// <summary>
        /// Gets user's pending transfer requests
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<PendingTransfer> GetTransferRequests(int userId)
        {
            Account userAcct = GetAccount(userId);
            int acctId = userAcct.AcctId;

            List<PendingTransfer> transfers = new List<PendingTransfer>();

            string sql = $"SELECT transfer_id, ut.username to_username, amount FROM transfer t JOIN account af ON t.account_from = af.account_id JOIN account ato ON t.account_to = ato.account_id JOIN tenmo_user uf ON af.user_id = uf.user_id JOIN tenmo_user ut ON ato.user_id = ut.user_id WHERE account_from = {acctId} AND transfer_status_id = 1";

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(sql, conn);

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        PendingTransfer transfer = GetPendingTransferFromReader(reader);
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
        /// <summary>
        /// Gets a CompletedTransfer from SqlDataReader for GetTransfers
        /// </summary>
        /// <param name="reader"></param>
        private AllTransfers GetCompletedTransferFromReader(SqlDataReader reader)
        {
            AllTransfers transfer = new AllTransfers()
            {
                TransferId = Convert.ToInt32(reader["transfer_id"]),
                ToUsername = Convert.ToString(reader["to_username"]),
                FromUsername = Convert.ToString(reader["from_username"]),
                Amount = Convert.ToDecimal(reader["amount"]),
                TransferStatus = Convert.ToString(reader["transfer_status_desc"])
            };
            return transfer;
        }

        private PendingTransfer GetPendingTransferFromReader(SqlDataReader reader)
        {
            PendingTransfer transfer = new PendingTransfer()
            {
                TransferId = Convert.ToInt32(reader["transfer_id"]),
                ToUsername = Convert.ToString(reader["to_username"]),
                Amount = Convert.ToDecimal(reader["amount"]),
            };
            return transfer;
        }

        /// <summary>
        /// Gets Details of Transfer from SqlDataReader for GetTransferById
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private Transfer GetTransferDetailsFromReader(SqlDataReader reader)
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

        /// <summary>
        /// Gets Account details from SqlDataReader for GetAccount
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
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
