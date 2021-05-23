using System.Collections.Generic;
using System.Threading.Tasks;
using DigitalAgency.Bll.TelegramBot.Models;
using DigitalAgency.Dal.Entities;
using Telegram.Bot.Types;

namespace DigitalAgency.Bll.TelegramBot.Services.Interfaces
{
    public interface IButtons
    {
        Task ViewOrderButtons(IEnumerable<BotShortOrderModel> orders, Chat chat,
            string messageString = "Orders", string orderString = "");
        Task ViewProjectButtons(IEnumerable<Project> projects, Chat chat, string messageString);
    }
}