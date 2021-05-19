/*using DigitalAgency.Bll.Services.Bot.Interfaces;
using DigitalAgency.Bll.Services.Interfaces;
using DigitalAgency.Dal.Entities;
using DigitalAgency.Dal.Storages.Interfaces;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using Task = System.Threading.Tasks.Task;

namespace DigitalAgency.Bll.Services.Bot
{
    public class WelcomeMenu : IWelcomeMenu
    {
        private readonly ITelegramBotClient _telegram;
        private readonly IClientStorage _clientStorage;
        private readonly IExecutorStorage _executorStorage;
        private readonly BotConfiguration _botConfiguration;
        private readonly IClientMenu _clientMenu;

        public WelcomeMenu(
            ITelegramBotClient telegram, IClientStorage clientStorage, 
            IExecutorStorage executorStorage, 
            IOptions<BotConfiguration> botConfiguration, 
            IClientMenu clientMenu)
        {
            _telegram = telegram;
            _clientStorage = clientStorage;
            _executorStorage = executorStorage;
            _clientMenu = clientMenu;
            _botConfiguration = botConfiguration.Value;
        }
        public async Task StartMenu(Client thisClient, Message receivedMessage) 
        {
            switch (receivedMessage.Text?.ToLower()) 
            { 
                case "/start": 
                { 
                    await GetContactPhone(receivedMessage.Chat.Id); 
                    return; 
                }
                case "back":
                {
                    await _telegram.SendTextMessageAsync(receivedMessage.Chat.Id, "Please choose from menu");
                    await _clientMenu.ClientMainMenu(thisClient, receivedMessage);
                    return;
                }
                default:
                {
                    await _clientMenu.ClientMainMenu(thisClient, receivedMessage);
                    return;
                }
            }
        }
        
    }
}*/