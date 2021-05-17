using System.Collections.Generic;
using System.Linq;
using DigitalAgency.Bll.Services.Bot.Interfaces;
using DigitalAgency.Bll.Services.Interfaces;
using DigitalAgency.Dal.Entities;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using Task = System.Threading.Tasks.Task;

namespace DigitalAgency.Bll.Services.Bot
{
    public class ClientMenu : IClientMenu
    {
        private readonly ITelegramBotClient _telegram;
        private readonly BotConfiguration _botConfiguration;
        private readonly IProjectService _projectService;
        private readonly IButtons _buttons;
        private readonly IOrderService _orderService;

        public ClientMenu(
            ITelegramBotClient telegram, 
            IOptions<BotConfiguration> botConfiguration, 
            IProjectService projectService, 
            IOrderService orderService, 
            IButtons buttons)
        {
            _telegram = telegram;
            _projectService = projectService;
            _orderService = orderService;
            _buttons = buttons;
            _botConfiguration = botConfiguration.Value;
        }
        public async Task ClientMainMenu(Client thisClient, Message receivedMessage)
        { 
            if (thisClient is not null) 
            { 
                await _telegram.SendTextMessageAsync(receivedMessage.Chat.Id, 
                    $"Client menu",replyMarkup: KeyboardMessages.DefaultKeyboardMessage(_botConfiguration.CommandsClient));
                       
                if (receivedMessage.Text == null)
                {
                    await _telegram.SendTextMessageAsync(receivedMessage.Chat.Id,
                        $"Wrong operation",
                        replyMarkup: KeyboardMessages.DefaultKeyboardMessage(_botConfiguration.CommandsClient));
                    return;
                }
                                
                switch (receivedMessage.Text.ToLower())
                {
                    case "view my projects":
                    {
                        await ViewProjects(thisClient, receivedMessage);
                        return;
                    }
                    case "add project":
                    {
                        await _telegram.SendTextMessageAsync(receivedMessage.Chat.Id,
                            "For this we need some information from you.\n Please reply to this messages");
                        await _telegram.SendTextMessageAsync(receivedMessage.Chat.Id, "Please write car manufacturer", replyMarkup: new ForceReplyMarkup());
                        return;
                    }
                    case "create order":
                    {
                        var cars = await _projectService.GetClientProjectsAsync(thisClient);
                        if (!cars.Any())
                        {
                            await _telegram.SendTextMessageAsync(receivedMessage.Chat.Id,
                                $"You have no cars. Try to create one");
                            return;
                        }
                        await _telegram.SendTextMessageAsync(receivedMessage.Chat.Id,
                            "For this we need some information from you.");
                        const string key = "Please choose a car to order";
                        await _buttons.AddOrderViewProjectButtons(cars,receivedMessage.Chat.Id,key);
                        return;
                    }
                    case "view my orders":
                    {
                        var orders = await _orderService.GetClientOrdersAsync(thisClient);
                        await ViewOrders(orders, receivedMessage);
                        return;
                    }
                    case "edit my information":
                    {
                        return;
                    }
                }
            }
        }
        private async Task ViewOrders(List<Order> orders, Message receivedMessage)
        {
            if (!orders.Any())
            {
                await _telegram.SendTextMessageAsync(receivedMessage.Chat.Id,
                    $"You have no active orders. Try to create one");
                return;
            }
            await _buttons.ViewOrderButtons(orders,receivedMessage.Chat.Id);
        }
        private async Task ViewProjects(Client thisClient, Message receivedMessage)
        {
            var projects = await _projectService.GetClientProjectsAsync(thisClient);
            if (!projects.Any())
            {
                await _telegram.SendTextMessageAsync(receivedMessage.Chat.Id,
                    $"You have no projects. Try to create one");
                return;
            }
           
            await _buttons.ViewProjectButtons(projects,receivedMessage.Chat.Id);
        }
    }
}