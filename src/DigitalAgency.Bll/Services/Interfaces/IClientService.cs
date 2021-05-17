using System.Collections.Generic;
using System.Threading.Tasks;
using DigitalAgency.Bll.Models;
using DigitalAgency.Dal.Entities;
using Telegram.Bot.Types;
using Task = System.Threading.Tasks.Task;

namespace DigitalAgency.Bll.Services.Interfaces
{
    public interface IClientService
    {
        Task<bool> CreateNewClient(Message receivedMessage);
        Task<List<ClientModel>> GetClientsAsync();
        Task<List<ExecutorModel>> GetExecutorsAsync();
        Task CreateClientAsync(ClientModel clientModel);
        Task CreateExecutorAsync(ExecutorModel executor);
        Task DeleteClientAsync(int id);
        Task DeleteExecutorAsync(int id);
        Task UpdateClientAsync(ClientModel client);
        Task UpdateExecutorAsync(Executor executor);
    }
}