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
    public class ChangeToHandler : IUpdateHandler
    {
        private readonly DateTimeFormatInfo _locale;

        public ChangeToHandler(ITelegramBotClient telegram)
        {
            _locale = new CultureInfo("en-US", false).DateTimeFormat;
        }

        public static bool CanHandle(IUpdateContext context)
        {
            return
                context.Update.Type == UpdateType.CallbackQuery
                &&
                context.Update.IsCallbackCommand(Constants.ChangeTo);
        }

        public async Task HandleAsync(IUpdateContext context, UpdateDelegate next, CancellationToken cancellationToken)
        {
            if (!DateTime.TryParseExact(
                    context.Update.TrimCallbackCommand(Constants.ChangeTo),
                    Constants.DateFormat,
                    null,
                    DateTimeStyles.None,
                    out var date)
            )
            {
                return;
            }

            var calendarMarkup = KeyboardMessages.Calendar(date, _locale);

            await context.Bot.Client.EditMessageReplyMarkupAsync(
                context.Update.CallbackQuery.Message.Chat.Id,
                context.Update.CallbackQuery.Message.MessageId,
                replyMarkup: calendarMarkup, cancellationToken: cancellationToken);
        }
    }
}
*/
