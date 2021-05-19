using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Telegram.Bot.Types.ReplyMarkups;

namespace DigitalAgency.Bll.Services.Bot
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
    }
}