using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using DigitalAgency.Bll.Models;
using DigitalAgency.Bll.Models.Enums;
using DigitalAgency.Bll.Services.Interfaces;
using DigitalAgency.Dal.Entities;
using DigitalAgency.Dal.Storages.Interfaces;

namespace DigitalAgency.Bll.Services;

public class OrderService : IOrderService
{
    readonly IMapper _mapper;
    readonly IOrderStorage _orderStorage;

    public OrderService(
        IOrderStorage orderStorage,
        IMapper mapper)
    {
        _orderStorage = orderStorage;
        _mapper = mapper;
    }

    public Task<bool> ChangeOrderState(int id, string state)
    {
        throw new NotImplementedException();
    }

    public async Task<List<OrderModel>> GetOrdersAsync()
    {
        List<Order> thisOrders = await _orderStorage.GetOrdersAsync();
        return _mapper.Map<List<OrderModel>>(thisOrders);
    }

    public async Task CreateOrderAsync(OrderModel order)
    {
        if (order.Client == null || order.Project == null)
            return;
        order.StateEnum = OrderStateEnum.New;
        order.CreationDate = DateTime.UtcNow;

        Order mapped = _mapper.Map<Order>(order);
        if (order.Executor != null)
        {
            mapped.ExecutorId = order.Executor.Id;
        }
        else
        {
            mapped.Executor = null;
        }

        await _orderStorage.CreateOrderAsync(mapped);
    }

    public async Task<bool> UpdateAsync(OrderModel order)
    {
        Order thisOrder = await _orderStorage.GetOrderAsync(x => x.Id == order.Id);
        if (thisOrder == null)
            return false;
        Order mappedOrder = _mapper.Map<Order>(order);

        thisOrder.ExecutorId = mappedOrder.ExecutorId ?? thisOrder.Executor.Id;
        thisOrder.ScheduledTime = mappedOrder.ScheduledTime;

        await _orderStorage.UpdateAsync(thisOrder);
        return true;
    }

    public async Task<bool> DeleteOrder(int id)
    {
        if (await _orderStorage.GetOrderAsync(order => order.Id == id) == null) return false;
        await _orderStorage.DeleteOrderAsync(id);
        return true;
    }
}