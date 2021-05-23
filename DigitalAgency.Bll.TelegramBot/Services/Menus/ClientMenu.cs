using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DigitalAgency.Bll.TelegramBot.Services.Helpers;
using DigitalAgency.Bll.TelegramBot.Services.Interfaces;
using DigitalAgency.Dal.Entities;
using DigitalAgency.Dal.Entities.Enums;
using DigitalAgency.Dal.Storages.Interfaces;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;
using Action = DigitalAgency.Dal.Entities.Action;

namespace DigitalAgency.Bll.TelegramBot.Services.Menus
{
    public class ClientMenu : IClientMenu
    {
        private readonly ITelegramBotClient _telegram;
        private readonly IClientMenuHelper _clientMenuHelper;
        private readonly IEnumerable<string> _clientCommands;
        private readonly IButtons _buttons;
        private readonly IOrderStorage _orderStorage;
        private readonly IProjectStorage _projectStorage;
        private readonly ReplyKeyboardMarkup _keyboard;
        private readonly IActionStorage _actionStorage;
        private readonly DownloadFileExtension _downloadFileExtension;

        public ClientMenu(
            ITelegramBotClient telegram,
            IButtons buttons, 
            IOptions<BotConfiguration> clientCommands, 
            IClientMenuHelper clientMenuHelper, 
            IProjectStorage projectStorage, 
            IOrderStorage orderStorage, IActionStorage actionStorage, 
            DownloadFileExtension downloadFileExtension)
        {
            _telegram = telegram;
            _buttons = buttons;
            _clientMenuHelper = clientMenuHelper;
            _projectStorage = projectStorage;
            _orderStorage = orderStorage;
            _actionStorage = actionStorage;
            _downloadFileExtension = downloadFileExtension;
            _clientCommands = clientCommands.Value.ClientMenu;
            _keyboard = KeyboardMessages.DefaultKeyboardMessage(_clientCommands);
        }
        public async Task ClientMainMenu(Client client, Update update)
        {
            if (!_clientCommands.Contains(update.Message.Text, StringComparer.OrdinalIgnoreCase))
            {
                if (update.Message.Text is "Register as Client" or "Back")
                {
                    await _telegram.SendTextMessageAsync(update.Message.Chat,$"Welcome, {client.FirstName}!", replyMarkup: _keyboard);
                    return;
                }
                await _telegram.SendTextMessageAsync(update.Message.Chat,$"Wrong command", replyMarkup: _keyboard);
            }
            //    "ClientMenu": ["View my projects","Add Project","Create order","View My Orders"

            switch (update.Message.Text)
            {
                case "View my projects":
                {
                    var projects = await _clientMenuHelper.ViewClientProjects(client);
                    
                    if (!projects.Any())
                    {
                        await _telegram.SendTextMessageAsync(update.Message.Chat, "You have no projects!");
                        return;
                    }

                    await _buttons.ViewProjectButtons(projects, update.Message.Chat,"Your available projects");
                    break;
                }
                case "Add Project":
                {
                    var project = await _projectStorage.CreateProjectAsync(new Project{OwnerId = client.Id});
                    await _telegram.SendTextMessageAsync(update.Message.Chat, "Please, follow guidelines");
                    var action = new Action
                    {
                        EntityType = typeof(Project).ToString(),
                        EntityId = project.Id,
                        ActionName = "name",
                        ClientId = client.Id,
                        ActionDate = DateTimeOffset.Now,
                        IsDone = false
                    };
                    await _actionStorage.CreateActionAsync(action);

                    await _telegram.SendTextMessageAsync(update.Message.Chat, "Please enter project name",
                        replyMarkup: new ForceReplyMarkup());
                    break;
                }
                case "Create order":
                {
                    break;
                }
                case "View My Orders":
                {
                    var orders = await _clientMenuHelper.ViewClientOrders(client);
                    
                    if (!orders.Any())
                    {
                        await _telegram.SendTextMessageAsync(update.Message.Chat, "You have no orders!");
                        return;
                    }

                    await _buttons.ViewOrderButtons(orders, update.Message.Chat,"Your available orders","order");
                    break;
                }
                default:
                {
                    break;
                }
            }
        }

