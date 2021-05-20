using System;
using System.Threading.Tasks;
using DigitalAgency.Bll.TelegramBot.Services.Interfaces;
using DigitalAgency.Dal.Entities;
using DigitalAgency.Dal.Storages.Interfaces;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace DigitalAgency.Bll.TelegramBot.Services
{
    public class BotService : IBotService
    {
        private readonly IClientStorage _clientStorage;
        private readonly IExecutorStorage _executorStorage;
        private readonly IExecutorMenu _executorMenu;
        private readonly IClientMenu _clientMenu;
        private readonly IRegistrationService _registrationService;
        private bool _isSuccessSingUp;


        public BotService(
            IClientStorage clientStorage, 
            IClientMenu clientMenu, 
            IExecutorMenu executorMenu, 
            IExecutorStorage executorStorage, 
            IRegistrationService registrationService)
        {
            _clientStorage = clientStorage;
            _clientMenu = clientMenu;
            _executorMenu = executorMenu;
            _executorStorage = executorStorage;
            _registrationService = registrationService;
        }
        public async Task ProcessMessageAsync(Update update)
        { 
            if (update.Type is UpdateType.Unknown)
                return;
            var thisClient =
                await _clientStorage.GetClientAsync(client => client.TelegramId == update.Message.From.Id);
            var thisExecutor =
                await _executorStorage.GetExecutorAsync(client => client.TelegramId == update.Message.From.Id);
            
            var receivedMessage = update.Message;
            
            if (receivedMessage == null)
                throw new ArgumentNullException($"{nameof(receivedMessage)} is null! [if (receivedMessage == null) BOTSERVICE]");

            if (thisExecutor?.FirstName == null 
                || thisClient?.FirstName == null) 
            {
                if (thisClient != null && thisClient.ChatId != 0
                    || thisExecutor != null && thisExecutor.ChatId != 0)
                {
                    await MenuSelector(thisClient, thisExecutor, update);
                }
                else
                {
                    _isSuccessSingUp = await _registrationService.StartRegistration(update);
                }
            }
            else
            {
                await MenuSelector(thisClient, thisExecutor, update);
            }
            
            // Just to ensure we always will get into this menu selector
            if(_isSuccessSingUp)
            {
                thisClient = await _clientStorage.GetClientAsync(x => x.TelegramId == update.Message.From.Id);
                thisExecutor = await _executorStorage.GetExecutorAsync(x => x.TelegramId == update.Message.From.Id);
                await MenuSelector(thisClient, thisExecutor, update);
            }
            
        }
        private async Task MenuSelector(Client thisClient, Executor thisExecutor, Update update)
        {
            if (thisExecutor != null && thisClient == null)
            {
                if (update.CallbackQuery != null)
                {
                    await _executorMenu.ProcessCallBack(thisExecutor, update);
                } 
                else if (update.Message.ReplyToMessage != null)
                {
                    await _executorMenu.ProcessReply(thisExecutor, update);
                }
                else
                {
                    await _executorMenu.StartMenu(thisExecutor, update);
                }
            } 
            else if (thisExecutor == null && thisClient != null)
            {
                if (update.CallbackQuery != null)
                {
                    
                } 
                else if (update.Message.ReplyToMessage != null)
                {
                
                }
                else
                {
                    //todo: client menu
                    //await _clientMenu.ClientMainMenu();
                }
            }
            else
            {
                await _registrationService.StartRegistration(update);
            }
        }
        
    }
}
