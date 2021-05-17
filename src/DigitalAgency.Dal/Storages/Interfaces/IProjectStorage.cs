using System.Collections.Generic;
using System.Threading.Tasks;
using DigitalAgency.Dal.Entities;
using Task = System.Threading.Tasks.Task;

namespace DigitalAgency.Dal.Storages.Interfaces
{
    public interface IProjectStorage
    {
        Task<Project> CreateProjectAsync(Project newProject);
        Task<List<Project>> GetProjectAsync();
        Task<Project> GetLastAdded(int ownerId);
        Task<List<Project>> GetClientProjectsAsync(Client thisClient);
        Task<Project> GetProjectByIdAsync(int carId);
        Task DeleteProjectAsync(int id);
        Task UpdateProjectAsync(Project project);


        
    }
}
