using System.Collections.Generic;
using TenmoServer.Models;

namespace TenmoServer.DAO
{
    public interface IAccountDao
    {
        decimal GetBalance(int userId);
        bool SendMoney(int fromAcctId, int toAcctId, decimal transferAmount);
       
    }
}
