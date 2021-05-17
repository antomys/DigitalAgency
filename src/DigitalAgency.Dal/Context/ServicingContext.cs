﻿using DigitalAgency.Dal.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DigitalAgency.Dal.Context
{

    public class ServicingContext : IdentityDbContext
    {
        public DbSet<Client> Clients { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Task> Tasks { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Executor> Executors { get; set; }

        public ServicingContext(DbContextOptions<ServicingContext> options) : base(options)
        {

        }
        public ServicingContext()
        {
            
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<OrderTask>().HasKey(sc => new {AutoPartId = sc.TaskId, ServiceOrderId = sc.OrderId});

            modelBuilder.Entity<OrderTask>()
                .HasOne(sc => sc.Task)
                .WithMany(s => s.OrderTasks)
                .HasForeignKey(sc => sc.TaskId);


            modelBuilder.Entity<OrderTask>()
                .HasOne(sc => sc.Order)
                .WithMany(s => s.OrderParts)
                .HasForeignKey(sc => sc.OrderId);
        }
    }
}


