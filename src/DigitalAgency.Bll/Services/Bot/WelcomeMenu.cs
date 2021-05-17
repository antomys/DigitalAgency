using DigitalAgency.Bll.Services.Bot.Interfaces;
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
                case "yes":
                {
                    await _clientStorage.DeleteClientAsync(thisClient.Id);
                    await _executorStorage.CreateExecutorAsync(new Executor
                    {
                        FirstName = thisClient.FirstName,
                        MiddleName = thisClient.MiddleName,
                        LastName = thisClient.LastName,
                        PhoneNumber = thisClient.PhoneNumber,
                        TelegramId = thisClient.TelegramId,
                        ChatId = thisClient.ChatId,
                    });
                    await _telegram.SendTextMessageAsync(receivedMessage.Chat.Id,
                        $"One more step for your registration!");
                    return;
                }
                case "no":
                { 
                    await _telegram.SendTextMessageAsync(receivedMessage.Chat.Id,
                            $"You have successfully registered as Client!, " +
                            $"{receivedMessage.From.FirstName}!",
                            replyMarkup: KeyboardMessages.DefaultKeyboardMessage(_botConfiguration.CommandsClient));
                    
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
        private async Task GetContactPhone(long chatId)
        {
            var keyboard = new ReplyKeyboardMarkup(new[]
            {
                new[] {new KeyboardButton("Share your contact") {RequestContact = true}}
            }, resizeKeyboard: true, oneTimeKeyboard: true);
            
            await _telegram.SendTextMessageAsync(chatId,
                "Welcome to our bot!\n" +
                "Before you can start use ours bot, you need to setup your account\n\n" +
                "First, enter your phone number", replyMarkup: keyboard);
        }
    }
}