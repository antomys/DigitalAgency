using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DigitalAgency.Bll.Services.Bot;
using DigitalAgency.Bll.Services.Bot.Interfaces;
using DigitalAgency.Bll.Services.Interfaces;
using DigitalAgency.Dal.Storages.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace DigitalAgency.Bll.Services
{
    public class BotService : IBotService
    {
        private readonly ITelegramBotClient _telegram;
        private readonly IClientStorage _clientStorage;
        private readonly IExecutorStorage _executorStorage;
        private readonly IRegistrationService _registrationService;


        public BotService(
            ITelegramBotClient telegram,
            IClientStorage clientStorage,
            IExecutorStorage executorStorage, 
            IRegistrationService registrationService)
        {
            _telegram = telegram;
            _clientStorage = clientStorage;
            _executorStorage = executorStorage;
            _registrationService = registrationService;
        }
        public async Task ProcessMessageAsync(Update update)
        {
            if (update.Type is UpdateType.Unknown)
                return;
            var thisUser =
                await _clientStorage.GetClientAsync(client => client.TelegramId == update.Message.From.Id);
            var thisExecutor =
                await _executorStorage.GetExecutorAsync(client => client.TelegramId == update.Message.From.Id);
            // todo; To explicit method callback execution
            if (update.CallbackQuery is not null)
            {
                if (thisUser != null)
                {
                    
                }

                if (thisExecutor != null)
                {
                    
                }

                throw new ArgumentOutOfRangeException($"No user or executor in callback! [LINE 63 BOTSERVICE]");
            }

            var receivedMessage = update.Message;
            
            if (receivedMessage == null)
            {
                throw new ArgumentNullException($"{nameof(receivedMessage)} is null! [if (receivedMessage == null) BOTSERVICE]");
            }
            
            // todo: To explicit registration
            if (thisExecutor == null || thisUser == null)
            {
                await _registrationService.StartRegistration(update);
            }
            // Todo: Process reply
            if (receivedMessage.ReplyToMessage != null)
            {
                
            }

            if (thisExecutor != null && thisUser == null)
            {
                
            }
            if (thisExecutor == null && thisUser != null)
            {
                
            }
        }

        
        
    }
}
