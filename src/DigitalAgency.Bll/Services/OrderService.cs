using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DigitalAgency.Bll.DTOs;
using DigitalAgency.Bll.Services.Interfaces;
using DigitalAgency.Dal.Context;
using DigitalAgency.Dal.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Task = System.Threading.Tasks.Task;


namespace DigitalAgency.Bll.Services
{
    public class OrderService : IOrderService
    {
        private readonly ServicingContext _context;
        private readonly ILogger<Order> _logger;
        public OrderService(ServicingContext context, ILogger<Order> logger)
        {
            _context = context;
            _logger = logger;

        }
        public async Task CreateOrderAsync(Order order)
        {
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
        }
        public async Task<List<int>> GetBusyTimeByDay(DateTime date)
        {
            _logger.LogInformation("Entered the GetBusyTimeByDay method with " + date);

            var todayOrders = await _context.Orders.Where(c => c.ScheduledTime.Day == date.Day ).ToListAsync();
            List<int> busyHour = new List<int>();
            for (int i = 0; i < todayOrders.Count(); i++)
                busyHour.Add(todayOrders[i].ScheduledTime.Hour);
            return busyHour;
        }
        public async Task<List<Order>> GetOrdersAsync()
        {
            var result = await _context.Orders
                .ToListAsync();
            return result;
        }

        public async Task<List<Order>> GetFullOrdersAsync()
        {
            return await _context.Orders.Include(x=>x.Project)
                .Include(x=> x.Executor)
                .Include(x=> x.Client)
                .ToListAsync();
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

        public async Task<List<Order>> GetClientOrdersAsync(Client thisClient)
        {
            return await _context.Orders.Where(x => x.ClientId == thisClient.Id)
                .Include(x=> x.Project).ToListAsync();
        }

        public async Task<List<Order>> GetExecutorOrdersAsync(int mechanicId)
        {
            return await _context.Orders.Where(x => x.ExecutorId == mechanicId).Include(x=> x.Project).ToListAsync();
        }

        public async Task<Order> GetOrderByIdAsync(int id)
        {
            return await _context.Orders.Where(x=> x.Id == id).Include(x=> x.Project).FirstOrDefaultAsync();
        }
        public Order ScheduleOrder(OrderDTO orderDto)
        {
            var client = _context.Clients.FirstOrDefault(c => c.FirstName == orderDto.FirstName && c.MiddleName == orderDto.MiddleName && c.LastName == orderDto.LastName);
            if (client == null)
            {
                client = new Client
                {
                    FirstName = orderDto.FirstName,
                    MiddleName = orderDto.MiddleName,
                    LastName = orderDto.LastName,
                    PhoneNumber = orderDto.ClientPhone
                };
                _context.Clients.Add(client);

            }

            var car = _context.Projects.
                Where((c => c.ProjectName == orderDto.CarMake && c.ProjectDescription == orderDto.CarModel && c.ProjectLink == orderDto.CarColor)).FirstOrDefault();
            if (car == null)
            {
                car = new Project
                {
                    ProjectName = orderDto.CarMake,
                    ProjectDescription = orderDto.CarModel,
                    ProjectLink = orderDto.CarColor,

                };
                _context.Projects.Add(car);

            }

            var serviceOrder = _context.Add(new Order
            {
                Client = client,
                Project = car,
                ScheduledTime = orderDto.ScheduledDate,
                CreationDate = DateTime.UtcNow

            });

            _context.SaveChanges();
            return serviceOrder.Entity;

        }
        public bool ChangeOrderState(int id, string state)
        {
            if (id < 0 || string.IsNullOrEmpty(state))
                return false;

            var order = _context.Orders.FirstOrDefault(c => c.Id == id);
            if (order == null)
                return false;

            order.State = state;
            _context.SaveChanges();
            return true;
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
