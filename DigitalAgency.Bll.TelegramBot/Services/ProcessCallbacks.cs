/*using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using AutoMapper;
using DigitalAgency.Bll.Models;
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
    public class ProcessCallbacks : IProcessCallbacks
    {
        private readonly IClientStorage _clientStorage;
        private readonly IOrderStorage _orderStorage;
        private readonly ITelegramBotClient _telegram;
        private readonly IProjectStorage _projectStorage;
        private readonly BotConfiguration _botConfiguration;
        public ProcessCallbacks(
            ITelegramBotClient telegram, 
            IOrderStorage orderStorage, 
            IClientStorage clientStorage, 
            IOptions<BotConfiguration> botConfiguration, 
            IProjectStorage project)
        {
            _telegram = telegram;
            _orderStorage = orderStorage;
            _clientStorage = clientStorage;
            _botConfiguration = botConfiguration.Value;
            _projectStorage = project;
        }
        //Only for Client
        public async Task ProcessCreateOrderCallback(Update update)
        {
            var typeAndId = update.CallbackQuery.Data.Split(';');
            var id = int.Parse(typeAndId[1]);
            var mapper = new Mapper(MappingConfig.GetMapper());
            var thisClient = await _clientStorage.GetClientAsync(client => client.TelegramId == update.CallbackQuery.From.Id);

            var backKeyboard = KeyboardMessages.DefaultKeyboardMessage(new[] {"Back"});
            var chatId = update.CallbackQuery.Message.Chat.Id;
            var type = typeAndId[0].ToLower();
            switch (type)
            {
                case "please choose a project to order":
                {
                    var newOrder = new Order{ClientId = thisClient.Id, ProjectId = id, CreationDate = DateTime.Now, StateEnum = "New"};
                    await _orderStorage.CreateOrderAsync(newOrder);

                    await _telegram.SendTextMessageAsync(chatId, "Please enter scheduled time for order",replyMarkup:new ForceReplyMarkup());
                    await _telegram.SendTextMessageAsync(chatId, "Format: yyyy-mm-dd");
                    return;
                }
                case "edit order":
                {
                    await _telegram.SendTextMessageAsync(chatId, "Please select what to change");
                    var dict = new ConcurrentDictionary<string, string>();

                    var thisOrder = await _orderStorage.GetOrderAsync(id);

                    var mapped = mapper.Map<ServiceOrderModel>(thisOrder);
                    mapped.Mechanic = new Client();
                    mapped.Mechanic = await _clientStorage.GetClientByIdAsync(thisOrder.ExecutorId);
                    
                    var excluded = new List<string>{"Schedule Date","State", "Delete"};
                    foreach (var property in excluded)
                    {
                        dict.TryAdd(property, "edit order " + property+";"+id);
                    }

                    var keys = KeyboardMessages.DefaultInlineKeyboardMessage(dict);

                    await _telegram.SendTextMessageAsync(chatId, mapped.ToString(), replyMarkup: keys);
                    return;
                }
                case "edit order schedule date":
                {
                    var thisOrder = await _orderStorage.GetOrderAsync(id);

                    var mapped = mapper.Map<ServiceOrderModel>(thisOrder);
                    mapped.Mechanic = new Client();
                    mapped.Mechanic = await _clientStorage.GetClientByIdAsync(thisOrder.ExecutorId);
                    await _telegram.SendTextMessageAsync(chatId, $"[{mapped.Id}]\n"+mapped, replyMarkup:new ForceReplyMarkup());
                    await _telegram.SendTextMessageAsync(chatId, "Please enter new schedule date");
                    await _telegram.SendTextMessageAsync(chatId, "Remember, Format: yyyy-mm-dd");
                    
                    return;
                }
                case "edit order state":
                {
                    var thisOrder = await _orderStorage.GetOrderAsync(id);

                    var mapped = mapper.Map<ServiceOrderModel>(thisOrder);
                    mapped.Mechanic = new Client();
                    mapped.Mechanic = await _clientStorage.GetClientByIdAsync(thisOrder.ExecutorId);
                    await _telegram.SendTextMessageAsync(chatId, $"[{mapped.Id}]\n"+mapped, replyMarkup:new ForceReplyMarkup());
                    await _telegram.SendTextMessageAsync(chatId, "Please enter new state");
                    return;
                }
                case "edit order delete":
                {
                    await _orderStorage.DeleteOrderAsync(id);
                    await _telegram.SendTextMessageAsync(chatId, "Successfully deleted!", replyMarkup: KeyboardMessages.DefaultKeyboardMessage(_botConfiguration.CommandsMechanic));
                    return;
                }
            }
        } 
        
        public async Task ProcessCallback(Update update)
        {
            var mapper = new Mapper(MappingConfig.GetMapper());
            var typeAndId = update.CallbackQuery.Data.Split(';');
            
            var id = int.Parse(typeAndId[1]);
            var thisClient = await _clientStorage.GetClientAsync(client => client.TelegramId == update.CallbackQuery.From.Id);
            
            var chatId = update.CallbackQuery.Message.Chat.Id;
            var backKeyboard = KeyboardMessages.DefaultKeyboardMessage(new[] {"Back"});
            
            var type = typeAndId[0].ToLower();

            if (type.Contains("ordercallback"))
            {
                var serviceOrder = await _orderStorage.GetOrderAsync(id);
                var mapped = mapper.Map<ServiceOrderModel>(serviceOrder);
                mapped.Mechanic = new Client();
                mapped.Mechanic = await _clientStorage.GetClientByIdAsync(serviceOrder.ExecutorId);
                
                var dict = new ConcurrentDictionary<string, string>();
                dict.TryAdd("Edit", $"edit order;{serviceOrder.Id}");
                    
                var keyboardButtons = KeyboardMessages.DefaultInlineKeyboardMessage(dict);
                await _telegram.SendTextMessageAsync(chatId,mapped.ToString(),replyMarkup: keyboardButtons );
                await _telegram.SendTextMessageAsync(chatId,$"To go back, press button below",replyMarkup: backKeyboard);
            }
            else if (type.Contains("clientcallback"))
            {
                var client = await _clientStorage.GetClientByIdAsync(id);
                var mapped = mapper.Map<ClientModel>(client);
                await _telegram.SendTextMessageAsync(chatId,mapped.ToString(),replyMarkup:backKeyboard);
            } 
            else if (type.Contains("projectcallback"))
            {
               var car = await _projectStorage.GetProjectByIdAsync(id);
               var mapped = mapper.Map<CarModel>(car);
               await _telegram.SendTextMessageAsync(chatId,mapped.ToString(),replyMarkup:backKeyboard);
            }
        }
    }
}*/