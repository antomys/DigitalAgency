using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DigitalAgency.Api.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        //private readonly IUserService _userStorage;
        private readonly ILogger<UserController> _logger;
        public UserController(ILogger<UserController> logger)
        {
            //_userStorage = userStorage;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> UserRegistration(string userPhoneNumber, string userPassword)
        {
            /*_logger.LogInformation("Star logging - method UserRegistration controller UserContoller");
            if (await _userStorage.RegisterUser(userPhoneNumber, userPassword))
                return Ok("The user was successfully registered");
            else return BadRequest("Invalid values entered or user with the same phone number already exists");*/
            throw new NotImplementedException();
        }

        [HttpPost]
        public async Task<IActionResult> GetUserToken(string userPhoneNumber, string userPassword)
        {
            /*_logger.LogInformation("Star logging - method GetUserToken controller UserContoller");
            var result = await _userStorage.GetToken(userPhoneNumber, userPassword);
           
            if (string.IsNullOrEmpty(result))
                return BadRequest("Invalid values entered");
            else return Ok(result);*/
            throw new NotImplementedException();
        }
    }
}