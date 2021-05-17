using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DigitalAgency.Dal.Context;
using DigitalAgency.Dal.Entities;
using DigitalAgency.Dal.Storages.Interfaces;
using Microsoft.EntityFrameworkCore;
using Task = System.Threading.Tasks.Task;

namespace DigitalAgency.Dal.Storages
{
    public class ClientStorage : IClientStorage
    {
        private readonly ServicingContext _context;
        public ClientStorage (ServicingContext context)
        {
            _context = context;
            
        }
        public async Task CreateClientAsync(Client newClient)
        {
            _context.Clients.Add(newClient);
            await _context.SaveChangesAsync();
        }
        public async Task<Client> GetClientAsync(Expression<Func<Client, bool>> expression)
        {
            return await _context.Clients.FirstOrDefaultAsync(expression);
        }
        public async Task DeleteClientAsync(int id)
        {
            var client = await _context.Clients.FindAsync(id);
            
            _context.Clients.Remove(client);
            await _context.SaveChangesAsync();
        }
        public async Task<List<Client>> GetClientsAsync()
        {
            return await _context.Clients.ToListAsync();
        }
        public async Task UpdateClientAsync(Client client)
        {
            _context.Clients.Update(client);
            await _context.SaveChangesAsync();
        }

    }
}
