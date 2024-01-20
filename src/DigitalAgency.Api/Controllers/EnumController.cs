using System.Threading.Tasks;
using DigitalAgency.Bll.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DigitalAgency.Api.Controllers;

[Route("[controller]/[action]")]
[ApiController]
public class EnumController : ControllerBase
{
    readonly IEnumService _enumService;

    public EnumController(IEnumService enumService)
    {
        _enumService = enumService;
    }

    [HttpGet]
    public async Task<IActionResult> GetPositions()
    {
        return Ok(await _enumService.GetPositionEnum());
    }

    [HttpGet]
    public async Task<IActionResult> GetStates()
    {
        return Ok(await _enumService.GetStateEnum());
    }
}