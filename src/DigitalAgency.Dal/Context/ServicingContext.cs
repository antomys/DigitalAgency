using DigitalAgency.Dal.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DigitalAgency.Dal.Context
{

    public class ServicingContext : IdentityDbContext
    {
        public DbSet<Client> Clients { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Card> Tasks { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Executor> Executors { get; set; }
        public DbSet<Action> Actions { get; set; }

        public ServicingContext(DbContextOptions<ServicingContext> options) : base(options)
        {

        }
        public ServicingContext()
        {
            
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Action>().HasQueryFilter(filter => !filter.IsDone);
            base.OnModelCreating(modelBuilder);
        }
    }
}


