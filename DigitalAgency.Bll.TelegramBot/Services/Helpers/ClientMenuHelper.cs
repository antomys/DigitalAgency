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

public class ClientMenuHelper : IClientMenuHelper
{
    readonly IExecutorStorage _executorStorage;
    readonly IMapper _mapper;
    readonly IOrderStorage _orderStorage;
    readonly IProjectStorage _projectStorage;

    public ClientMenuHelper(IOrderStorage orderStorage,
        IProjectStorage projectStorage, IMapper mapper, IExecutorStorage executorStorage)
    {
        _orderStorage = orderStorage;
        _projectStorage = projectStorage;
        _mapper = mapper;
        _executorStorage = executorStorage;
    }

    public async Task<List<BotShortOrderModel>> ViewClientOrders(Client thisClient)
    {
        List<Order> executorOrders = await _orderStorage.GetOrdersAsync(x => x.ClientId == thisClient.Id);

        List<BotShortOrderModel> mapped = _mapper.Map<List<Order>, List<BotShortOrderModel>>(executorOrders);

        return mapped;
    }

    public async Task<List<Project>> ViewClientProjects(Client thisClient)
    {
        List<Project> executorOrders = await _projectStorage.GetProjectsAsync(x => x.OwnerId == thisClient.Id);

        return executorOrders;
    }

    public async Task<Executor> GetNullExecutor()
    {
        return await _executorStorage.GetExecutorAsync(x => x.FirstName == "NULL");
    }

    public Task<InlineKeyboardMarkup> ConstructClientOrderButtons(Update update, Order thisOrder)
    {
        ConcurrentDictionary<string, string> dict = new ConcurrentDictionary<string, string>();
        foreach (string name in Enum.GetNames(typeof(OrderStateEnum)))
        {
            dict.TryAdd(name, $"{name}:{thisOrder.Id}");
        }

        return Task.FromResult(KeyboardMessages.DefaultInlineKeyboardMessage(dict));
    }
}