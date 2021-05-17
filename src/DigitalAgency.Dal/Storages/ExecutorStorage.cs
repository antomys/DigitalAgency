using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DigitalAgency.Dal.Context;
using DigitalAgency.Dal.Entities;
using DigitalAgency.Dal.Storages.Interfaces;
using Microsoft.EntityFrameworkCore;
using Task = System.Threading.Tasks.Task;

namespace DigitalAgency.Dal.Storages
{
    public class ExecutorStorage : IExecutorStorage
    {
        private readonly ServicingContext _context;
        public ExecutorStorage (ServicingContext context)
        {
            _context = context;
            
        }
        public async Task CreateExecutorAsync(Executor newClient)
        {
            _context.Executors.Add(newClient);
            await _context.SaveChangesAsync();
        }
        public async Task<Executor> GetExecutorAsync(Expression<Func<Executor, bool>> expression)
        {
            return await _context.Executors.FirstOrDefaultAsync(expression);
        }
        public async Task DeleteExecutorAsync(int id)
        {
            var client = await _context.Executors.FindAsync(id);
            
            _context.Executors.Remove(client);
            await _context.SaveChangesAsync();
        }
        public async Task<List<Executor>> GetExecutorsAsync()
        {
            return await _context.Executors.ToListAsync();
        }
        public async Task<Executor> GetExecutorByIdAsync(int id)
        {
            return await _context.Executors.FirstOrDefaultAsync(x=> x.Id == id) ?? new Executor();
        }
        public async Task UpdateExecutorAsync(Executor client)
        {
            _context.Executors.Update(client);
            await _context.SaveChangesAsync();
        }
    }
}