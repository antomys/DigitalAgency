using System.Threading.Tasks;
using DigitalAgency.Bll.Models;

namespace DigitalAgency.Bll.Services.Interfaces
{
    public interface IProjectService
    {
        Task<object> GetProjectAsync();
        Task CreateProjectAsync(ProjectModel project);
        Task DeleteProjectAsync(int id);
        Task UpdateProjectAsync(ProjectModel project);
    }
}