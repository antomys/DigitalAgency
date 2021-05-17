﻿using System;
using System.Linq;
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
    public class ProjectController : ControllerBase
    {
        private readonly IProjectService _projectService;
        private readonly IValidator<Project> _projectValidator;
        private readonly ILogger<ProjectController> _logger;
        public ProjectController(IProjectService projectService, IValidator<Project> carValidator, ILogger<ProjectController> logger)
        {
            _projectService = projectService;
            _projectValidator = carValidator;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetProject()
        {
            _logger.LogInformation("Star logging - method GetCar controller ProjectContoller");
            var result = await _projectService.GetProjectAsync();
            _logger.LogDebug("Time request {Time}",  DateTime.UtcNow);
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<Project>> PostProject(Project project)
        {
            _logger.LogInformation("Star logging - method PostCar controller ProjectContoller");
            var result = _projectValidator.Validate(project);
            if (!result.IsValid)
            {
                return BadRequest(result.Errors.Select(x => new { Error = x.ErrorMessage, Code = x.ErrorCode }).ToList());
            }
            await _projectService.CreateProjectAsync(project);
            _logger.LogDebug("Time request {Time}", DateTime.UtcNow);
            return Ok(project);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(int id)
        {
            _logger.LogInformation("Star logging - method DeleteCar controller ProjectContoller");
            await _projectService.DeleteProjectAsync(id);
            _logger.LogDebug("Time request {Time}", DateTime.UtcNow);
            return Ok(id);
        }

        [HttpPut]
        public async Task<IActionResult> PutProject(Project project)
        {
            _logger.LogInformation("Star logging - method PutCar controller ProjectContoller");
            var result = _projectValidator.Validate(project);
            if (!result.IsValid)
            {
                return BadRequest(result.Errors.Select(x => new { Error = x.ErrorMessage, Code = x.ErrorCode }).ToList());
            }
            await _projectService.UpdateProjectAsync(project);
            _logger.LogDebug("Time request {Time}", DateTime.UtcNow);
            return Ok(project);
        }
    }
}