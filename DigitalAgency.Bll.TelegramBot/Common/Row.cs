using System;
using System.Collections.Generic;
using System.Globalization;
using DigitalAgency.Dal.Entities;
using Telegram.Bot.Types.ReplyMarkups;

namespace DigitalAgency.Bll.TelegramBot.Common
{
public static class Row
    {
        public static IEnumerable<InlineKeyboardButton> Date(in DateTime date, DateTimeFormatInfo dateTimeFormatInfo)
        {
            var newDate = new DateTime(date.Year, date.Month + 1, date.Day);
            return new[]
            {
                InlineKeyboardButton.WithCallbackData(
                    $">> {newDate.ToString("Y", dateTimeFormatInfo)} <<",
                    $"{Constants.YearMonthPicker}{newDate.ToString(Constants.DateFormat)}"
                )
            };
        }
           

        public static IEnumerable<InlineKeyboardButton> DayOfWeek(DateTimeFormatInfo dtfi)
        {
            var dayNames = new InlineKeyboardButton[7];

            var firstDayOfWeek = (int)dtfi.FirstDayOfWeek;
            for (var i = 0; i < 7; i++)
            {
                yield return dtfi.AbbreviatedDayNames[(firstDayOfWeek + i) % 7];
            }
        }

        public static IEnumerable<IEnumerable<InlineKeyboardButton>> Month(DateTime date, DateTimeFormatInfo dtfi, Order thisOrder)
        {
            var firstDayOfMonth = new DateTime(date.Year, date.Month+1, 1);

            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1).Day;

            for (int dayOfMonth = 1, weekNum = 0; dayOfMonth <= lastDayOfMonth; weekNum++)
            {
                yield return NewWeek(weekNum, ref dayOfMonth);
            }

            IEnumerable<InlineKeyboardButton> NewWeek(int weekNum, ref int dayOfMonth)
            {
                var week = new InlineKeyboardButton[7];

                for (var dayOfWeek = 0; dayOfWeek < 7; dayOfWeek++)
                {
                    if ((weekNum == 0 && dayOfWeek < FirstDayOfWeek())
                       ||
                       dayOfMonth > lastDayOfMonth
                    )
                    {
                        week[dayOfWeek] = " ";
                        continue;
                    }
//pck:2021/06/22:14
                    week[dayOfWeek] = InlineKeyboardButton.WithCallbackData(
                        dayOfMonth.ToString(),
                        $"{Constants.PickDate}{date.Year}/{date.Month+1}/{dayOfMonth}:{thisOrder.Id}"
                    );

                    dayOfMonth++;
                }
                return week;

                int FirstDayOfWeek() =>
                    (7 + (int)firstDayOfMonth.DayOfWeek - (int)dtfi.FirstDayOfWeek) % 7;
            }
        }
    }
}