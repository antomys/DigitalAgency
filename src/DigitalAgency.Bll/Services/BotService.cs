/*
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
        private readonly IProcessCallbacks _processCallbacks;
        private readonly IProcessReply _processReply;
        private readonly IExecutorStorage _executorStorage;


        public BotService(
            ITelegramBotClient telegram,
            IClientStorage clientStorage,
            IProcessCallbacks processCallbacks, 
            IProcessReply processReply, 
            IExecutorStorage executorStorage)
        {
            _telegram = telegram;
            _clientStorage = clientStorage;
            _processCallbacks = processCallbacks;
            _processReply = processReply;
            _executorStorage = executorStorage;
        }
        public async Task ProcessMessageAsync(Update update)
        {
            if (update.Type is UpdateType.Unknown)
                return;
            var thisUser =
                await _clientStorage.GetClientAsync(client => client.TelegramId == update.CallbackQuery.From.Id);
            var thisExecutor =
                await _executorStorage.GetExecutorAsync(client => client.TelegramId == update.CallbackQuery.From.Id);
            
            if (update.CallbackQuery is not null)
            {
                if (thisUser != null)
                {
                    if (update.CallbackQuery.Message.Text.ToLower().Contains("order") 
                        && !update.CallbackQuery.Message.Text.ToLower().Contains("orders"))
                    {
                        await _processCallbacks.ProcessCreateOrderCallback(update);
                    }
                    else
                    {
                        await _processCallbacks.ProcessCallback(update);
                    }
                    return;
                } 
                if (thisExecutor != null)
                {
                    
                }
                
            }
            var receivedMessage = update.Message;
            var sender = receivedMessage.From;
            
            var thisClient = await _clientStorage.GetClientAsync(client => client.TelegramId == sender.Id);
            
            if (receivedMessage.ReplyToMessage is not null && !string.IsNullOrEmpty(receivedMessage.Text))
            {
                await _processReply.ProcessClientReplies(receivedMessage,thisClient, update);
                return;
            }
            if (receivedMessage.Type == MessageType.Contact && receivedMessage.Contact != null && thisClient == null)
            {
                await _telegram.SendTextMessageAsync(
                    receivedMessage.Chat.Id, $"Thank you, {sender.FirstName}!\n" + 
                                             $"Your phone number is {receivedMessage.Contact.PhoneNumber}");
                if (await _clientStorage.CreateNewClient(receivedMessage))
                {
                    await EditClient(receivedMessage.Chat.Id);
                }
            }
            if (thisClient != null && (string.IsNullOrEmpty(receivedMessage.Text) || receivedMessage.Text.Equals("/start")))
            {
                await _telegram.SendTextMessageAsync(receivedMessage.Chat.Id, $"Welcome back,{thisClient.FirstName}!");
                receivedMessage.Text = "back";
            }
        }
        private async Task EditClient(long chatId)
        { 
            var keyboard = KeyboardMessages.DefaultKeyboardMessage(new List<string> {"Yes", "No"});
            await _telegram.SendTextMessageAsync(chatId, "Are you an executor?", replyMarkup: keyboard);
        }
    }
}
*/
