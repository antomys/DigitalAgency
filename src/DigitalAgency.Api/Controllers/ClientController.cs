using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using DigitalAgency.Api.Validate;
using DigitalAgency.Bll.Models;
using DigitalAgency.Bll.Services.Interfaces;
using DigitalAgency.Dal.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DigitalAgency.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        private readonly IClientService _clientService;
        private readonly ILogger<ClientController> _logger;
        private readonly ClientModelValidator _clientValidator;
        private readonly ExecutorModelValidator _executorValidator;

        public ClientController(IClientService clientService,
            ILogger<ClientController> logger,
            ClientModelValidator clientValidator, 
            ExecutorModelValidator executorValidator)
        {
            _clientService = clientService;
            _logger = logger;
            _clientValidator = clientValidator;
            _executorValidator = executorValidator;
        }

        [HttpGet]
        public async Task<IActionResult> GetClient()
        {
            _logger.LogInformation("Star logging - method GetClientAsync controller ClientContoller");
            var result = await _clientService.GetClientsAsync();
            _logger.LogDebug("Time request {Time}", DateTime.UtcNow);
            return Ok(result);
        }
        
        [HttpGet("executor")]
        public async Task<IActionResult> GetExecutor()
        {
            _logger.LogInformation("Star logging - method GetClientAsync controller ClientContoller");
            var result = await _clientService.GetExecutorsAsync();
            _logger.LogDebug("Time request {Time}", DateTime.UtcNow);
            return Ok(result);
        }
        
        [HttpPost]
        public async Task<ActionResult<ClientModel>> PostClient(ClientModel clientModel)
        {
            _logger.LogInformation("Star logging - method PostClient controller ClientContoller");
            var result = await _clientValidator.ValidateAsync(clientModel);
            if (!result.IsValid)
            {
                return BadRequest(result.Errors.Select(x => new { Error = x.ErrorMessage, Code = x.ErrorCode }).ToList());
            }
            await _clientService.CreateClientAsync(clientModel);
            _logger.LogDebug("Time request {Time}", DateTime.UtcNow);
            return Ok(clientModel);
        }
        [HttpPost("executor")]
        public async Task<ActionResult<ExecutorModel>> PostExecutor(ExecutorModel executor)
        {
            _logger.LogInformation("Star logging - method PostClient controller ClientContoller");
            var result = await _executorValidator.ValidateAsync(executor);
            if (!result.IsValid)
            {
                return BadRequest(result.Errors.Select(x => new { Error = x.ErrorMessage, Code = x.ErrorCode }).ToList());
            }
            await _clientService.CreateExecutorAsync(executor);
            _logger.LogDebug("Time request {Time}", DateTime.UtcNow);
            return Ok(executor);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteClient(int id)
        {
            _logger.LogInformation("Star logging - method DeleteClient controller ClientContoller");
            await _clientService.DeleteClientAsync(id);
            _logger.LogDebug("Time request {Time}", DateTime.UtcNow);
            return Ok(id);
        }
        
        [HttpDelete("executor/{id:int}")]
        public async Task<IActionResult> DeleteExecutor(int id)
        {
            _logger.LogInformation("Star logging - method DeleteClient controller ClientContoller");
            await _clientService.DeleteExecutorAsync(id);
            _logger.LogDebug("Time request {Time}", DateTime.UtcNow);
            return Ok(id);
        }

        [HttpPut]
        public async Task<ActionResult<ClientModel>> PutClient(ClientModel client)
        {
            _logger.LogInformation("Star logging - method PutClient controller ClientContoller " + JsonSerializer.Serialize(client));
            var result = await _clientValidator.ValidateAsync(client);
            if (!result.IsValid)
            {
                return BadRequest(result.Errors.Select(x => new { Error = x.ErrorMessage, Code = x.ErrorCode }).ToList());
            }
            await _clientService.UpdateClientAsync(client);
            _logger.LogDebug("Time request {Time}", DateTime.UtcNow);
            return Ok(client);
        }
        [HttpPut("executor")]
        public async Task<ActionResult<ExecutorModel>> PutExecutor(Executor executor)
        {
            _logger.LogInformation("Star logging - method PutClient controller ClientContoller " + JsonSerializer.Serialize(executor));
            await _clientService.UpdateExecutorAsync(executor);
            _logger.LogDebug("Time request {Time}", DateTime.UtcNow);
            return Ok(executor);
        }
    }
}