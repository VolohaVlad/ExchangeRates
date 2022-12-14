using ExchangeRates.Infrastructure.Keys;
using System;

namespace ExchangeRates.Infrastructure.Helpers
{
    public static class DateTimeHelper
    {
        public static long ToUnixTimeMilliseconds(this DateTime dateTime)
        {
            var dateTimeOffset = new DateTimeOffset(dateTime);
            return dateTimeOffset.ToUnixTimeMilliseconds();
        }

        public static DateTime ToLastDayInMonth(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, 1, 0, 0, 0).AddMonths(1).AddDays(-1);
        }

        public static DateTime ToDayStart(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0);
        }

        public static string ToCacheKeyFormat(this DateTime dateTime)
        {
            return dateTime.ToString(CacheKey.DateFormat);
        }
    }
}
