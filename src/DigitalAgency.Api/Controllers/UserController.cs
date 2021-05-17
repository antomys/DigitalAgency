using System.Threading.Tasks;
using DigitalAgency.Dal.Storages.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DigitalAgency.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserStorage _userStorage;
        private readonly ILogger<UserController> _logger;
        public UserController(IUserStorage userStorage, ILogger<UserController> logger)
        {
            _userStorage = userStorage;
            _logger = logger;
        }

        [HttpPost("/Registration")]
        public async Task<IActionResult> UserRegistration(string userPhoneNumber, string userPassword)
        {
            _logger.LogInformation("Star logging - method UserRegistration controller UserContoller");
            if (await _userStorage.RegisterUser(userPhoneNumber, userPassword))
                return Ok("The user was successfully registered");
            else return BadRequest("Invalid values entered or user with the same phone number already exists");
        }

        [HttpPost("/GetToken")]
        public async Task<IActionResult> GetUserToken(string userPhoneNumber, string userPassword)
        {
            _logger.LogInformation("Star logging - method GetUserToken controller UserContoller");
            var result = await _userStorage.GetToken(userPhoneNumber, userPassword);
           
            if (string.IsNullOrEmpty(result))
                return BadRequest("Invalid values entered");
            else return Ok(result);
            
        }
    }
}