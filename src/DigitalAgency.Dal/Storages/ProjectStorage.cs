using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DigitalAgency.Dal.Context;
using DigitalAgency.Dal.Entities;
using DigitalAgency.Dal.Storages.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Task = System.Threading.Tasks.Task;

namespace DigitalAgency.Dal.Storages;

public class ProjectStorage : IProjectStorage
{
    readonly ServicingContext _context;

    public ProjectStorage(ServicingContext context)
    {
        _context = context;
    }

    public async Task<Project> CreateProjectAsync(Project newProject)
    {
        EntityEntry<Project> thisCar = await _context.Projects.AddAsync(newProject);
        await _context.SaveChangesAsync();
        return thisCar.Entity;
    }

    public async Task<Project> GetLastAdded(int ownerId)
    {
        return await _context.Projects.OrderBy(x => x.Id).Where(x => x.OwnerId == ownerId).LastOrDefaultAsync();
    }

    public async Task<List<Project>> GetProjectsAsync(Expression<Func<Project, bool>> expression)
    {
        return await _context.Projects.Where(expression)
            .Include(x => x.Orders)
            .ThenInclude(x => x.Client).ToListAsync();
    }

    public async Task<bool> AnyAsync(Expression<Func<Project, bool>> expression)
    {
        return await _context.Projects.AnyAsync(expression);
    }

    public async Task<Project> GetProjectAsync(Expression<Func<Project, bool>> expression)
    {
        return await _context.Projects.Where(expression)
            .Include(x => x.Client)
            .ThenInclude(x => x.Orders).FirstOrDefaultAsync();
    }

    public async Task DeleteProjectAsync(int id)
    {
        Project client = await _context.Projects.FindAsync(id);

        _context.Projects.Remove(client);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Project>> GetProjectsAsync()
    {
        return await _context.Projects.Include(x => x.Client).ToListAsync();
    }

    public async Task UpdateProjectAsync(Project project)
    {
        _context.Update(project);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Project>> GetClientProjectsAsync(Client thisClient)
    {
        return await _context.Projects.Where(x => x.OwnerId == thisClient.Id).ToListAsync();
    }

    public async Task<Project> GetProjectByIdAsync(int carId)
    {
        return await _context.Projects.FindAsync(carId);
    }
}