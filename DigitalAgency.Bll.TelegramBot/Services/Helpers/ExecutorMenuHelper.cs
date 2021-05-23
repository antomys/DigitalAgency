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

namespace DigitalAgency.Bll.TelegramBot.Services.Helpers
{
    public class ExecutorMenuHelper : IExecutorMenuHelper
    {
        private readonly IExecutorStorage _executorStorage;
        private readonly IOrderStorage _orderStorage;
        private readonly IMapper _mapper;

        public ExecutorMenuHelper(IOrderStorage orderStorage, IExecutorStorage executorStorage, IMapper mapper)
        {
            _orderStorage = orderStorage;
            _executorStorage = executorStorage;
            _mapper = mapper;
        }

        public async Task<List<BotShortOrderModel>> ViewMyOrders(Executor executor)
        {
            var executorOrders = await _orderStorage.GetOrdersAsync(x => x.ExecutorId == executor.Id);
            
            var mapped = _mapper.Map<List<Order>,List<BotShortOrderModel>>(executorOrders);

            return mapped;
        }

        public async Task<List<BotShortOrderModel>> ViewFreeOrders(Executor executor)
        {
            var executorOrders = await _orderStorage.GetOrdersAsync(x => x.Executor.PhoneNumber == "NULL");
            
            var mapped = _mapper.Map<List<Order>,List<BotShortOrderModel>>(executorOrders);

            return mapped;
        }
        
        public Task<InlineKeyboardMarkup> ConstructStatesButtons(Update update, Order thisOrder)
        {
            var dict = new ConcurrentDictionary<string, string>();
            foreach (var name in Enum.GetNames(typeof(OrderStateEnum)))
            {
                dict.TryAdd(name, $"{name}:{thisOrder.Id}");
            }

            return Task.FromResult(KeyboardMessages.DefaultInlineKeyboardMessage(dict));
        }

        public Task<InlineKeyboardMarkup> ConstructConfirmOrderButtons(Order thisOrder)
        {
            var dict = new ConcurrentDictionary<string, string>();
            dict.TryAdd("Yes", $"yes:{thisOrder.Id}");
            dict.TryAdd("No", $"no:{thisOrder.Id}");
            var keys = KeyboardMessages.DefaultInlineKeyboardMessage(dict);

            return Task.FromResult(keys);
        }
    }
}