        public async Task ProcessCallBack(Client client, Update update)
        {
            var chat = update.CallbackQuery.Message.Chat;

            if (update.CallbackQuery.Data.Contains("back", StringComparison.OrdinalIgnoreCase))
            {
                update.Message = new Message {Text = "Back", Chat = update.CallbackQuery.Message.Chat};
                await ClientMainMenu(client, update);
                return;
            }

            var entityActionId = update.CallbackQuery.Data.Split(':');
            
            var entityId = entityActionId[^1];

            switch (entityActionId[0])
            {
                case "order":
                {
                    var thisOrder = await _orderStorage.GetOrderAsync(x => x.Id.ToString() == entityId);
                    if (entityActionId.Length == 2)
                    {
                        var orderString = $"Project Name: {thisOrder.Project.ProjectName}\n" +
                                          $"Created at: {thisOrder.CreationDate}\n" +
                                          $"Due to: {thisOrder.ScheduledTime}\n" +
                                          $"State: {Enum.GetName(thisOrder.StateEnum)}";
                        if (thisOrder.Executor.FirstName != "NULL")
                        {
                            orderString += $"Executor name:{thisOrder.Executor.FirstName}\n" +
                                           $"Executor phone:{thisOrder.Executor.PhoneNumber}";
                            await _telegram.SendContactAsync(chat, 
                                thisOrder.Executor.PhoneNumber, thisOrder.Executor.FirstName);
                        }
                        
                        var dictKeys = new ConcurrentDictionary<string, string>();
                        dictKeys.TryAdd("Cancel order", $"order:cancel:{thisOrder.Id}");
                        dictKeys.TryAdd("Delete order", $"order:delete:{thisOrder.Id}");
                        dictKeys.TryAdd("Back", "back");
                        var editKeys = KeyboardMessages.DefaultInlineKeyboardMessage(dictKeys);
                        await _telegram.SendTextMessageAsync(chat, orderString, replyMarkup: editKeys);
                    }

                    switch (entityActionId[1])
                    {
                        case "delete":
                        {
                            await _orderStorage.DeleteOrderAsync(thisOrder.Id);

                            await _telegram.SendTextMessageAsync(chat, "Successfully deleted!", replyMarkup: _keyboard);
                            return;
                        }
                        case "cancel":
                        {
                            await _telegram.SendTextMessageAsync(chat,
                                $"State of order for project {thisOrder.Project.ProjectName}" +
                                $"was changed by client {client.FirstName}\n" +
                                $"From {thisOrder.StateEnum} to {OrderStateEnum.Canceled}");
                        
                            thisOrder.StateEnum = OrderStateEnum.Canceled;
                            await _orderStorage.UpdateAsync(thisOrder);
                            //todo debug. new feature
                            await _telegram.SendTextMessageAsync(chat, "Successfully cancelled!", replyMarkup: _keyboard);
                            return;
                        }
                    }
                    return;
                }
                case "project":
                {
                    var thisProject = await _projectStorage.GetProjectAsync(x => x.Id.ToString() == entityId);
                    if (entityActionId.Length == 2)
                    {
                        var projectString = $"Project name: {thisProject.ProjectName}\n" +
                                            $"Project Description:{thisProject.ProjectDescription}\n";

                                            var filePath = thisProject.ProjectFilePath;
                
                        if (System.IO.File.Exists(filePath) && Uri.TryCreate(thisProject.ProjectLink, UriKind.Absolute, out _) )
                        {
                            await using var stream = System.IO.File.Open(filePath, FileMode.Open);
                            var inputOnlineFile = new InputOnlineFile(stream) {FileName = Path.GetFileName(filePath)};
                            projectString += $"Link: {thisProject.ProjectLink}\n";
                            await _telegram.SendDocumentAsync(chat, inputOnlineFile,"Technical task");
                        }

                        var dictKeys = new ConcurrentDictionary<string, string>();
                        dictKeys.TryAdd("Edit name", $"project:name:{thisProject.Id}");
                        dictKeys.TryAdd("Edit description", $"project:description:{thisProject.Id}");
                        dictKeys.TryAdd("Edit link", $"project:link:{thisProject.Id}");
                        dictKeys.TryAdd("Edit file", $"project:file:{thisProject.Id}");
                        dictKeys.TryAdd("Back", "back");
                        var editKeys = KeyboardMessages.DefaultInlineKeyboardMessage(dictKeys);

                        await _telegram.SendTextMessageAsync(chat, projectString, replyMarkup: editKeys);
                    }
                    else
                    {
                        var action = new Action
                        {
                            EntityType = typeof(Project).ToString(),
                            EntityId = thisProject.Id,
                            ClientId = client.Id,
                            ActionDate = DateTimeOffset.Now,
                            IsDone = false
                        };

                        action.ActionName = entityActionId[1] switch
                        {
                            "name" => "name",
                            "description" => "description",
                            "link" => "link",
                            "file" => "file",
                            _ => action.ActionName
                        };
                        await _actionStorage.CreateActionAsync(action);
                        var textMessage = $"Enter new {entityActionId[1]} for project \"{thisProject.ProjectName}\"";

                        await _telegram.SendTextMessageAsync(chat, textMessage,
                            replyMarkup: new ForceReplyMarkup());
                    }
                   
                    break;
                }
            }
        }
        public async Task ProcessReplyToMessage(Client client, Update update)
        {
            var thisAction = await _actionStorage.GetActionAsync(x => x.ClientId == client.Id);

            if (thisAction.EntityType == typeof(Project).ToString())
            {
                var thisProject = await _projectStorage.GetProjectAsync(x => x.Id == thisAction.EntityId);
                switch (thisAction.ActionName)
                {
                    case "name":
                    {
                        thisProject.ProjectName = update.Message.Text;
                        break;
                    }
                    case "description":
                    {
                        thisProject.ProjectDescription = update.Message.Text;
                        break;
                    }
                    case "link":
                    {
                        thisProject.ProjectLink = update.Message.Text;
                        break;
                    }
                    case "file":
                    {
                        if (update.Message.Document == null)
                        {
                            await _telegram.SendTextMessageAsync(update.Message.Chat, $"Wrong input!",
                                replyMarkup: _keyboard);
                            return;
                        }
                        var filePath = await _downloadFileExtension.DownloadFile(update.Message.Document.FileId);
                        thisProject.ProjectFilePath = filePath;
                        break;
                    }
                }

                var previousAction = thisAction.ActionName;
                thisAction = await UpdateAction(thisAction, thisProject);
                
                await _actionStorage.UpdateActionAsync(thisAction);
                await _projectStorage.UpdateProjectAsync(thisProject);
                if(thisAction.IsDone)
                    await _telegram.SendTextMessageAsync(update.Message.Chat, $"Successfully added {thisAction.ActionName}!",
                        replyMarkup: _keyboard);
                else
                {
                    
                    await _telegram.SendTextMessageAsync(update.Message.Chat, 
                        $"Successfully added {previousAction}!\n" +
                        $"Please enter {thisAction.ActionName}",
                        replyMarkup: new ForceReplyMarkup());
                }
                
                return;
            }
            
            if (thisAction.EntityType == typeof(Client).ToString())
            {
                
            }
        }

        private Task<Action> UpdateAction(Action action, Project project)
        {
            if (string.IsNullOrEmpty(project.ProjectName))
            {
                action.ActionName = "name";
            } 
            else if (string.IsNullOrEmpty(project.ProjectLink))
            {
                action.ActionName = "link";
            } 
            else if (string.IsNullOrEmpty(project.ProjectDescription))
            {
                action.ActionName = "description";
            }
            else if (string.IsNullOrEmpty(project.ProjectFilePath))
            {
                action.ActionName = "file";
            }
            else
            {
                action.IsDone = true;
            }

            return Task.FromResult(action);
        }
        
    }
}