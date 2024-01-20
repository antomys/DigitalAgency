using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DigitalAgency.Bll.TelegramBot.Models;
using DigitalAgency.Bll.TelegramBot.Services.Helpers;
using DigitalAgency.Bll.TelegramBot.Services.Interfaces;
using DigitalAgency.Dal.Entities;
using DigitalAgency.Dal.Entities.Enums;
using DigitalAgency.Dal.Storages.Interfaces;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using Action = DigitalAgency.Dal.Entities.Action;
using File = System.IO.File;

namespace DigitalAgency.Bll.TelegramBot.Services.Menus;

public class ClientMenu : IClientMenu
{
    readonly IActionStorage _actionStorage;
    readonly IButtons _buttons;
    readonly IEnumerable<string> _clientCommands;
    readonly IClientMenuHelper _clientMenuHelper;
    readonly DownloadFileExtension _downloadFileExtension;
    readonly ReplyKeyboardMarkup _keyboard;
    readonly IOrderStorage _orderStorage;
    readonly IProjectStorage _projectStorage;
    readonly ITelegramBotClient _telegram;

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
                await _telegram.SendTextMessageAsync(update.Message.Chat, $"Welcome, {client.FirstName}!", replyMarkup: _keyboard);
                return;
            }
            await _telegram.SendTextMessageAsync(update.Message.Chat, "Wrong command", replyMarkup: _keyboard);
        }
        //    "ClientMenu": ["View my projects","Add Project","Create order","View My Orders"

        switch (update.Message.Text)
        {
            case "View my projects":
            {
                List<Project> projects = await _clientMenuHelper.ViewClientProjects(client);

                if (!projects.Any())
                {
                    await _telegram.SendTextMessageAsync(update.Message.Chat, "You have no projects!");
                    return;
                }

                await _buttons.ViewProjectButtons(projects, update.Message.Chat, "Your available projects");
                break;
            }
            case "Add Project":
            {
                Project project = await _projectStorage.CreateProjectAsync(new Project {
                    OwnerId = client.Id,
                });
                await _telegram.SendTextMessageAsync(update.Message.Chat, "Please, follow guidelines");
                Action action = new Action {
                    EntityType = typeof(Project).ToString(),
                    EntityId = project.Id,
                    ActionName = "name",
                    ClientId = client.Id,
                    ActionDate = DateTimeOffset.Now,
                    IsDone = false,
                };
                await _actionStorage.CreateActionAsync(action);

                await _telegram.SendTextMessageAsync(update.Message.Chat, "Please enter project name",
                    replyMarkup: new ForceReplyMarkup());
                break;
            }
            case "View My Orders":
            {
                List<BotShortOrderModel> orders = await _clientMenuHelper.ViewClientOrders(client);

                if (!orders.Any())
                {
                    await _telegram.SendTextMessageAsync(update.Message.Chat, "You have no orders!");
                    return;
                }

                await _buttons.ViewOrderButtons(orders, update.Message.Chat, "Your available orders", "order");
                break;
            }
        }
    }

    public async Task ProcessCallBack(Client client, Update update)
    {
        Chat chat = update.CallbackQuery.Message.Chat;

        if (update.CallbackQuery.Data.Contains("back", StringComparison.OrdinalIgnoreCase))
        {
            update.Message = new Message {
                Text = "Back", Chat = update.CallbackQuery.Message.Chat,
            };
            await ClientMainMenu(client, update);
            return;
        }

        string[] entityActionId = update.CallbackQuery.Data.Split(':');

        string entityId = entityActionId[^1];

        switch (entityActionId[0])
        {
            case "order":
            {
                Order thisOrder = await _orderStorage.GetOrderAsync(x => x.Id.ToString() == entityId);

                if (entityActionId.Length > 2 && Enum.TryParse<PositionsEnum>(entityActionId[1], out PositionsEnum parsedPosition))
                {
                    thisOrder.ExecutorPosition = parsedPosition;

                    await _orderStorage.UpdateAsync(thisOrder);

                    DateTimeFormatInfo dtfi = CultureInfo.GetCultureInfo("en-US").DateTimeFormat;
                    InlineKeyboardMarkup calendarMarkup = KeyboardMessages.Calendar(DateTime.Today, dtfi, thisOrder, "order:");

                    await _telegram.SendTextMessageAsync(client.ChatId, "Please specify date", replyMarkup: calendarMarkup);

                    return;
                }

                if (entityActionId.Length == 2)
                {
                    string orderString = $"Project Name: {thisOrder.Project.ProjectName}\n"
                                         + $"Created at: {thisOrder.CreationDate}\n"
                                         + $"Due to: {thisOrder.ScheduledTime}\n"
                                         + $"State: {Enum.GetName(thisOrder.StateEnum)}\n";
                    if (thisOrder.Executor.FirstName != "NULL")
                    {
                        string phoneNumber = Encoding.UTF8.GetString(Convert.FromBase64String(thisOrder.Executor.PhoneNumber));
                        orderString += $"Executor name:{thisOrder.Executor.FirstName}\n" + $"Executor phone:{phoneNumber}";

                        await _telegram.SendContactAsync(chat,
                            phoneNumber, thisOrder.Executor.FirstName);
                    }

                    ConcurrentDictionary<string, string> dictKeys = new ConcurrentDictionary<string, string>();
                    dictKeys.TryAdd("Cancel order", $"order:cancel:{thisOrder.Id}");
                    dictKeys.TryAdd("Delete order", $"order:delete:{thisOrder.Id}");
                    dictKeys.TryAdd("Back", "back");
                    InlineKeyboardMarkup editKeys = KeyboardMessages.DefaultInlineKeyboardMessage(dictKeys);
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
                        if (thisOrder.Executor.FirstName != "NULL")
                            await _telegram.SendTextMessageAsync(thisOrder.Executor.ChatId,
                                $"State of order for project {thisOrder.Project.ProjectName}"
                                + $" was changed by client {client.FirstName}\n"
                                + $"From {thisOrder.StateEnum} to {OrderStateEnum.Canceled}");

                        thisOrder.StateEnum = OrderStateEnum.Canceled;
                        await _orderStorage.UpdateAsync(thisOrder);
                        //todo debug. new feature
                        await _telegram.SendTextMessageAsync(chat, "Successfully cancelled!", replyMarkup: _keyboard);
                        return;
                    }
                    case "pck":
                    {
                        DateTimeOffset date = DateTimeOffset.Parse(entityActionId[2]);

                        await _telegram.SendTextMessageAsync(chat,
                            $"Your date in order for project {thisOrder.Project.ProjectName}\n" + $"has been changed to {date}",
                            replyMarkup: _keyboard);

                        await _telegram.SendTextMessageAsync(chat, "Successfully created order!");

                        thisOrder.ScheduledTime = date;
                        await _orderStorage.UpdateAsync(thisOrder);

                        return;
                    }
                }
                return;
            }
            case "project":
            {
                Project thisProject = await _projectStorage.GetProjectAsync(x => x.Id.ToString() == entityId);
                if (entityActionId.Length == 2)
                {
                    string projectString = $"Project name: {thisProject.ProjectName}\n" + $"Project Description:{thisProject.ProjectDescription}\n";

                    string filePath = thisProject.ProjectFilePath;

                    if (File.Exists(filePath) && Uri.TryCreate(thisProject.ProjectLink, UriKind.Absolute, out _))
                    {
                        await using FileStream stream = File.Open(filePath, FileMode.Open);
                        InputFileStream inputOnlineFile = InputFile.FromStream(stream, Path.GetFileName(filePath));

                        projectString += $"Link: {thisProject.ProjectLink}\n";
                        await _telegram.SendDocumentAsync(chat, inputOnlineFile, caption: "Technical task");
                    }

                    ConcurrentDictionary<string, string> dictKeys = new ConcurrentDictionary<string, string>();
                    dictKeys.TryAdd("Create order", $"project:order:{thisProject.Id}");
                    dictKeys.TryAdd("Edit name", $"project:name:{thisProject.Id}");
                    dictKeys.TryAdd("Edit description", $"project:description:{thisProject.Id}");
                    dictKeys.TryAdd("Edit link", $"project:link:{thisProject.Id}");
                    dictKeys.TryAdd("Edit file", $"project:file:{thisProject.Id}");
                    dictKeys.TryAdd("Delete", $"project:delete:{thisProject.Id}");
                    dictKeys.TryAdd("Back", "back");
                    InlineKeyboardMarkup editKeys = KeyboardMessages.DefaultInlineKeyboardMessage(dictKeys);

                    await _telegram.SendTextMessageAsync(chat, projectString, replyMarkup: editKeys);
                }
                else
                {
                    switch (entityActionId[1])
                    {
                        case "order":
                        {
                            Executor unnamedExecutor = await _clientMenuHelper.GetNullExecutor();
                            Order order = new Order {
                                ClientId = client.Id,
                                ExecutorId = unnamedExecutor.Id,
                                ProjectId = thisProject.Id,
                                CreationDate = DateTimeOffset.Now,
                                StateEnum = OrderStateEnum.New,
                            };

                            await _orderStorage.CreateOrderAsync(order);

                            await SendPosition(chat, order);
                            return;
                        }
                        case "delete":
                        {
                            await _projectStorage.DeleteProjectAsync(thisProject.Id);
                            await _telegram.SendTextMessageAsync(chat, "Successfully deleted!",
                                replyMarkup: _keyboard);
                            return;
                        }
                    }

                    Action action = new Action {
                        EntityType = typeof(Project).ToString(),
                        EntityId = thisProject.Id,
                        ClientId = client.Id,
                        ActionDate = DateTimeOffset.Now,
                        IsDone = false,
                    };

                    action.ActionName = entityActionId[1] switch {
                        "name" => "name",
                        "description" => "description",
                        "link" => "link",
                        "file" => "file",
                        _ => action.ActionName,
                    };
                    await _actionStorage.CreateActionAsync(action);
                    string textMessage = $"Enter new {entityActionId[1]} for project \"{thisProject.ProjectName}\"";

                    await _telegram.SendTextMessageAsync(chat, textMessage,
                        replyMarkup: new ForceReplyMarkup());
                }

                break;
            }
        }
    }

    public async Task ProcessReplyToMessage(Client client, Update update)
    {
        Action thisAction = await _actionStorage.GetActionAsync(x => x.ClientId == client.Id);

        if (thisAction.EntityType == typeof(Project).ToString())
        {
            Project thisProject = await _projectStorage.GetProjectAsync(x => x.Id == thisAction.EntityId);
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
                        await _telegram.SendTextMessageAsync(update.Message.Chat, "Wrong input!",
                            replyMarkup: _keyboard);
                        return;
                    }
                    string filePath = await _downloadFileExtension.DownloadFile(update.Message.Document.FileId);
                    thisProject.ProjectFilePath = filePath;
                    break;
                }
            }

            string previousAction = thisAction.ActionName;
            thisAction = await UpdateAction(thisAction, thisProject);

            await _actionStorage.UpdateActionAsync(thisAction);
            await _projectStorage.UpdateProjectAsync(thisProject);
            if (thisAction.IsDone)
            {
                await _telegram.SendTextMessageAsync(update.Message.Chat, $"Successfully added {thisAction.ActionName}!",
                    replyMarkup: _keyboard);
            }
            else
            {
                await _telegram.SendTextMessageAsync(update.Message.Chat,
                    $"Successfully added {previousAction}!\n" + $"Please enter {thisAction.ActionName}",
                    replyMarkup: new ForceReplyMarkup());
            }

            return;
        }
        // for what this? Investigate!
        if (thisAction.EntityType == typeof(Client).ToString())
        { }
    }

    async Task SendPosition(Chat chat, Order thisOrder)
    {
        ConcurrentDictionary<string, string> dictKeys = new ConcurrentDictionary<string, string>();
        foreach (string position in Enum.GetNames(typeof(PositionsEnum))
                     .Where(x => x != Enum.GetName(PositionsEnum.Unknown)))
        {
            dictKeys.TryAdd(position, $"order:{position}:{thisOrder.Id}");
        }
        InlineKeyboardMarkup editKeys = KeyboardMessages.DefaultInlineKeyboardMessage(dictKeys);

        await _telegram.SendTextMessageAsync(chat, "Please choose executor position",
            replyMarkup: editKeys);
    }

    Task<Action> UpdateAction(Action action, Project project)
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