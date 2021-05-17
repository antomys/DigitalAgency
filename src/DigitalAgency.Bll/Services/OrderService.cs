using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using DigitalAgency.Bll.Models;
using DigitalAgency.Bll.Services.Interfaces;
using DigitalAgency.Dal.Entities;
using DigitalAgency.Dal.Storages.Interfaces;
using Task = System.Threading.Tasks.Task;


namespace DigitalAgency.Bll.Services
{
    public class OrderService : IOrderService
    {
        private IOrderStorage _orderStorage;
        private IMapper _mapper;

        public OrderService(
            IOrderStorage orderStorage, 
            IMapper mapper)
        {
            _orderStorage = orderStorage;
            _mapper = mapper;
        }

        public Task<bool> ChangeOrderState(int id, string state)
        {
            throw new System.NotImplementedException();
        }

        public async Task<List<OrderModel>> GetOrdersAsync()
        {
            return _mapper.Map<List<OrderModel>>(await _orderStorage.GetOrdersAsync());
        }

        public async Task CreateOrderAsync(OrderModel order)
        {
            await _orderStorage.CreateOrderAsync(_mapper.Map<Order>(order));
        }

        public async Task UpdateAsync(OrderModel order)
        {
            if (await _orderStorage.GetOrder(x => x.Id == order.Id) != null)
                await _orderStorage.UpdateAsync(_mapper.Map<Order>(order));
        }

        public async Task<bool> DeleteOrder(int id)
        {
            if (await _orderStorage.GetOrder(order => order.Id == id) == null) return false;
            await _orderStorage.DeleteOrderAsync(id);
            return true;

        }
    }
}