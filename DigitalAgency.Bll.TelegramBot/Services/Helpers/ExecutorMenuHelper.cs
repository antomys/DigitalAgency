using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using DigitalAgency.Bll.TelegramBot.Models;
using DigitalAgency.Bll.TelegramBot.Services.Interfaces;
using DigitalAgency.Dal.Entities;
using DigitalAgency.Dal.Entities.Enums;
using DigitalAgency.Dal.Storages.Interfaces;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace DigitalAgency.Bll.TelegramBot.Services.Helpers;

public class ExecutorMenuHelper : IExecutorMenuHelper
{
    readonly IExecutorStorage _executorStorage;
    readonly IMapper _mapper;
    readonly IOrderStorage _orderStorage;

    public ExecutorMenuHelper(IOrderStorage orderStorage, IExecutorStorage executorStorage, IMapper mapper)
    {
        _orderStorage = orderStorage;
        _executorStorage = executorStorage;
        _mapper = mapper;
    }

    public async Task<List<BotShortOrderModel>> ViewMyOrders(Executor executor)
    {
        List<Order> executorOrders = await _orderStorage.GetOrdersAsync(x => x.ExecutorId == executor.Id);

        List<BotShortOrderModel> mapped = _mapper.Map<List<Order>, List<BotShortOrderModel>>(executorOrders);

        return mapped;
    }

    public async Task<List<BotShortOrderModel>> ViewFreeOrders(Executor executor)
    {
        List<Order> executorOrders = await _orderStorage
            .GetOrdersAsync(x => x.Executor.PhoneNumber == "NULL"
                                 && x.ExecutorPosition == executor.Position);

        List<BotShortOrderModel> mapped = _mapper.Map<List<Order>, List<BotShortOrderModel>>(executorOrders);

        return mapped;
    }

    public Task<InlineKeyboardMarkup> ConstructStatesButtons(Update update, Order thisOrder)
    {
        ConcurrentDictionary<string, string> dict = new ConcurrentDictionary<string, string>();
        foreach (string name in Enum.GetNames(typeof(OrderStateEnum)))
        {
            dict.TryAdd(name, $"{name}:{thisOrder.Id}");
        }

        return Task.FromResult(KeyboardMessages.DefaultInlineKeyboardMessage(dict));
    }

    public Task<InlineKeyboardMarkup> ConstructConfirmOrderButtons(Order thisOrder)
    {
        ConcurrentDictionary<string, string> dict = new ConcurrentDictionary<string, string>();
        dict.TryAdd("Yes", $"yes:{thisOrder.Id}");
        dict.TryAdd("No", $"no:{thisOrder.Id}");
        InlineKeyboardMarkup keys = KeyboardMessages.DefaultInlineKeyboardMessage(dict);

        return Task.FromResult(keys);
    }
}