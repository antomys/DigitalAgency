using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DigitalAgency.Bll.TelegramBot.Models;
using DigitalAgency.Bll.TelegramBot.Services.Interfaces;
using DigitalAgency.Dal.Entities;
using DigitalAgency.Dal.Entities.Enums;
using DigitalAgency.Dal.Storages.Interfaces;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using File = System.IO.File;

namespace DigitalAgency.Bll.TelegramBot.Services.Menus;

public class ExecutorMenu : IExecutorMenu
{
    readonly IButtons _buttons;
    readonly IEnumerable<string> _executorCommands;
    readonly IExecutorMenuHelper _executorMenuHelper;
    readonly IExecutorStorage _executorStorage;
    readonly ReplyKeyboardMarkup _keyboard;
    readonly IMapper _mapper;
    readonly IOrderStorage _orderStorage;
    readonly ITelegramBotClient _telegram;

    public ExecutorMenu(ITelegramBotClient telegram,
        IExecutorStorage executorStorage,
        IOptions<BotConfiguration> executorCommands,
        IExecutorMenuHelper executorMenuHelper,
        IButtons buttons, IOrderStorage orderStorage,
        IMapper mapper)
    {
        _telegram = telegram;
        _executorStorage = executorStorage;
        _executorMenuHelper = executorMenuHelper;
        _buttons = buttons;
        _orderStorage = orderStorage;
        _mapper = mapper;
        _executorCommands = executorCommands.Value.ExecutorMenu;
        _keyboard = KeyboardMessages.DefaultKeyboardMessage(_executorCommands);
    }

    // Crank code
    public async Task StartMenu(Executor executor, Update update)
    {
        if (executor.Position == PositionsEnum.Unknown
            || update.Message.Text != null
            && Enum.TryParse<PositionsEnum>(update.Message.Text, out _)
            || update.CallbackQuery is { Data: "Back" })
        {
            if (await EditPositionMenu(update) == false)
                return;

            await _telegram.SendTextMessageAsync(update.Message.Chat, $"Welcome, {executor.FirstName}!", replyMarkup: _keyboard);
            return;
        }

        // Crank code
        if (!_executorCommands.Contains(update.Message.Text, StringComparer.OrdinalIgnoreCase) && update.Message.Text != "Back")
        {
            await _telegram.SendTextMessageAsync(update.Message.Chat, "Wrong command", replyMarkup: _keyboard);
        }

        switch (update.Message.Text)
        {
            case "View my orders":
            {
                List<BotShortOrderModel> orders = await _executorMenuHelper.ViewMyOrders(executor);

                if (!orders.Any())
                {
                    await _telegram.SendTextMessageAsync(update.Message.Chat, "You have no active orders");
                    return;
                }

                await _buttons.ViewOrderButtons(orders, update.Message.Chat, "Your orders to complete");
                break;
            }
            case "Edit my position":
            {
                await SendEditPosition(update);
                break;
            }
            case "View available orders":
            {
                List<BotShortOrderModel> orders = await _executorMenuHelper.ViewFreeOrders(executor);

                if (!orders.Any())
                {
                    await _telegram.SendTextMessageAsync(update.Message.Chat, "No available orders!");
                    return;
                }

                await _buttons.ViewOrderButtons(orders, update.Message.Chat, "All available orders!");
                break;
            }
            default:
            {
                await _telegram.SendTextMessageAsync(update.Message.Chat, $"Welcome, {executor.FirstName}!", replyMarkup: _keyboard);
                break;
            }
        }
    }

