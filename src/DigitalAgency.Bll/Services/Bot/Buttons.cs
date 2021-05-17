using System.Collections.Concurrent;
using System.Collections.Generic;
using DigitalAgency.Bll.Services.Interfaces;
using DigitalAgency.Dal.Entities;
using DigitalAgency.Dal.Storages.Interfaces;
using Telegram.Bot;
using Task = System.Threading.Tasks.Task;

namespace DigitalAgency.Bll.Services.Bot
{
    public class Buttons
    {
        private readonly IExecutorStorage _executorStorage;
        private readonly ITelegramBotClient _telegram;

        public Buttons(IExecutorStorage executorStorage, ITelegramBotClient telegram)
        {
            _executorStorage = executorStorage;
            _telegram = telegram;
        }

        private async Task ViewOrderButtons(IEnumerable<Order> orders, long chatId)
        {
            var keyOrders = new ConcurrentDictionary<string,string>();
            foreach (var order in orders)
            {
                var executor = await _executorStorage.GetExecutorByIdAsync(order.ExecutorId ?? order.ClientId);
                keyOrders.TryAdd(order.Project.ProjectName 
                                 + " " + order.Project.ProjectDescription 
                                 + " ; state: " + order.StateEnum + " ; executor: " 
                                 + executor.FirstName, 
                    string.Concat(order.GetType().ToString(),";",order.Id));
            }
        
            var keyboard = KeyboardMessages.DefaultInlineKeyboardMessage(keyOrders);
        
            await _telegram.SendTextMessageAsync(chatId, "Your active orders", replyMarkup: keyboard);
        }
        
        private async Task ViewProjectButtons(IEnumerable<Project> projects, long chatId)
        {
            var keyOrders = new ConcurrentDictionary<string,string>();
            foreach (var project in projects)
            {
                keyOrders.TryAdd(project.ProjectName + " " + project.ProjectDescription + " ; " + project.ProjectLink, 
                    string.Concat(project.GetType().ToString(),";",project.Id));
            }
            var keyboard = KeyboardMessages.DefaultInlineKeyboardMessage(keyOrders);
        
            await _telegram.SendTextMessageAsync(chatId, "Your active projects", replyMarkup: keyboard);
        }
        
        private async Task AddOrderViewProjectButtons(IEnumerable<Project> projects, long chatId, string key = "project")
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