using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DigitalAgency.Api.Controllers;

[Route("[controller]/[action]")]
[ApiController]
public class CardController : ControllerBase
{
    readonly ILogger<CardController> _logger;

    public CardController(ILogger<CardController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public Task<IActionResult> GetCard()
    {
        throw new NotImplementedException();
    }
}