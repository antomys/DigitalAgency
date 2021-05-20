using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DigitalAgency.Bll.TelegramBot.Services.Interfaces;
using DigitalAgency.Dal.Entities;
using DigitalAgency.Dal.Entities.Enums;
using DigitalAgency.Dal.Storages.Interfaces;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace DigitalAgency.Bll.TelegramBot.Services
{
    public class ExecutorMenu : IExecutorMenu
    {
        private readonly IExecutorStorage _executorStorage;
        private readonly ITelegramBotClient _telegram;
        private readonly IEnumerable<string> _executorCommands;
        public ExecutorMenu(ITelegramBotClient telegram, 
            IExecutorStorage executorStorage, 
            IOptions<BotConfiguration> executorCommands)
        {
            _telegram = telegram;
            _executorStorage = executorStorage;
            _executorCommands = executorCommands.Value.ExecutorMenu;
        }

        public async Task StartMenu(Executor executor, Update update)
        {
            var keyboard = KeyboardMessages.DefaultKeyboardMessage(_executorCommands);
            
            if (executor.Position == PositionsEnum.Unknown)
            {
                if(await EditPositionMenu(executor, update) == false)
                    return;
                
                await _telegram.SendTextMessageAsync(update.Message.Chat,$"Welcome, {executor.FirstName}!", replyMarkup: keyboard);
                return;
            }

            if (!_executorCommands.Contains(update.Message.Text, StringComparer.OrdinalIgnoreCase))
            {
                await _telegram.SendTextMessageAsync(update.Message.Chat,$"Wrong command", replyMarkup: keyboard);
            }
            
            switch (update.Message.Text)
            {
                case "View my orders":
                {
                    break;
                }
                case "Edit my position":
                {
                    break;
                }
                case "View available orders":
                {
                    break;
                }
                default:
                {
                    break;
                }
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
        private async Task<bool> EditPositionMenu(Executor executor, Update update)
        {
            if (update.Message.Text == null)
            {
                await _telegram.SendTextMessageAsync(update.Message.Chat, "Wrong input!");
                return false;
            }
            
            if (!Enum.TryParse<PositionsEnum>(update.Message.Text, out var parsedPosition))
            {
                var keys = Enum.GetNames(typeof(PositionsEnum))
                    .Where(x=> x != Enum.GetName(PositionsEnum.Unknown)).ToList();

                var keyboard = KeyboardMessages.DefaultKeyboardMessage(keys);

                await _telegram.SendTextMessageAsync(update.Message.Chat, "Please choose your position",
                    replyMarkup: keyboard);
            }
            else
            {
                return await EditExecutorPosition(update.Message);
            }

            return false;
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