using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DigitalAgency.Bll.Services.Bot.Interfaces;
using DigitalAgency.Dal.Entities;
using DigitalAgency.Dal.Entities.Enums;
using DigitalAgency.Dal.Storages.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace DigitalAgency.Bll.Services.Bot
{
    public class ExecutorMenu : IExecutorMenu
    {
        private readonly IExecutorStorage _executorStorage;
        private readonly ITelegramBotClient _telegram;
        public ExecutorMenu(ITelegramBotClient telegram, IExecutorStorage executorStorage)
        {
            _telegram = telegram;
            _executorStorage = executorStorage;
        }

        public async Task StartMenu(Executor executor, Update update)
        {
            if (executor.Position == PositionsEnum.Unknown)
            {
                await EditPositionMenu(executor, update);
                return;
            }
        }

        //todo: implement
        public async Task ProcessCallBack(Executor executor, Update update)
        {
            throw new NotImplementedException();
        }
        
        public async Task ProcessReply(Executor executor, Update update)
        {
            throw new NotImplementedException();
        }

        private async Task EditPositionMenu(Executor executor, Update update)
        {
            if (update.Message.Text == null)
            {
                await _telegram.SendTextMessageAsync(update.Message.Chat, "Wrong input!");
                return;
            }
            
            if (!Enum.TryParse<PositionsEnum>(update.Message.Text, out var parsedPosition))
            {
                var keys = Enum.GetNames(typeof(PositionsEnum)).ToList();
            
                var keyboard = KeyboardMessages.DefaultKeyboardMessage(keys);

                await _telegram.SendTextMessageAsync(update.Message.Chat, "Please choose your position",
                    replyMarkup: keyboard);
            }
            else
            {
                await EditExecutorPosition(update.Message);
            }
        }
        
        private async Task<bool> EditExecutorPosition(Message message)
        {
            var thisExecutor = await _executorStorage.GetExecutorAsync(x => x.TelegramId == message.From.Id);

            if (!Enum.TryParse<PositionsEnum>(message.Text, out var parsedPosition)) return false;
            thisExecutor.Position = parsedPosition;
            await _executorStorage.UpdateExecutorAsync(thisExecutor);
            await _telegram.SendTextMessageAsync(message.Chat, $"Success! Your position is {parsedPosition}");
            return true;

        }
    }
}