using System.Threading.Tasks;
using DigitalAgency.Bll.Services.Interfaces;
using DigitalAgency.Dal.Storages.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DigitalAgency.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        protected readonly IUserStorage UserStorage;
        private readonly ILogger<UserController> _logger;
        public UserController(IUserStorage userStorage, ILogger<UserController> logger)
        {
            UserStorage = userStorage;
            _logger = logger;
        }

        [HttpPost("/Registration")]
        public async Task<IActionResult> UserRegistration(string UserPhoneNumber, string UserPassword)
        {
            _logger.LogInformation("Star logging - method UserRegistration controller UserContoller");
            if (await UserStorage.RegisterUser(UserPhoneNumber, UserPassword))
               
            return Ok("The user was successfully registered");
            else return BadRequest("Invalid values entered or user with the same phone number already exists");
        }

        [HttpPost("/GetToken")]
        public async Task<IActionResult> GetUserToken(string UserPhoneNumber, string UserPassword)
        {
            _logger.LogInformation("Star logging - method GetUserToken controller UserContoller");
            var result = await UserStorage.GetToken(UserPhoneNumber, UserPassword);
           
            if (string.IsNullOrEmpty(result))
                return BadRequest("Invalid values entered");
            else return Ok(result);
            
        }
    }
}