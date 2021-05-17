using System;
using DigitalAgency.Bll.Services.Interfaces;
using DigitalAgency.Dal.Entities;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using Task = System.Threading.Tasks.Task;

namespace DigitalAgency.Bll.Services.Bot
{
    public class ProcessReply
    {
        private readonly IProjectService _projectService;
        private readonly ITelegramBotClient _telegram;
        private readonly IOrderService _orderService;
        private readonly BotConfiguration _botConfiguration;
        
        public ProcessReply(
            IOrderService orderService, 
            ITelegramBotClient telegram, 
            IProjectService projectService, 
            IOptions<BotConfiguration> botConfiguration)
        {
            _orderService = orderService;
            _telegram = telegram;
            _projectService = projectService;
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
                     await _projectService.CreateProjectAsync(new Project {ProjectName = message.Text, OwnerId = thisClient.Id});
                            
                     await _telegram.SendTextMessageAsync(chatId, $"Please write project description",
                         replyMarkup: new ForceReplyMarkup());
                     await _telegram.SendTextMessageAsync(chatId, $"(max 128 symbols)");
                     break;
                 }
                 case "please write project description":
                 {
                     var car = await _projectService.GetLastAdded(thisClient.Id);
                     car.ProjectDescription = message.Text;
                            
                     await _projectService.UpdateProjectAsync(car);
                            
                     await _telegram.SendTextMessageAsync(chatId, "Please insert project link",
                         replyMarkup: new ForceReplyMarkup());
                     break;
                 }
                 case "please insert project link":
                 {
                     var car = await _projectService.GetLastAdded(thisClient.Id);
                     car.ProjectLink = message.Text;
                            
                     await _projectService.UpdateProjectAsync(car);
        
                     await _telegram.SendTextMessageAsync(chatId, "Great! Your car has been added!",
                         replyMarkup: KeyboardMessages.DefaultKeyboardMessage(_botConfiguration.CommandsClient));
                     break;
                 }
                 case "please enter scheduled time for order":
                 {
                     var order = await _orderService.GetLastAdded(thisClient.Id);
                     order.ScheduledTime = DateTime.Parse(message.Text);
        
                     await _orderService.UpdateAsync(order);
                            
                     await _telegram.SendTextMessageAsync(chatId, "Great! Your order has been added!\nWait for operator confirmation",
                         replyMarkup: KeyboardMessages.DefaultKeyboardMessage(_botConfiguration.CommandsClient));
                     return;
                 }
             }
                }
    }
}