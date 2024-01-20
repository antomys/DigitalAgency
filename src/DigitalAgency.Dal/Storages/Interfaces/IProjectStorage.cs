using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DigitalAgency.Dal.Entities;
using Task = System.Threading.Tasks.Task;

namespace DigitalAgency.Dal.Storages.Interfaces;

public interface IProjectStorage
{
    Task<Project> CreateProjectAsync(Project newProject);
    Task<List<Project>> GetProjectsAsync();
    Task<Project> GetLastAdded(int ownerId);
    Task<List<Project>> GetProjectsAsync(Expression<Func<Project, bool>> expression);
    Task<bool> AnyAsync(Expression<Func<Project, bool>> expression);
    Task<Project> GetProjectAsync(Expression<Func<Project, bool>> expression);
    Task DeleteProjectAsync(int id);
    Task UpdateProjectAsync(Project project);
}