    public async Task ProcessCallBack(Executor executor, Update update)
    {
        ReplyKeyboardMarkup backKey = KeyboardMessages.DefaultKeyboardMessage(new[] {
            "Back",
        });
        Chat chat = update.CallbackQuery.Message.Chat;
        if (!update.CallbackQuery.Data.Contains(':'))
        {
            if (update.CallbackQuery.Data == "Back")
            {
                await _telegram.SendTextMessageAsync(chat, "Back", replyMarkup: _keyboard);
                return;
            }

            Order thisOrder = await _orderStorage.GetOrderAsync(x => x.Id.ToString() == update.CallbackQuery.Data);

            BotShortOrderModel mapped = _mapper.Map<BotShortOrderModel>(thisOrder);
            string phoneNumber = Encoding.UTF8.GetString(Convert.FromBase64String(mapped.ClientPhone));

            string orderString = $"Project Name: {mapped.ProjectName}\n"
                                 + $"Client Name: {mapped.ClientName}\n"
                                 + $"Client Phone: {phoneNumber}\n"
                                 + $"Created at: {mapped.CreationDate}\n"
                                 + $"Due to: {mapped.ScheduledTime}\n"
                                 + $"State: {Enum.GetName(thisOrder.StateEnum)}\n";
            string filePath = thisOrder.Project.ProjectFilePath;

            if (File.Exists(filePath) && Uri.TryCreate(thisOrder.Project.ProjectLink, UriKind.Absolute, out _))
            {
                await using FileStream stream = File.Open(filePath, FileMode.Open);
                var inputOnlineFile = InputFile.FromStream(stream, Path.GetFileName(filePath));
                orderString += $"Link: {thisOrder.Project.ProjectLink}\n";
                await _telegram.SendDocumentAsync(chat, inputOnlineFile, caption: "Technical task");
            }

            await _telegram.SendTextMessageAsync(chat, orderString);

            if (thisOrder.ExecutorId == executor.Id)
            {
                ConcurrentDictionary<string, string> dictKeys = new ConcurrentDictionary<string, string>();
                dictKeys.TryAdd("Edit State", $"state:{thisOrder.Id}");
                dictKeys.TryAdd("Edit End date", $"date:{thisOrder.Id}");
                dictKeys.TryAdd("Back", "Back");
                InlineKeyboardMarkup editKeys = KeyboardMessages.DefaultInlineKeyboardMessage(dictKeys);

                await _telegram.SendContactAsync(chat, phoneNumber, mapped.ClientName, replyMarkup: editKeys);
            }
            else
            {
                InlineKeyboardMarkup keys = await _executorMenuHelper.ConstructConfirmOrderButtons(thisOrder);
                await _telegram.SendTextMessageAsync(chat, "Do you confirm this order?", replyMarkup: keys);
            }
        }
        else
        {
            string[] answerId = update.CallbackQuery.Data.Split(':');

            string id = answerId[^1];

            Order thisOrder = await _orderStorage.GetOrderAsync(x => x.Id.ToString() == id);

            switch (answerId[0])
            {
                case "yes":
                {
                    thisOrder.ExecutorId = executor.Id;
                    await _orderStorage.UpdateAsync(thisOrder);
                    await _telegram.SendTextMessageAsync(chat,
                        "Successfully confirmed!\n\nPlease contact client at:");

                    await _telegram.SendTextMessageAsync(thisOrder.Client.ChatId,
                        $"Your order for {thisOrder.Project.ProjectName}" + $" has been picked up by {executor.FirstName}!\n\nPlease contact him at:");

                    string executorPhoneNumber = Encoding.UTF8.GetString(Convert.FromBase64String(executor.PhoneNumber));
                    string clientPhoneNumber = Encoding.UTF8.GetString(Convert.FromBase64String(thisOrder.Client.PhoneNumber));

                    await _telegram.SendContactAsync(thisOrder.Client.ChatId,
                        executorPhoneNumber,
                        executor.FirstName);

                    await _telegram.SendContactAsync(chat, clientPhoneNumber, thisOrder.Client.FirstName,
                        replyMarkup: backKey);
                    break;
                }
                case "no":
                {
                    await _telegram.SendTextMessageAsync(chat, "Okay!", replyMarkup: _keyboard);
                    break;
                }
                case "state":
                {
                    InlineKeyboardMarkup states = await _executorMenuHelper.ConstructStatesButtons(update, thisOrder);
                    await _telegram.SendTextMessageAsync(chat, "Choose new order state", replyMarkup: states);
                    break;
                }
                case "date":
                {
                    DateTimeFormatInfo dtfi = CultureInfo.GetCultureInfo("en-US").DateTimeFormat;
                    InlineKeyboardMarkup calendarMarkup = KeyboardMessages.Calendar(DateTime.Today, dtfi, thisOrder);

                    await _telegram.SendTextMessageAsync(chat, "Pick date:", replyMarkup: calendarMarkup);
                    break;
                }
                case "pck":
                {
                    DateTimeOffset date = DateTimeOffset.Parse(answerId[1]);
                    await _telegram.SendTextMessageAsync(chat, $"Changed date\nfrom {thisOrder.ScheduledTime}\n" + $"to {date}",
                        replyMarkup: _keyboard);

                    await _telegram.SendTextMessageAsync(thisOrder.Client.ChatId,
                        $"Your order for project {thisOrder.Project.ProjectName}\n" + $"has been changed from {thisOrder.ScheduledTime} to {date}");

                    thisOrder.ScheduledTime = date;
                    await _orderStorage.UpdateAsync(thisOrder);

                    return;
                }
            }

            if (Enum.TryParse<OrderStateEnum>(answerId[0], out OrderStateEnum newState))
            {
                await _telegram.SendTextMessageAsync(thisOrder.Client.ChatId,
                    "State of your order on\n"
                    + $"project {thisOrder.Project.ProjectName} was changed from {Enum.GetName(thisOrder.StateEnum)} "
                    + $"to {newState}");
                await _telegram.SendTextMessageAsync(chat,
                    $"Success! Changed state from {thisOrder.StateEnum}" + $"to {newState}", replyMarkup: _keyboard);

                thisOrder.StateEnum = newState;
                await _orderStorage.UpdateAsync(thisOrder);
            }
        }
    }

    async Task<bool> EditPositionMenu(Update update)
    {
        if (update.Message.Text == null)
        {
            await _telegram.SendTextMessageAsync(update.Message.Chat, "Wrong input!");
            return false;
        }

        if (!Enum.TryParse<PositionsEnum>(update.Message.Text, out _))
        {
            await SendEditPosition(update);
        }
        else
        {
            return await EditExecutorPosition(update.Message);
        }

        return false;
    }

    async Task SendEditPosition(Update update)
    {
        List<string> keys = Enum.GetNames(typeof(PositionsEnum))
            .Where(x => x != Enum.GetName(PositionsEnum.Unknown)).ToList();

        ReplyKeyboardMarkup keyboard = KeyboardMessages.DefaultKeyboardMessage(keys);

        await _telegram.SendTextMessageAsync(update.Message.Chat, "Please choose your position",
            replyMarkup: keyboard);
    }

    async Task<bool> EditExecutorPosition(Message message)
    {
        Executor thisExecutor = await _executorStorage.GetExecutorAsync(x => x.TelegramId == message.From.Id);

        if (!Enum.TryParse<PositionsEnum>(message.Text, out PositionsEnum parsedPosition)) return false;
        thisExecutor.Position = parsedPosition;
        await _executorStorage.UpdateExecutorAsync(thisExecutor);
        await _telegram.SendTextMessageAsync(message.Chat, $"Success! Your position is {parsedPosition}");
        return true;
    }
}