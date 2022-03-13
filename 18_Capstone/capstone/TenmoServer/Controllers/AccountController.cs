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
        public ActionResult<List<AllTransfers>> GetTransfers(int userId)
        {
            return accountDao.GetTransfers(userId);
        }

        [HttpGet("{userId}/transfers/pending")]
        public ActionResult<List<PendingTransfer>> GetPendingTransfers(int userId)
        {
            return accountDao.GetPendingTransfers(userId);
        }

        [HttpGet("{userId}/transfer/{transferId}")]
        public ActionResult<Transfer> GetTransferDetails(int transferId, int userId)
        {
            return accountDao.GetTransferById(transferId, userId);
        }


        [HttpPost("transfer")]
        public ActionResult<bool> SendMoney(TransferMoney transfer)
        {
            return Ok(accountDao.SendMoney(transfer.FromUserId, transfer.ToUserId, transfer.TransferAmount));
        }

        [HttpPost("transfer/request")]
        public ActionResult<bool> RequesetMoney(TransferMoney transfer)
        {
            return Ok(accountDao.RequestMoney(transfer.FromUserId, transfer.ToUserId, transfer.TransferAmount));
        }

        [HttpPut("transfer/request/apporove")]
        public ActionResult<bool> ApproveTransferRequest(UpdatePendingApproval updatedRequest)
        {
            return Ok(accountDao.ApproveTransferRequest(updatedRequest.TransferId));
        }

        [HttpPut("transfer/request/reject")]
        public ActionResult<bool> RejectTransferRequest(UpdatePendingApproval updatedRequest)
        {
            return Ok(accountDao.RejectTransferRequest(updatedRequest.TransferId));
        }
    }
}
