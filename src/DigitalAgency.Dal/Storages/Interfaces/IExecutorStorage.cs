using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DigitalAgency.Dal.Entities;
using Task = System.Threading.Tasks.Task;

namespace DigitalAgency.Dal.Storages.Interfaces;

public interface IExecutorStorage
{
    Task CreateExecutorAsync(Executor newClient);
    Task<List<Executor>> GetExecutorsAsync();
    Task<Executor> GetExecutorAsync(Expression<Func<Executor, bool>> expression);
    Task<Executor> GetExecutorByIdAsync(int id);
    Task DeleteExecutorAsync(int id);
    Task UpdateExecutorAsync(Executor client);
}