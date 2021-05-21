using System.Collections.Concurrent;
using System.Collections.Generic;
using DigitalAgency.Bll.TelegramBot.Models;
using DigitalAgency.Bll.TelegramBot.Services.Interfaces;
using DigitalAgency.Dal.Entities;
using DigitalAgency.Dal.Storages.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using Task = System.Threading.Tasks.Task;

namespace DigitalAgency.Bll.TelegramBot.Services
{
    public class Buttons : IButtons
    {
        private readonly IExecutorStorage _executorStorage;
        private readonly ITelegramBotClient _telegram;

        public Buttons(IExecutorStorage executorStorage, ITelegramBotClient telegram)
        {
            _executorStorage = executorStorage;
            _telegram = telegram;
        }

        public async Task ViewOrderButtons(IEnumerable<BotShortOrderModel> orders, Chat chat, string messageString = "Orders")
        {
            var keyOrders = new ConcurrentDictionary<string,string>();
            foreach (var order in orders)
            {
                keyOrders.TryAdd("Project: "+order.ProjectName+" Tasks: " +order.TasksCount, order.Id.ToString());
            }
        
            var keyboard = KeyboardMessages.DefaultInlineKeyboardMessage(keyOrders);
        
            await _telegram.SendTextMessageAsync(chat, messageString, replyMarkup: keyboard);
        }
        
        public async Task ViewProjectButtons(IEnumerable<Project> projects, long chatId, string messageString = "Projects")
        {
            var keyOrders = new ConcurrentDictionary<string,string>();
            foreach (var project in projects)
            {
                keyOrders.TryAdd(project.ProjectName + " " + project.ProjectDescription, project.Id.ToString());
            }
            var keyboard = KeyboardMessages.DefaultInlineKeyboardMessage(keyOrders);
            
            await _telegram.SendTextMessageAsync(chatId, messageString, replyMarkup: keyboard);
        }
        
        //todo: what is this?
        public async Task AddOrderViewProjectButtons(IEnumerable<Project> projects, long chatId, string key = "project")
        {
            var keyOrders = new ConcurrentDictionary<string,string>();
            foreach (var project in projects)
            {
                keyOrders.TryAdd(project.ProjectName + " " + project.ProjectDescription + " ; " + project.ProjectLink, 
                    string.Concat(key,";",project.Id));
            }
            var keyboard = KeyboardMessages.DefaultInlineKeyboardMessage(keyOrders);
        
            await _telegram.SendTextMessageAsync(chatId, key, replyMarkup: keyboard);
        } 
    }
}