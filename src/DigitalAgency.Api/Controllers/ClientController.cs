using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using DigitalAgency.Bll.Services.Interfaces;
using DigitalAgency.Dal.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DigitalAgency.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        private readonly IClientService _clientService;
        private readonly IExecutorService _executorService;
        private readonly IValidator<DigitalAgency.Dal.Entities.Client> _clientValidator;
        private readonly ILogger<ClientController> _logger;
        public ClientController(IClientService clientService, 
            IValidator<DigitalAgency.Dal.Entities.Client> clientValidation, 
            ILogger<ClientController> logger, 
            IExecutorService executorService)
        {
            _clientService = clientService;
            _clientValidator = clientValidation;
            _logger = logger;
            _executorService = executorService;
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
            var result = await _executorService.GetExecutorsAsync();
            _logger.LogDebug("Time request {Time}", DateTime.UtcNow);
            return Ok(result);
        }
        
        [HttpPost]
        public async Task<ActionResult<DigitalAgency.Dal.Entities.Client>> PostClient(DigitalAgency.Dal.Entities.Client client)
        {
            _logger.LogInformation("Star logging - method PostClient controller ClientContoller");
            var result = _clientValidator.Validate(client);
            if (!result.IsValid)
            {
                return BadRequest(result.Errors.Select(x => new { Error = x.ErrorMessage, Code = x.ErrorCode }).ToList());
            }
            await _clientService.CreateClientAsync(client);
            _logger.LogDebug("Time request {Time}", DateTime.UtcNow);
            return Ok(client);
        }
        [HttpPost("executor")]
        public async Task<ActionResult<DigitalAgency.Dal.Entities.Client>> PostExecutor(Executor executor)
        {
            _logger.LogInformation("Star logging - method PostClient controller ClientContoller");
            await _executorService.CreateExecutorAsync(executor);
            _logger.LogDebug("Time request {Time}", DateTime.UtcNow);
            return Ok(executor);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClient(int id)
        {
            _logger.LogInformation("Star logging - method DeleteClient controller ClientContoller");
            await _clientService.DeleteClientAsync(id);
            _logger.LogDebug("Time request {Time}", DateTime.UtcNow);
            return Ok(id);
        }
        
        [HttpDelete("executor/{id}")]
        public async Task<IActionResult> DeleteExecutor(int id)
        {
            _logger.LogInformation("Star logging - method DeleteClient controller ClientContoller");
            await _executorService.DeleteExecutorAsync(id);
            _logger.LogDebug("Time request {Time}", DateTime.UtcNow);
            return Ok(id);
        }

        [HttpPut]
        public async Task<ActionResult<DigitalAgency.Dal.Entities.Client>> PutClient(DigitalAgency.Dal.Entities.Client client)
        {
            _logger.LogInformation("Star logging - method PutClient controller ClientContoller " + JsonSerializer.Serialize(client));
            var result = _clientValidator.Validate(client);
            if (!result.IsValid)
            {
                return BadRequest(result.Errors.Select(x => new { Error = x.ErrorMessage, Code = x.ErrorCode }).ToList());
            }
            await _clientService.UpdateClientAsync(client);
            _logger.LogDebug("Time request {Time}", DateTime.UtcNow);
            return Ok(client);
        }
        [HttpPut("executor")]
        public async Task<ActionResult<DigitalAgency.Dal.Entities.Client>> PutExecutor(Executor executor)
        {
            _logger.LogInformation("Star logging - method PutClient controller ClientContoller " + JsonSerializer.Serialize(executor));
            await _executorService.UpdateExecutorAsync(executor);
            _logger.LogDebug("Time request {Time}", DateTime.UtcNow);
            return Ok(executor);
        }
    }
}