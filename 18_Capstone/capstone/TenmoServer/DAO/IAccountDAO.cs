using System.Collections.Generic;
using TenmoServer.Models;

namespace TenmoServer.DAO
{
    public interface IAccountDao
    {
        decimal GetBalance(int userId);
        bool SendMoney(int fromUserId, int toUserId, decimal transferAmount);
        bool RequestMoney(int fromUserId, int toUserId, decimal transferAmount);
        public bool ApproveTransferRequest(int transferId);
        public bool RejectTransferRequest(int transferId);
        Transfer GetTransferById(int transferId, int userId);
        List<AllTransfers> GetTransfers(int userId);
        List<PendingTransfer> GetPendingTransfers(int userId);
    }
}
