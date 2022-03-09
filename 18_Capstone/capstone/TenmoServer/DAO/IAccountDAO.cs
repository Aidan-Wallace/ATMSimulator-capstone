using System.Collections.Generic;
using TenmoServer.Models;

namespace TenmoServer.DAO
{
    public interface IAccountDao
    {
        decimal GetBalance(int userId);
       
    }
}
