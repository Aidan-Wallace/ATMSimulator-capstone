using Microsoft.AspNetCore.Authorization;
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
    [Authorize]
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

        [HttpGet("{userId}/transfer/{transferId}")]
        public ActionResult<Transfer> GetTransferDetails(int transferId, int userId)
        {
            return accountDao.GetTransferById(transferId, userId);
        }

        [HttpPost("{transfer}/{transferTypeId}")]
        public ActionResult<bool> HandleMoneyTransfers(TransferMoney transfer, int transferTypeId)
        {
            return Ok(accountDao.HandleMoneyTransfers(transferTypeId, transfer.FromUserId, transfer.ToUserId, transfer.TransferAmount));
        }
    }
}
