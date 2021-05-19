using System;
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

        private async Task EditPositionMenu(Executor executor, Update update)
        {
            var receivedMessage = update.Message;

            if (!Enum.IsDefined(typeof(PositionsEnum), receivedMessage.Text))
            {
                await _telegram.SendTextMessageAsync(receivedMessage.Chat, "Wrong input. Try again!");
                return;
            }
            else
            {
                executor.Position = PositionsEnum.Copywriter;
                await _executorStorage.UpdateExecutorAsync(executor);
                await _telegram.SendTextMessageAsync(receivedMessage.Chat, $"Success! Your position\nis{executor.Position}");
                return;
            }
        }
        
        private async Task<bool> EditExecutorPosition(Message message)
        {
            var thisExecutor = await _executorStorage.GetExecutorAsync(x => x.TelegramId == message.From.Id);

            if (!Enum.TryParse<PositionsEnum>(message.Text, out var parsedPosition)) return false;
            thisExecutor.Position = parsedPosition;
            await _executorStorage.UpdateExecutorAsync(thisExecutor);
            return true;

        }
    }
}