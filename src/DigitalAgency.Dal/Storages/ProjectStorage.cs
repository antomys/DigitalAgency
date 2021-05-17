using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DigitalAgency.Dal.Context;
using DigitalAgency.Dal.Entities;
using DigitalAgency.Dal.Storages.Interfaces;
using Microsoft.EntityFrameworkCore;
using Task = System.Threading.Tasks.Task;

namespace DigitalAgency.Dal.Storages
{
    public class ProjectStorage : IProjectStorage
    {
        private readonly ServicingContext _context;
        public ProjectStorage(ServicingContext context)
        {
            _context = context;


        }
        public async Task<Project> CreateProjectAsync(Project newProject)
        {
            var thisCar = await _context.Projects.AddAsync(newProject);
            await _context.SaveChangesAsync();
            return thisCar.Entity;
        }
        public async Task<Project> GetLastAdded(int ownerId)
        {
            return await _context.Projects.OrderBy(x=> x.Id).Where(x => x.OwnerId == ownerId).LastOrDefaultAsync();
        }
        public async Task DeleteProjectAsync(int id)
        {
            var client = await _context.Projects.FindAsync(id);

            _context.Projects.Remove(client);
            await _context.SaveChangesAsync();
        }
        public async Task<List<Project>> GetProjectAsync()
        {
            return await _context.Projects.ToListAsync();
        }
        public async Task<List<Project>> GetClientProjectsAsync(Client thisClient)
        {
            return await _context.Projects.Where(x => x.OwnerId == thisClient.Id).ToListAsync();
        }
        public async Task<Project> GetProjectByIdAsync(int carId)
        {
            return await _context.Projects.FindAsync(carId);
        }
        public async Task UpdateProjectAsync(Project project)
        {
            var existingCar = await _context.Projects.FirstOrDefaultAsync(c => c.Id == project.Id);
            if (existingCar != null)
            {
                existingCar.ProjectName = project.ProjectName;
                existingCar.ProjectDescription = project.ProjectDescription;
                existingCar.ProjectLink = project.ProjectLink;
                await _context.SaveChangesAsync();
            }
        }
    }
}
