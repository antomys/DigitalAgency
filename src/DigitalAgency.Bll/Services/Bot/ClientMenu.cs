using System.Collections.Generic;
using System.Linq;
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
    public class ClientMenu : IClientMenu
    {
        private readonly ITelegramBotClient _telegram;
        private readonly IButtons _buttons;

        public ClientMenu(
            ITelegramBotClient telegram,
            IButtons buttons)
        {
            _telegram = telegram;
            _buttons = buttons;
        }
        public async Task ClientMainMenu(Client thisClient, Message receivedMessage)
        {
        }
        private async Task ViewOrders(List<Order> orders, Message receivedMessage)
        {
            if (!orders.Any())
            {
                await _telegram.SendTextMessageAsync(receivedMessage.Chat.Id,
                    $"You have no active orders. Try to create one");
                return;
            }
            await _buttons.ViewOrderButtons(orders,receivedMessage.Chat.Id);
        }
        private async Task ViewProjects(Client thisClient, Message receivedMessage)
        {
        }
    }
}