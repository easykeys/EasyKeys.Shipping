﻿namespace EasyKeys.Shipping.Abstractions;

public static class DateTimeExtensions
{
    public static DateTime AddBusinessDays(this DateTime date, int days)
    {
        if (days < 0)
        {
            throw new ArgumentException("days cannot be negative", "days");
        }

        if (days == 0)
        {
            return date;
        }

        if (date.DayOfWeek == DayOfWeek.Saturday)
        {
            date = date.AddDays(2);
            days--;
        }
        else if (date.DayOfWeek == DayOfWeek.Sunday)
        {
            date = date.AddDays(1);
            days--;
        }

        date = date.AddDays(days / 5 * 7);
        var extraDays = days % 5;

        if ((int)date.DayOfWeek + extraDays > 5)
        {
            extraDays += 2;
        }

        return date.AddDays(extraDays);
    }
}
