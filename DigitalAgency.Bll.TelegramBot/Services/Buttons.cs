using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using DigitalAgency.Bll.TelegramBot.Models;
using DigitalAgency.Bll.TelegramBot.Services.Interfaces;
using DigitalAgency.Dal.Entities;
using DigitalAgency.Dal.Storages.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using Task = System.Threading.Tasks.Task;

namespace DigitalAgency.Bll.TelegramBot.Services;

public class Buttons : IButtons
{
    readonly IExecutorStorage _executorStorage;
    readonly ITelegramBotClient _telegram;

    public Buttons(IExecutorStorage executorStorage, ITelegramBotClient telegram)
    {
        _executorStorage = executorStorage;
        _telegram = telegram;
    }

    public async Task ViewOrderButtons(IEnumerable<BotShortOrderModel> orders, Chat chat, string messageString = "Orders",
        string orderString = "")
    {
        ConcurrentDictionary<string, string> keyOrders = new ConcurrentDictionary<string, string>();
        foreach (BotShortOrderModel order in orders)
        {
            if (string.IsNullOrEmpty(orderString))
            {
                keyOrders.TryAdd("Project: " + order.ProjectName + "\nTasks: " + order.TasksCount, order.Id.ToString());
            }
            else
            {
                keyOrders.TryAdd("Project: " + order.ProjectName + "\nState: " + Enum.GetName(order.StateEnum), orderString + ':' + order.Id);
            }
        }

        InlineKeyboardMarkup keyboard = KeyboardMessages.DefaultInlineKeyboardMessage(keyOrders);

        await _telegram.SendTextMessageAsync(chat, messageString, replyMarkup: keyboard);
    }


    public async Task ViewProjectButtons(IEnumerable<Project> projects, Chat chat, string messageString)
    {
        ConcurrentDictionary<string, string> keyOrders = new ConcurrentDictionary<string, string>();
        foreach (Project project in projects)
        {
            keyOrders.TryAdd(project.ProjectName, "project:" + project.Id);
        }
        InlineKeyboardMarkup keyboard = KeyboardMessages.DefaultInlineKeyboardMessage(keyOrders);

        await _telegram.SendTextMessageAsync(chat, messageString, replyMarkup: keyboard);
    }
}