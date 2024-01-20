using System;
using System.Threading.Tasks;
using DigitalAgency.Bll.TelegramBot.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;

namespace DigitalAgency.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TelegramController : ControllerBase
{
    readonly IBotService _botService;
    readonly ILogger<TelegramController> _logger;

    public TelegramController(ILogger<TelegramController> logger, IBotService botService)
    {
        _logger = logger;
        _botService = botService;
    }

    [HttpPost]
    public async Task<IActionResult> TelegramProcess(Update update)
    {
        try
        {
            await _botService.ProcessMessageAsync(update);
            return Ok();
        }
        catch (Exception exception)
        {
            _logger.LogWarning(exception.Message);
            return Ok();
        }
    }
}