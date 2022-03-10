using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using TenmoServer.Models;
using TenmoServer.DAO;

namespace TenmoServer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserDao userDao;
        public UserController(IUserDao _userDao)
        {
            userDao = _userDao;
        }

        [HttpGet]
        public ActionResult<List<User>> GetUsers()
        {
            List<User> users = userDao.GetUsersForMenu();

            return users;
        }
    }
}
