using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DigitalAgency.Dal.Entities;
using Task = System.Threading.Tasks.Task;

namespace DigitalAgency.Bll.Services.Interfaces
{
    public interface IExecutorService
    {
        Task CreateExecutorAsync(Executor newClient);
        Task<List<Executor>> GetExecutorsAsync();
        Task<Executor> GetExecutorAsync(Expression<Func<Executor, bool>> expression);
        Task<Executor> GetExecutorByIdAsync(int id);
        Task DeleteExecutorAsync(int id);
        Task UpdateExecutorAsync(Executor client);
        
    }
}