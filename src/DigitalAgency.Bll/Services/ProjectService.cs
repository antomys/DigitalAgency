using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using DigitalAgency.Bll.Models;
using DigitalAgency.Bll.Services.Interfaces;
using DigitalAgency.Dal.Entities;
using DigitalAgency.Dal.Storages.Interfaces;

namespace DigitalAgency.Bll.Services;

public class ProjectService : IProjectService
{
    readonly IMapper _mapper;
    readonly IProjectStorage _projectStorage;

    public ProjectService(IProjectStorage projectStorage, IMapper mapper)
    {
        _projectStorage = projectStorage;
        _mapper = mapper;
    }

    public async Task<List<ProjectModel>> GetProjectsAsync()
    {
        return _mapper.Map<List<ProjectModel>>(await _projectStorage.GetProjectsAsync());
    }

    public async Task<bool> CreateProjectAsync(ProjectModel project)
    {
        Project thisProject = await _projectStorage
            .GetProjectAsync(x => x.ProjectName == project.ProjectName && x.Client.Id == project.Client.Id);
        if (project.Client == null || thisProject != null)
        {
            return false;
        }
        Project mappedProject = _mapper.Map<Project>(project);
        await _projectStorage.CreateProjectAsync(mappedProject);
        return true;
    }

    public async Task DeleteProjectAsync(int id)
    {
        if (await _projectStorage.GetProjectAsync(x => x.Id == id) != null)
            await _projectStorage.DeleteProjectAsync(id);
    }

    public async Task UpdateProjectAsync(ProjectModel project)
    {
        Project thisProject = await _projectStorage.GetProjectAsync(x => x.Id == project.Id);
        if (thisProject?.Client == null) return;

        Project mappedProject = _mapper.Map<Project>(project);

        thisProject.ProjectDescription = mappedProject.ProjectDescription ?? thisProject.ProjectDescription;
        thisProject.ProjectLink = mappedProject.ProjectLink ?? thisProject.ProjectDescription;
        thisProject.ProjectFilePath = mappedProject.ProjectFilePath ?? thisProject.ProjectFilePath;

        await _projectStorage.UpdateProjectAsync(thisProject);
    }
}