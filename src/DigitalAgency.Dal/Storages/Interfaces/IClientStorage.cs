using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DigitalAgency.Dal.Entities;
using Task = System.Threading.Tasks.Task;

namespace DigitalAgency.Dal.Storages.Interfaces
{
    public interface IClientStorage
    {
        Task CreateClientAsync(Client newClient);
        Task<List<Client>> GetClientsAsync();
        Task<Client> GetClientAsync(Expression<Func<Client, bool>> expression);
        Task DeleteClientAsync(int id);
        Task UpdateClientAsync(Client client);
    }
}
