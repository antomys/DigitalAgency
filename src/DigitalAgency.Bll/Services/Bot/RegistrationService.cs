using System;
using System.Threading.Tasks;
using DigitalAgency.Bll.Services.Bot.Interfaces;
using DigitalAgency.Dal.Entities;
using DigitalAgency.Dal.Entities.Enums;
using DigitalAgency.Dal.Storages.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace DigitalAgency.Bll.Services.Bot
{
    public class RegistrationService : IRegistrationService
    {
        private readonly IClientStorage _clientStorage;
        private readonly IExecutorStorage _executorStorage;
        private readonly ITelegramBotClient _telegram;

        public RegistrationService(
            IClientStorage clientStorage,
            IExecutorStorage executorStorage, 
            ITelegramBotClient telegram)
        {
            _clientStorage = clientStorage;
            _executorStorage = executorStorage;
            _telegram = telegram;
        }
        
        public async Task StartRegistration(Update update)
        {
            var receivedMessage = update.Message;
            
            if (update.Message.Contact != null)
            {
                await WelcomeMessage(update.Message);
                return;
            }
            if(update.Message.Contact == null && receivedMessage == null 
               || !receivedMessage.Text.Contains("register",StringComparison.OrdinalIgnoreCase))
            {
                await GetContactPhone(update.Message.Chat);
                return;
            }
                
            switch (receivedMessage.Text.ToLower())
            {
                case "register as client":
                {
                    if (await CreateBotClient(receivedMessage))
                    {
                        await _telegram.SendTextMessageAsync(receivedMessage.Chat, "Successfully Signed up!");
                        //todo: not return but redirect to client menu
                        return;
                    }
                    await _telegram.SendTextMessageAsync(receivedMessage.Chat, "Something went wrong. Try again!");
                    await WelcomeMessage(receivedMessage);
                    return;
                }
                case "register as executor":
                {
                    if (await CreateBotExecutor(receivedMessage))
                    {
                        await _telegram.SendTextMessageAsync(receivedMessage.Chat, $"Successfully Signed up!");
                        //todo: not return but redirect to executor menu
                        return;
                    }
                    await _telegram.SendTextMessageAsync(receivedMessage.Chat, $"Something went wrong. Try again!");
                    await WelcomeMessage(receivedMessage);
                    return;
                }
                default:
                {
                    await _telegram.SendTextMessageAsync(update.Message.Chat.Id,"Wrong command! Try again:)");
                    await WelcomeMessage(update.Message);
                    return;
                }
            }
        }
        private async Task WelcomeMessage(Message chat)
        {
            await _clientStorage.CreateClientAsync(new Client
                {
                    TelegramId = chat.Contact.UserId, 
                    PhoneNumber = GetPhoneNumber(chat)
                    
                });
            
            const string regAs = "Register as ";
            var keyboard = KeyboardMessages.DefaultKeyboardMessage(new[] {regAs + "Client", regAs + "Executor"});

            await _telegram.SendTextMessageAsync(chat.Chat,
                $"Now please choose who you want to be: ", replyMarkup: keyboard);
        }
        private async Task GetContactPhone(Chat chat)
        {
            var keyboard = new ReplyKeyboardMarkup(new[]
            {
                new[] {new KeyboardButton("Share your contact") {RequestContact = true}}
            }, resizeKeyboard: true, oneTimeKeyboard: true);
            await _telegram.SendTextMessageAsync(chat,
                "Welcome to the Digital Agency bot! Looks like you are new here!\n" +
                $"Let's sign you up.\nPlease share your phone.\nIt need to complete registration", replyMarkup: keyboard);
        }
        private static string GetPhoneNumber(Message message)
        {
            return message.Contact.PhoneNumber.Contains('+') ? message.Contact.PhoneNumber : message.Contact.PhoneNumber.Insert(0, "+");
        }
        private async Task<bool> CreateBotClient(Message message)
        {
            var contactDetails = message.From;
            var thisContact = await _clientStorage.GetClientAsync(x => x.TelegramId == contactDetails.Id);
            
            if (thisContact == null) { return false; }
            
            var contact = new Client
            {
                FirstName = contactDetails.FirstName,
                MiddleName = contactDetails.Username,
                LastName = contactDetails.LastName,
                PhoneNumber = thisContact.PhoneNumber,
                TelegramId = thisContact.TelegramId,
                ChatId = message.Chat.Id,
            };
            await _clientStorage.UpdateClientAsync(contact);
            return true;
        }
        private async Task<bool> CreateBotExecutor(Message message)
        {
            var contactDetails = message.From;
            var clientToExecutor = await _clientStorage.GetClientAsync(x => x.TelegramId == contactDetails.Id);
            
            if (clientToExecutor == null) return false;

            var executor = new Executor
            {
                FirstName = contactDetails.FirstName,
                MiddleName = contactDetails.Username,
                LastName = contactDetails.LastName,
                PhoneNumber = clientToExecutor.PhoneNumber,
                Position = PositionsEnum.Unknown,
                TelegramId = clientToExecutor.TelegramId,
                ChatId = message.Chat.Id
            };
            await _clientStorage.DeleteClientAsync(clientToExecutor.Id);
            await _executorStorage.CreateExecutorAsync(executor);
            return true;
        }
    }
}