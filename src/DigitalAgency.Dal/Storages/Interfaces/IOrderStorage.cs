using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DigitalAgency.Dal.Entities;
using Task = System.Threading.Tasks.Task;

namespace DigitalAgency.Dal.Storages.Interfaces;

public interface IOrderStorage
{
    Task CreateOrderAsync(Order order);
    Task<List<Order>> GetOrdersAsync();
    Task<Order> GetOrderAsync(Expression<Func<Order, bool>> expression);
    Task<Order> GetLastAdded(int clientId);
    Task UpdateAsync(Order order);
    Task<List<Order>> GetOrdersAsync(Expression<Func<Order, bool>> expression);
    Task<bool> AnyAsync(Expression<Func<Order, bool>> expression);
    Task DeleteOrderAsync(int id);
    bool DeleteOrder(int id);
}