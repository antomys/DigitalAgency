using System;
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
    public class ProcessReply
    {
        private readonly IProjectStorage _projectStorage;
        private readonly ITelegramBotClient _telegram;
        private readonly IOrderStorage _orderStorage;
        private readonly BotConfiguration _botConfiguration;
        
        public ProcessReply(
            IOrderStorage orderStorage, 
            ITelegramBotClient telegram, 
            IProjectStorage projectStorage, 
            IOptions<BotConfiguration> botConfiguration)
        {
            _orderStorage = orderStorage;
            _telegram = telegram;
            _projectStorage = projectStorage;
            _botConfiguration = botConfiguration.Value;
        }
         public async Task ProcessClientReplies(Message message, Client thisClient, Update update) 
         { 
             var chatId = message.Chat.Id;
             var repliedTo = message.ReplyToMessage;
             
             switch (repliedTo.Text.ToLower())
             {
                 case "please write project name":
                 {
                     await _projectStorage.CreateProjectAsync(new Project {ProjectName = message.Text, OwnerId = thisClient.Id});
                            
                     await _telegram.SendTextMessageAsync(chatId, $"Please write project description",
                         replyMarkup: new ForceReplyMarkup());
                     await _telegram.SendTextMessageAsync(chatId, $"(max 128 symbols)");
                     break;
                 }
                 case "please write project description":
                 {
                     var car = await _projectStorage.GetLastAdded(thisClient.Id);
                     car.ProjectDescription = message.Text;
                            
                     await _projectStorage.UpdateProjectAsync(car);
                            
                     await _telegram.SendTextMessageAsync(chatId, "Please insert project link",
                         replyMarkup: new ForceReplyMarkup());
                     break;
                 }
                 case "please insert project link":
                 {
                     var car = await _projectStorage.GetLastAdded(thisClient.Id);
                     car.ProjectLink = message.Text;
                            
                     await _projectStorage.UpdateProjectAsync(car);
        
                     await _telegram.SendTextMessageAsync(chatId, "Great! Your car has been added!",
                         replyMarkup: KeyboardMessages.DefaultKeyboardMessage(_botConfiguration.CommandsClient));
                     break;
                 }
                 case "please enter scheduled time for order":
                 {
                     var order = await _orderStorage.GetLastAdded(thisClient.Id);
                     order.ScheduledTime = DateTime.Parse(message.Text);
        
                     await _orderStorage.UpdateAsync(order);
                            
                     await _telegram.SendTextMessageAsync(chatId, "Great! Your order has been added!\nWait for operator confirmation",
                         replyMarkup: KeyboardMessages.DefaultKeyboardMessage(_botConfiguration.CommandsClient));
                     return;
                 }
             }
                }
    }
}