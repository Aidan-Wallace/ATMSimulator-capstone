using System.Collections.Generic;
using TenmoServer.Models;

namespace TenmoServer.DAO
{
    public interface IAccountDao
    {
        decimal GetBalance(int userId);
        bool SendMoney(int fromUserId, int toUserId, decimal transferAmount);
        Transfer GetTransferById(int transferId, int userId);
        List<CompletedTransfer> GetTransfers(int userId);
    }
}
