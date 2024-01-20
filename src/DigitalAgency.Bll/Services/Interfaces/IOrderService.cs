using System.Collections.Generic;
using System.Threading.Tasks;
using DigitalAgency.Bll.Models;
using Task = System.Threading.Tasks.Task;

namespace DigitalAgency.Bll.Services.Interfaces;

public interface IOrderService
{
    Task<bool> ChangeOrderState(int id, string state);
    Task<List<OrderModel>> GetOrdersAsync();
    Task CreateOrderAsync(OrderModel order);
    Task<bool> UpdateAsync(OrderModel order);
    Task<bool> DeleteOrder(int id);
}