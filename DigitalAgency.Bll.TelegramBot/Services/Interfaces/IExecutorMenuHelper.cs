using System.Collections.Generic;
using System.Threading.Tasks;
using DigitalAgency.Bll.TelegramBot.Models;
using DigitalAgency.Dal.Entities;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace DigitalAgency.Bll.TelegramBot.Services.Interfaces;

public interface IExecutorMenuHelper
{
    Task<List<BotShortOrderModel>> ViewMyOrders(Executor executor);
    Task<List<BotShortOrderModel>> ViewFreeOrders(Executor executor);
    Task<InlineKeyboardMarkup> ConstructConfirmOrderButtons(Order thisOrder);
    Task<InlineKeyboardMarkup> ConstructStatesButtons(Update update, Order thisOrder);
}