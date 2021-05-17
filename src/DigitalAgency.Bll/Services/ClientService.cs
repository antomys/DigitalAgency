using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DigitalAgency.Bll.Services.Interfaces;
using DigitalAgency.Dal.Context;
using DigitalAgency.Dal.Entities;
using Microsoft.EntityFrameworkCore;
using Task = System.Threading.Tasks.Task;

namespace DigitalAgency.Bll.Services
{
    public class ClientService : IClientService
    {
        private readonly ServicingContext _context;
        public ClientService (ServicingContext context)
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
        public async Task<Client> GetClientByIdAsync(int id)
        {
            return await _context.Clients.FirstOrDefaultAsync(x=> x.Id == id) ?? new Client();
        }
        public async Task UpdateClientAsync(Client client)
        {
            _context.Clients.Update(client);
            await _context.SaveChangesAsync();
        }
        public async Task<bool> CreateNewClient(Telegram.Bot.Types.Message receivedMessage)
        {
            var sender = receivedMessage.From;
            
            var phoneNumber = receivedMessage.Contact.PhoneNumber;
            if (!phoneNumber.Contains('+'))
                phoneNumber = '+' + phoneNumber;
            
            if (await GetClientAsync(x => x.TelegramId == sender.Id) != null) return false;
            
            var newClient = new Client 
            {
                FirstName = sender.FirstName, 
                MiddleName = '@'+sender.Username, 
                LastName = sender.LastName, 
                PhoneNumber = phoneNumber, 
                TelegramId = sender.Id,
                ChatId = receivedMessage.Chat.Id,
            };
            await CreateClientAsync(newClient);
            return true;
        }
    }
}
