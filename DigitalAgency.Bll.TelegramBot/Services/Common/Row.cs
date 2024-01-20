using System;
using System.Collections.Generic;
using System.Globalization;
using DigitalAgency.Dal.Entities;
using Telegram.Bot.Types.ReplyMarkups;

namespace DigitalAgency.Bll.TelegramBot.Services.Common;

public static class Row
{
    public static IEnumerable<InlineKeyboardButton> Date(in DateTime date, DateTimeFormatInfo dateTimeFormatInfo)
    {
        DateTime newDate = new DateTime(date.Year, date.Month + 1, date.Day);
        return new[] {
            InlineKeyboardButton.WithCallbackData(
                $">> {newDate.ToString("Y", dateTimeFormatInfo)} <<",
                $"{Constants.YearMonthPicker}{newDate.ToString(Constants.DateFormat)}"
            ),
        };
    }


    public static IEnumerable<InlineKeyboardButton> DayOfWeek(DateTimeFormatInfo dtfi)
    {
        InlineKeyboardButton[] dayNames = new InlineKeyboardButton[7];

        int firstDayOfWeek = (int)dtfi.FirstDayOfWeek;
        for (int i = 0; i < 7; i++)
        {
            yield return dtfi.AbbreviatedDayNames[(firstDayOfWeek + i) % 7];
        }
    }

    public static IEnumerable<IEnumerable<InlineKeyboardButton>> Month(DateTime date, DateTimeFormatInfo dtfi, Order thisOrder,
        string additionalPayload = "")
    {
        DateTime firstDayOfMonth = new DateTime(date.Year, date.Month + 1, 1);

        int lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1).Day;

        for (int dayOfMonth = 1, weekNum = 0; dayOfMonth <= lastDayOfMonth; weekNum++)
        {
            yield return NewWeek(weekNum, ref dayOfMonth);
        }

        IEnumerable<InlineKeyboardButton> NewWeek(int weekNum, ref int dayOfMonth)
        {
            InlineKeyboardButton[] week = new InlineKeyboardButton[7];

            for (int dayOfWeek = 0; dayOfWeek < 7; dayOfWeek++)
            {
                if (weekNum == 0 && dayOfWeek < FirstDayOfWeek() || dayOfMonth > lastDayOfMonth)
                {
                    week[dayOfWeek] = " ";
                    continue;
                }

//pck:2021/06/22:14
                week[dayOfWeek] = InlineKeyboardButton.WithCallbackData(
                    dayOfMonth.ToString(),
                    $"{additionalPayload}{Constants.PickDate}{date.Year}/{date.Month + 1}/{dayOfMonth}:{thisOrder.Id}"
                );

                dayOfMonth++;
            }
            return week;

            int FirstDayOfWeek()
            {
                return (7 + (int)firstDayOfMonth.DayOfWeek - (int)dtfi.FirstDayOfWeek) % 7;
            }
        }
    }
}