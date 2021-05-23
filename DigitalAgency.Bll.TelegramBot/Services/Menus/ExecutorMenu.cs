using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
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
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;

namespace DigitalAgency.Bll.TelegramBot.Services.Menus
{
    public class ExecutorMenu : IExecutorMenu
    {
        private readonly IExecutorStorage _executorStorage;
        private readonly ITelegramBotClient _telegram;
        private readonly IEnumerable<string> _executorCommands;
        private readonly IExecutorMenuHelper _executorMenuHelper;
        private readonly IOrderStorage _orderStorage;
        private readonly IButtons _buttons;
        private readonly IMapper _mapper;
        private readonly ReplyKeyboardMarkup _keyboard;
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
                || update.CallbackQuery is {Data: "Back"})
            {
                if(await EditPositionMenu(update) == false)
                    return;
                
                await _telegram.SendTextMessageAsync(update.Message.Chat,$"Welcome, {executor.FirstName}!", replyMarkup: _keyboard);
                return;
            }
            
            // Crank code
            if (!_executorCommands.Contains(update.Message.Text, StringComparer.OrdinalIgnoreCase) && update.Message.Text != "Back")
            {
                await _telegram.SendTextMessageAsync(update.Message.Chat,$"Wrong command", replyMarkup: _keyboard);
            }
            
