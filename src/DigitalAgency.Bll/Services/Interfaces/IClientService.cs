using System.Collections.Generic;
using System.Threading.Tasks;
using DigitalAgency.Bll.Models;
using Task = System.Threading.Tasks.Task;

namespace DigitalAgency.Bll.Services.Interfaces;

public interface IClientService
{
    Task<List<ClientModel>> GetClientsAsync();
    Task<List<ExecutorModel>> GetExecutorsAsync();
    Task<bool> CreateClientAsync(ClientModel clientModel);
    Task<bool> CreateExecutorAsync(ExecutorModel executor);
    Task DeleteClientAsync(int id);
    Task DeleteExecutorAsync(int id);
    Task UpdateClientAsync(ClientModel client);
    Task UpdateExecutorAsync(ExecutorModel executor);
}