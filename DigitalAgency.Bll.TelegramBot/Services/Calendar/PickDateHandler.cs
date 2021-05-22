/*
using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using DigitalAgency.Bll.TelegramBot.Common;
using Telegram.Bot;
using Telegram.Bot.Framework.Abstractions;
using Telegram.Bot.Types.Enums;

namespace DigitalAgency.Bll.TelegramBot.Services.Calendar
{
    public class PickDateHandler : IUpdateHandler
    {
        private readonly DateTimeFormatInfo _locale;
        private readonly ITelegramBotClient _telegram;

        public PickDateHandler(ITelegramBotClient telegram)
        {
            _telegram = telegram;
            _locale = new CultureInfo("en-US", false).DateTimeFormat;
        }

        public static bool CanHandle(IUpdateContext context)
        {
            return
                context.Update.Type == UpdateType.CallbackQuery
                &&
                context.Update.IsCallbackCommand(Constants.PickDate);
        }

        public async Task HandleAsync(IUpdateContext context, UpdateDelegate next, CancellationToken cancellationToken)
        {
            if (!DateTime.TryParseExact(
                    context.Update.TrimCallbackCommand(Constants.PickDate),
                    Constants.DateFormat,
                    null,
                    DateTimeStyles.None,
                    out var date)
            )
            {
                return;
            }

            await _telegram.SendTextMessageAsync(
                context.Update.CallbackQuery.Message.Chat.Id,
                date.ToString("d", _locale), cancellationToken: cancellationToken);
        }
    }
}
*/
