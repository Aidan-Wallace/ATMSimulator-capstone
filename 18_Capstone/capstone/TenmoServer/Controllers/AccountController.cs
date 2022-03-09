using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.DAO;

namespace TenmoServer.Controllers
{
    [Route("[controller]")]
    
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountDao accountDao;
        public AccountController(IAccountDao _accountDao)
        {
            accountDao = _accountDao;
        }

        [HttpGet("{userId}")]
        public ActionResult<decimal> GetBalance(int userId)
        //public IActionResult<decimal> GetBalance(int userId)
        {
            return Ok(accountDao.GetBalance(userId));
        }

        [HttpPost("{transfer}")]
        public ActionResult<bool> SendMoney(int fromUserId, int toUserId, decimal transferAmount)
        {
            return Ok(accountDao.SendMoney(fromUserId, toUserId, transferAmount));
        }
    }
}
