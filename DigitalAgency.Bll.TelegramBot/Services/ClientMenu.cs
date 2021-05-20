using System.Collections.Generic;
using System.Linq;
using DigitalAgency.Bll.TelegramBot.Services.Interfaces;
using DigitalAgency.Dal.Entities;
using Telegram.Bot;
using Telegram.Bot.Types;
using Task = System.Threading.Tasks.Task;

namespace DigitalAgency.Bll.TelegramBot.Services
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