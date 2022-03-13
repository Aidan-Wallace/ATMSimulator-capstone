using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TenmoClient.Models
{
    public class Transfer
    {
        public int TransferId { get; set; }
        public string TransferTypeDescription { get; set; }
        public string TransferStatusDescription { get; set; }
        public string FromUsername { get; set; }
        public string ToUsername { get; set; }
        public decimal TransferAmount { get; set; }
    }

    public class TransferMoney
    {
        public int FromUserId { get; set; }
        public int ToUserId { get; set; }
        public decimal TransferAmount { get; set; }
    }

    public class AllTransfers
    {
        public int TransferId { get; set; }
        public string FromUsername { get; set; }
        public string ToUsername { get; set; }
        public decimal Amount { get; set; }
        public string TransferStatus { get; set; }
    }

    public class PendingTransfer
    {
        public int TransferId { get; set; }
        public string ToUsername { get; set; }
        public decimal Amount { get; set; }
    }

    public class UpdatePendingApproval
    {
        public int TransferId { get; set; }
        public int RequestedTransferType { get; set; }
    }
}
