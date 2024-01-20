using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DigitalAgency.Bll.Models;
using DigitalAgency.Bll.Services.Interfaces;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DigitalAgency.Api.Controllers;

[Route("[controller]/[action]")]
[ApiController]
public class ProjectController : ControllerBase
{
    readonly ILogger<ProjectController> _logger;
    readonly IProjectService _projectService;
    readonly IValidator<ProjectModel> _projectValidator;

    public ProjectController(
        IProjectService projectService,
        IValidator<ProjectModel> projectValidator,
        ILogger<ProjectController> logger)
    {
        _projectService = projectService;
        _projectValidator = projectValidator;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetProject()
    {
        _logger.LogInformation("Star logging - method GetCar controller ProjectContoller");
        List<ProjectModel> result = await _projectService.GetProjectsAsync();
        _logger.LogDebug("Time request {Time}", DateTime.UtcNow);
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<ProjectModel>> PostProject(ProjectModel project)
    {
        _logger.LogInformation("Star logging - method PostCar controller ProjectContoller");
        ValidationResult result = await _projectValidator.ValidateAsync(project);
        if (!result.IsValid)
        {
            return BadRequest(result.Errors.Select(x => new {
                Error = x.ErrorMessage, Code = x.ErrorCode,
            }).ToList());
        }
        _logger.LogDebug("Time request {Time}", DateTime.UtcNow);
        if (await _projectService.CreateProjectAsync(project))
            return Ok(project);
        return BadRequest();
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteProject(int id)
    {
        _logger.LogInformation("Star logging - method DeleteCar controller ProjectContoller");
        await _projectService.DeleteProjectAsync(id);
        _logger.LogDebug("Time request {Time}", DateTime.UtcNow);
        return Ok(id);
    }

    [HttpPut]
    public async Task<IActionResult> PutProject(ProjectModel project)
    {
        _logger.LogInformation("Star logging - method PutCar controller ProjectContoller");
        ValidationResult result = await _projectValidator.ValidateAsync(project);
        if (!result.IsValid)
        {
            return BadRequest(result.Errors.Select(x => new {
                Error = x.ErrorMessage, Code = x.ErrorCode,
            }).ToList());
        }
        await _projectService.UpdateProjectAsync(project);
        _logger.LogDebug("Time request {Time}", DateTime.UtcNow);
        return Ok(project);
    }
}