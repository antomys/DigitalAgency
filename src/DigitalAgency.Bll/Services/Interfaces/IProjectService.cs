using System.Collections.Generic;
using System.Threading.Tasks;
using DigitalAgency.Bll.Models;

namespace DigitalAgency.Bll.Services.Interfaces
{
    public interface IProjectService
    {
        Task<List<ProjectModel>> GetProjectsAsync();
        Task<bool> CreateProjectAsync(ProjectModel project);
        Task DeleteProjectAsync(int id);
        Task UpdateProjectAsync(ProjectModel project);
    }
}