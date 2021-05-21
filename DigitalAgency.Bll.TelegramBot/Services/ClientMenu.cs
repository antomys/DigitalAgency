using System.Collections.Generic;
using DigitalAgency.Bll.TelegramBot.Services.Interfaces;
using DigitalAgency.Dal.Entities;
using DigitalAgency.Dal.Storages.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using Task = System.Threading.Tasks.Task;

namespace DigitalAgency.Bll.TelegramBot.Services
{
    public class ClientMenu : IClientMenu
    {
        private readonly ITelegramBotClient _telegram;
        private readonly IClientStorage _clientStorage;

        public ClientMenu(
            ITelegramBotClient telegram, 
            IClientStorage clientStorage)
        {
            _telegram = telegram;
            _clientStorage = clientStorage;
        }
        public async Task ClientMainMenu(Client thisClient, Message receivedMessage)
        {
            
        }
        private async Task ViewOrders(List<Order> orders, Message receivedMessage)
        {
            
        }
        private async Task ViewProjects(Client thisClient, Message receivedMessage)
        {
            
        }
    }
}