using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DigitalAgency.Dal.Entities;
using Task = System.Threading.Tasks.Task;

namespace DigitalAgency.Bll.Services.Interfaces
{
    public interface IClientService
    {
        Task CreateClientAsync(Client newClient);
        Task<bool> CreateNewClient(Telegram.Bot.Types.Message receivedMessage);
        Task<List<Client>> GetClientsAsync();
        Task<Client> GetClientAsync(Expression<Func<Client, bool>> expression);
        Task<Client> GetClientByIdAsync(int id);
        Task DeleteClientAsync(int id);
        Task UpdateClientAsync(Client client);
        
    }
}
