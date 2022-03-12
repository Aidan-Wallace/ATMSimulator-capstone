using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.DAO;
using TenmoServer.Models;

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
        {
            return Ok(accountDao.GetBalance(userId));
        }

        [HttpGet("{userId}/transfers")]
        public ActionResult<List<CompletedTransfer>> GetTransfers(int userId)
        {
            return accountDao.GetTransfers(userId);
        }

        [HttpGet("transfer/{transferId}")]
        public ActionResult<Transfer> GetTransferDetails(int transferId)
        {
            return accountDao.GetTransferById(transferId);
        }

        [HttpPost("{transfer}")]
        public ActionResult<bool> SendMoney(TransferMoney transfer)
        {
            return Ok(accountDao.SendMoney(transfer.FromUserId, transfer.ToUserId, transfer.TransferAmount));
        }
    }
}
