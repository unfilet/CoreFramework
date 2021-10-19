using System;
public static class DateTimeExtensions
{
    public static DateTime ResetTimeToStartOfDay(this DateTime dateTime)
        => new DateTime(
         dateTime.Year,
         dateTime.Month,
         dateTime.Day,
         0, 0, 0, 0);

    public static DateTime ResetTimeToEndOfDay(this DateTime dateTime)
        => new DateTime(
         dateTime.Year,
         dateTime.Month,
         dateTime.Day,
         23, 59, 59, 999);

    public static DateTime Tomorrow(this DateTime dateTime)
        => dateTime.ResetTimeToStartOfDay().AddDays(1);

    public static DateTime Yesterday(this DateTime dateTime)
        => dateTime.ResetTimeToStartOfDay().AddDays(-1);

    public static DateTime Min(DateTime left, DateTime right)
        => left < right ? left : right;

    public static DateTime Max(DateTime left, DateTime right)
        => left < right ? right : left;
}