            switch (update.Message.Text)
            {
                case "View my orders":
                {
                    var orders = await _executorMenuHelper.ViewMyOrders(executor);

                    if (!orders.Any())
                    {
                        await _telegram.SendTextMessageAsync(update.Message.Chat, "You have no active orders");
                        return;
                    }

                    await _buttons.ViewOrderButtons(orders, update.Message.Chat,"Your orders to complete");
                    break;
                }
                case "Edit my position":
                {
                    await SendEditPosition(update);
                    break;
                }
                case "View available orders":
                {
                    var orders = await _executorMenuHelper.ViewFreeOrders(executor);
                    
                    if (!orders.Any())
                    {
                        await _telegram.SendTextMessageAsync(update.Message.Chat, "No available orders!");
                        return;
                    }

                    await _buttons.ViewOrderButtons(orders, update.Message.Chat,"All available orders!");
                    break;
                }
                default:
                {
                    await _telegram.SendTextMessageAsync(update.Message.Chat,$"Welcome, {executor.FirstName}!", replyMarkup: _keyboard);
                    break;
                }
            }
        }
        
        //todo: implement
        public async Task ProcessCallBack(Executor executor, Update update)
        {
            var backKey = KeyboardMessages.DefaultKeyboardMessage(new[] {"Back"});
            var chat = update.CallbackQuery.Message.Chat;
            if (!update.CallbackQuery.Data.Contains(':'))
            {
                if (update.CallbackQuery.Data == "Back")
                {
                    await _telegram.SendTextMessageAsync(chat, "Back", replyMarkup: _keyboard);
                    return;
                }
                   
                var thisOrder = await _orderStorage.GetOrderAsync(x => x.Id.ToString() == update.CallbackQuery.Data);
                
                var mapped = _mapper.Map<BotShortOrderModel>(thisOrder);
                var orderString = $"Project Name: {mapped.ProjectName}\n" +
                                  $"Client Name: {mapped.ClientName}\n" +
                                  $"Client Phone: {mapped.ClientPhone}\n" +
                                  $"Created at: {mapped.CreationDate}\n" +
                                  $"Due to: {mapped.ScheduledTime}\n" +
                                  $"State: {Enum.GetName(thisOrder.StateEnum)}\n";
                var filePath = thisOrder.Project.ProjectFilePath;
                
                if (System.IO.File.Exists(filePath) && Uri.TryCreate(thisOrder.Project.ProjectLink, UriKind.Absolute, out _) )
                {
                    await using var stream = System.IO.File.Open(filePath, FileMode.Open);
                    var inputOnlineFile = new InputOnlineFile(stream) {FileName = Path.GetFileName(filePath)};
                    orderString += $"Link: {thisOrder.Project.ProjectLink}\n";
                    await _telegram.SendDocumentAsync(chat, inputOnlineFile,"Technical task");
                }
                
                await _telegram.SendTextMessageAsync(chat, orderString);
                
                if (thisOrder.ExecutorId == executor.Id)
                {
                    var dictKeys = new ConcurrentDictionary<string, string>();
                    dictKeys.TryAdd("Edit State", $"state:{thisOrder.Id}");
                    dictKeys.TryAdd("Edit End date", $"date:{thisOrder.Id}");
                    dictKeys.TryAdd("Back", "Back");
                    var editKeys = KeyboardMessages.DefaultInlineKeyboardMessage(dictKeys);
                    await _telegram.SendContactAsync(chat, mapped.ClientPhone, mapped.ClientName, replyMarkup : editKeys);
                }
                else
                {
                    var keys = await _executorMenuHelper.ConstructConfirmOrderButtons(thisOrder);
                    await _telegram.SendTextMessageAsync(chat, "Do you confirm this order?", replyMarkup: keys);
                }
            }
            else
            {
                var answerId = update.CallbackQuery.Data.Split(':');

                var id = answerId[^1];
                
                var thisOrder = await _orderStorage.GetOrderAsync(x => x.Id.ToString() == id);

                switch (answerId[0])
                {
                    case "yes":
                    {
                        thisOrder.ExecutorId = executor.Id;
                        await _orderStorage.UpdateAsync(thisOrder);
                        await _telegram.SendTextMessageAsync(chat,
                            "Successfully confirmed!\n\nPlease contact client at:");
                        await _telegram.SendTextMessageAsync(thisOrder.Client.ChatId,
                            $"Your order for {thisOrder.Project.ProjectName}" +
                            $" has been picked up by {executor.FirstName}!\n\nPlease contact him at:");
                        await _telegram.SendContactAsync(thisOrder.Client.ChatId, executor.PhoneNumber,
                            executor.FirstName);

                        await _telegram.SendContactAsync(chat, thisOrder.Client.PhoneNumber, thisOrder.Client.FirstName,
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
                        var states = await _executorMenuHelper.ConstructStatesButtons(update, thisOrder);
                        await _telegram.SendTextMessageAsync(chat, "Choose new order state", replyMarkup: states);
                        break;
                    }
                    case "date" :
                    {
                        //todo: implement
                        var dtfi = CultureInfo.GetCultureInfo("en-US").DateTimeFormat;
                        var calendarMarkup = KeyboardMessages.Calendar(DateTime.Today, dtfi, thisOrder);

                        await _telegram.SendTextMessageAsync(
                            chat,
                            "Pick date:",
                            replyMarkup: calendarMarkup
                        );
                        break;
                    }
                    case "pck":
                    {
                        var date = DateTimeOffset.Parse(answerId[1]);
                        await _telegram.SendTextMessageAsync(chat, $"Changed date\nfrom {thisOrder.ScheduledTime}\n" +
                                                                   $"to {date}", replyMarkup: _keyboard);

                        await _telegram.SendTextMessageAsync(thisOrder.Client.ChatId,
                            $"Your order for project {thisOrder.Project.ProjectName}\n" +
                            $"has been changed from {thisOrder.ScheduledTime} to {date}");

                        thisOrder.ScheduledTime = date;
                        await _orderStorage.UpdateAsync(thisOrder);

                        return;
                    }
                }

                if (Enum.TryParse<OrderStateEnum>(answerId[0], out var newState))
                {
                    await _telegram.SendTextMessageAsync(thisOrder.Client.ChatId, "State of your order on\n" +
                        $"project {thisOrder.Project.ProjectName} was changed from {Enum.GetName(thisOrder.StateEnum)} " +
                        $"to {newState}");
                    await _telegram.SendTextMessageAsync(chat,
                        $"Success! Changed state from {thisOrder.StateEnum}" +
                        $"to {newState}",replyMarkup:_keyboard);
                    
                    thisOrder.StateEnum = newState;
                    await _orderStorage.UpdateAsync(thisOrder);  
                }
            }
        }
        private async Task<bool> EditPositionMenu(Update update)
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

        private async Task SendEditPosition(Update update)
        {
            var keys = Enum.GetNames(typeof(PositionsEnum))
                .Where(x=> x != Enum.GetName(PositionsEnum.Unknown)).ToList();

            var keyboard = KeyboardMessages.DefaultKeyboardMessage(keys);

            await _telegram.SendTextMessageAsync(update.Message.Chat, "Please choose your position",
                replyMarkup: keyboard);
        }
        private async Task<bool> EditExecutorPosition(Message message)
        {
            var thisExecutor = await _executorStorage.GetExecutorAsync(x => x.TelegramId == message.From.Id);

            if (!Enum.TryParse<PositionsEnum>(message.Text, out var parsedPosition)) return false;
            thisExecutor.Position = parsedPosition;
            await _executorStorage.UpdateExecutorAsync(thisExecutor);
            await _telegram.SendTextMessageAsync(message.Chat, $"Success! Your position is {parsedPosition}");
            return true;
        }
    }
}