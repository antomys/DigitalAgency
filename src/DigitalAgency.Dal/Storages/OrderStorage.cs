using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DigitalAgency.Dal.Context;
using DigitalAgency.Dal.Entities;
using DigitalAgency.Dal.Storages.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Task = System.Threading.Tasks.Task;


namespace DigitalAgency.Dal.Storages
{
    public class OrderStorage : IOrderStorage
    {
        private readonly ServicingContext _context;
        public OrderStorage(ServicingContext context)
        {
            _context = context;

        }
        public async Task CreateOrderAsync(Order order)
        {
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
        }
        public async Task<List<Order>> GetOrdersAsync()
        {
            var result = await _context.Orders
                .ToListAsync();
            return result;
        }

        public async Task<Order> GetOrder(Expression<Func<Order, bool>> expression)
        {
            return await _context.Orders.Where(expression)
                .Include(x => x.Executor)
                .Include(x => x.Tasks)
                .Include(x => x.Client)
                .ThenInclude(x => x.Projects).FirstOrDefaultAsync();
        }

        public async Task<Order> GetLastAdded(int clientId)
        {
            return await _context.Orders.OrderBy(x=> x.Id).Where(x => x.ClientId == clientId).LastOrDefaultAsync();
        }
        public async Task UpdateAsync(Order order)
        {
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
        }
        public async Task<List<Order>> GetOrdersAsync(Expression<Func<Order,bool>> expression)
        {
            return await _context.Orders.Where(expression)
                .Include(x => x.Project)
                .Include(x => x.Executor)
                .Include(x => x.Client).ToListAsync();
        }
        public async Task DeleteOrderAsync(int id)
        {
            var thisService = await _context.Orders.FirstOrDefaultAsync(x => x.Id == id);
            _context.Remove(thisService);
            await _context.SaveChangesAsync();
        }
        public bool DeleteOrder(int id)
        {
            if (id < 0)
                return false;

            var order = _context.Orders.FirstOrDefault(c => c.Id == id);
            if (order == null)
                return false;

            _context.Remove(order);
            _context.SaveChanges();
            return true;
        }
    }
}
