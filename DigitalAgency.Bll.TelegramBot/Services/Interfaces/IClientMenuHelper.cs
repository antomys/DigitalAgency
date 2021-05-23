using System.Collections.Generic;
using System.Threading.Tasks;
using DigitalAgency.Bll.TelegramBot.Models;
using DigitalAgency.Dal.Entities;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace DigitalAgency.Bll.TelegramBot.Services.Interfaces
{
    public interface IClientMenuHelper
    {
        Task<List<BotShortOrderModel>> ViewClientOrders(Client thisClient);
        Task<List<Project>> ViewClientProjects(Client thisClient);
        Task<InlineKeyboardMarkup> ConstructClientOrderButtons(Update update, Order thisOrder);
        Task<Executor> GetNullExecutor();
    }
}