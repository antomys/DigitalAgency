using System.Collections.Generic;
using System.Threading.Tasks;
using DigitalAgency.Bll.Models;
using DigitalAgency.Bll.Services.Interfaces;
using DigitalAgency.Dal.Entities;
using DigitalAgency.Dal.Storages.Interfaces;
using Task = System.Threading.Tasks.Task;

namespace DigitalAgency.Bll.Services
{
    public class ClientService: IClientService
    {
        private readonly IClientStorage _clientStorage;

        public ClientService(IClientStorage clientStorage)
        {
            _clientStorage = clientStorage;
        }
        public async Task<bool> CreateNewClient(Telegram.Bot.Types.Message receivedMessage)
        {
            var sender = receivedMessage.From;
            
            var phoneNumber = receivedMessage.Contact.PhoneNumber;
            if (!phoneNumber.Contains('+'))
                phoneNumber = '+' + phoneNumber;
            
            if (await _clientStorage.GetClientAsync(x => x.TelegramId == sender.Id) != null) return false;
            
            var newClient = new Client 
            {
                FirstName = sender.FirstName, 
                MiddleName = '@'+sender.Username, 
                LastName = sender.LastName, 
                PhoneNumber = phoneNumber, 
                TelegramId = sender.Id,
                ChatId = receivedMessage.Chat.Id,
            };
            await _clientStorage.CreateClientAsync(newClient);
            return true;
        }
        public Task<List<ClientModel>> GetClientsAsync()
        {
            throw new System.NotImplementedException();
        }
        public Task<List<ExecutorModel>> GetExecutorsAsync()
        {
            throw new System.NotImplementedException();
        }
        public Task CreateClientAsync(ClientModel clientModel)
        {
            throw new System.NotImplementedException();
        }
        public Task CreateExecutorAsync(ExecutorModel executor)
        {
            throw new System.NotImplementedException();
        }
        public Task DeleteClientAsync(int id)
        {
            throw new System.NotImplementedException();
        }
        public Task DeleteExecutorAsync(int id)
        {
            throw new System.NotImplementedException();
        }
        public Task UpdateClientAsync(ClientModel client)
        {
            throw new System.NotImplementedException();
        }
        public Task UpdateExecutorAsync(Executor executor)
        {
            throw new System.NotImplementedException();
        }
    }
}