using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DigitalAgency.Bll.DTOs;
using DigitalAgency.Dal.Entities;
using Task = System.Threading.Tasks.Task;

namespace DigitalAgency.Bll.Services.Interfaces
{
    public interface IOrderService
    {
        Task CreateOrderAsync(Order order);
        Task<List<int>> GetBusyTimeByDay(DateTime date);
        Task<List<Order>> GetOrdersAsync();
        Task<List<Order>> GetFullOrdersAsync();
        Task<Order> GetLastAdded(int clientId);
        Task UpdateAsync(Order order);
        Task<List<Order>> GetClientOrdersAsync(Client thisClient);
        Task<List<Order>> GetExecutorOrdersAsync(int mechanicId);
        Task<Order> GetOrderByIdAsync(int id);
        Order ScheduleOrder(OrderDTO orderDto);
        bool ChangeOrderState(int id, string state);
        Task DeleteOrderAsync(int id);
        bool DeleteOrder(int id);
    }
}
