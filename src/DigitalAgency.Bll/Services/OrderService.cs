using System.Collections.Generic;
using System.Threading.Tasks;
using DigitalAgency.Bll.Models;
using DigitalAgency.Bll.Services.Interfaces;
using DigitalAgency.Dal.Entities;
using DigitalAgency.Dal.Storages.Interfaces;


namespace DigitalAgency.Bll.Services
{
    public class OrderService : IOrderService
    {
        private IOrderStorage _orderStorage;

        public OrderService(IOrderStorage orderStorage)
        {
            _orderStorage = orderStorage;
        }

        public Task<bool> ChangeOrderState(int id, string state)
        {
            throw new System.NotImplementedException();
        }
        
        public async Task<List<Order>> GetClientOrdersAsync(ClientModel thisClient)
        {
            if (thisClient != null)
            {
                return await _orderStorage.GetOrdersAsync(order => order.ClientId == thisClient.Id);
            }

            return null;
        }
    }
}