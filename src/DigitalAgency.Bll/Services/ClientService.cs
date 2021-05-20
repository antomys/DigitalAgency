using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using DigitalAgency.Bll.Models;
using DigitalAgency.Bll.Models.Enums;
using DigitalAgency.Bll.Services.Interfaces;
using DigitalAgency.Dal.Entities;
using DigitalAgency.Dal.Storages.Interfaces;
using Task = System.Threading.Tasks.Task;

namespace DigitalAgency.Bll.Services
{
    public class ClientService: IClientService
    {
        private readonly IClientStorage _clientStorage;
        private readonly IExecutorStorage _executorStorage;
        private readonly IMapper _mapper;

        public ClientService(
            IClientStorage clientStorage, 
            IMapper mapper, 
            IExecutorStorage executorStorage)
        {
            _clientStorage = clientStorage;
            _mapper = mapper;
            _executorStorage = executorStorage;
        }
        public async Task<List<ClientModel>> GetClientsAsync()
        {
            return _mapper.Map<List<ClientModel>>(await _clientStorage.GetClientsAsync());
        }
        public async Task<List<ExecutorModel>> GetExecutorsAsync()
        {
            return _mapper.Map<List<ExecutorModel>>(await _executorStorage.GetExecutorsAsync());
        }
        public async Task<bool> CreateClientAsync(ClientModel clientModel)
        {
            if (clientModel.FirstName == null || clientModel.PhoneNumber == null)
                return false;
            
            clientModel.Orders = new List<OrderModel>();
            clientModel.Projects = new List<ProjectModel>();

            await _clientStorage.CreateClientAsync(_mapper.Map<Client>(clientModel));
            return false;
        }
        public async Task<bool> CreateExecutorAsync(ExecutorModel executor)
        {
            if (executor.FirstName == null || executor.PhoneNumber == null)
                return false;

            executor.Orders = new List<OrderModel>();
            
            await _executorStorage.CreateExecutorAsync(_mapper.Map<Executor>(executor));
            return false;
        }
        public async Task DeleteClientAsync(int id)
        {
            if (await _clientStorage.GetClientAsync(x => x.Id == id) != null)
                await _clientStorage.DeleteClientAsync(id);
        }
        public async Task DeleteExecutorAsync(int id)
        {
            if (await _executorStorage.GetExecutorAsync(x => x.Id == id) != null)
                await _executorStorage.DeleteExecutorAsync(id);
        }
        public async Task UpdateClientAsync(ClientModel client)
        {
            var thisClient = await _clientStorage.GetClientAsync(x => x.TelegramId == client.TelegramId);

            if (client is null || thisClient is null)
                return;
            var mappedClient = _mapper.Map<Client>(client);
            
            thisClient.Orders = mappedClient.Orders ?? thisClient.Orders;
            thisClient.Projects = mappedClient.Projects ?? thisClient.Projects;
            thisClient.FirstName = mappedClient.FirstName ?? thisClient.FirstName;
            thisClient.LastName = mappedClient.LastName ?? thisClient.LastName;

            await _clientStorage.UpdateClientAsync(thisClient);

        }
        public async Task UpdateExecutorAsync(ExecutorModel executor)
        {
            var thisExecutor = await _executorStorage.GetExecutorAsync(x => x.TelegramId == executor.TelegramId);

            if (executor is null || thisExecutor is null)
                return;
            var mappedExecutor = _mapper.Map<Executor>(executor);
            
            thisExecutor.Orders = mappedExecutor.Orders ?? thisExecutor.Orders;
            thisExecutor.Position =
                Enum.IsDefined(typeof(PositionsEnum), executor.Position)
                    ? (Dal.Entities.Enums.PositionsEnum) mappedExecutor.Position
                    : (Dal.Entities.Enums.PositionsEnum) PositionsEnum.Unknown;
            thisExecutor.FirstName = mappedExecutor.FirstName ?? thisExecutor.FirstName;
            thisExecutor.LastName = mappedExecutor.LastName ?? thisExecutor.LastName;

            await _executorStorage.UpdateExecutorAsync(thisExecutor);

        }
    }
}