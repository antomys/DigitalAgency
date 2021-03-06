using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DigitalAgency.Bll.TelegramBot.Services.Common;
using DigitalAgency.Dal.Entities;
using Telegram.Bot.Types.ReplyMarkups;

namespace DigitalAgency.Bll.TelegramBot.Services
{
    public static class KeyboardMessages
    {
        public static ReplyKeyboardMarkup DefaultKeyboardMessage(IEnumerable<string> commands)
        { 
            var keyboardButtons = commands.Select(command => new[] {new KeyboardButton(command)})
                .Cast<IEnumerable<KeyboardButton>>().ToList();
            return new ReplyKeyboardMarkup
            {
                Keyboard = keyboardButtons,
                ResizeKeyboard = true,
                OneTimeKeyboard = true
            };
        }
        public static InlineKeyboardMarkup DefaultInlineKeyboardMessage(ConcurrentDictionary<string,string> commands)
        {
            var keyboardButtons = commands
                .Select((command) => new[] {InlineKeyboardButton.WithCallbackData(command.Key,command.Value.ToString())})
                .Cast<IEnumerable<InlineKeyboardButton>>().AsEnumerable();
            
            return new InlineKeyboardMarkup(keyboardButtons); 
        }
        public static InlineKeyboardMarkup Calendar(in DateTime date, DateTimeFormatInfo dtfi, Order thisOrder, string additionalPayload = "")
        {
            var keyboardRows = new List<IEnumerable<InlineKeyboardButton>>();

            keyboardRows.Add(Row.Date(date, dtfi));
            keyboardRows.Add(Row.DayOfWeek(dtfi));
            keyboardRows.AddRange(Row.Month(date, dtfi, thisOrder, additionalPayload));

            return new InlineKeyboardMarkup(keyboardRows);
        }
    }
}