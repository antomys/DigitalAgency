using System;
using Telegram.Bot.Types;

namespace DigitalAgency.Bll.TelegramBot.Services.Calendar;

public static class CallbackCommandEx
{
    public static bool IsCallbackCommand(this Update update, string command)
    {
        return update.CallbackQuery.Data.StartsWith(
            command,
            StringComparison.Ordinal);
    }

    public static string TrimCallbackCommand(this Update update, string pattern)
    {
        return update.CallbackQuery.Data.Replace(pattern, string.Empty);
    }
}