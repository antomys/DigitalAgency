using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using DigitalAgency.Bll.Models;
using DigitalAgency.Bll.Services.Interfaces;
using DigitalAgency.Dal.Entities;
using DigitalAgency.Dal.Storages.Interfaces;

namespace DigitalAgency.Bll.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IMapper _mapper;
        private readonly IProjectStorage _projectStorage;

        public ProjectService(IProjectStorage projectStorage, IMapper mapper)
        {
            _projectStorage = projectStorage;
            _mapper = mapper;
        }

        public async Task<List<ProjectModel>> GetProjectsAsync()
        {
            return _mapper.Map<List<ProjectModel>>(await _projectStorage.GetProjectsAsync());
        }

        public async Task CreateProjectAsync(ProjectModel project)
        {
            var mappedProject = _mapper.Map<Project>(project);
            if(await _projectStorage
                .GetProjectAsync(x=> x.ProjectName == mappedProject.ProjectName && x.Client.Id == mappedProject.Client.Id) != null)
                await _projectStorage.CreateProjectAsync(_mapper.Map<Project>(project));
        }

        public async Task DeleteProjectAsync(int id)
        {
            if(await _projectStorage.GetProjectAsync(x=> x.Id == id) != null)
                await _projectStorage.DeleteProjectAsync(id);
        }

        public async Task UpdateProjectAsync(ProjectModel project)
        {
            if(await _projectStorage.GetProjectAsync(x=> x.Id == project.Id) != null)
                await _projectStorage.DeleteProjectAsync(project.Id);
        }
    }
}