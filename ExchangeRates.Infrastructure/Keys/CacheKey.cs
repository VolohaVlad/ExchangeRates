using System;

namespace ExchangeRates.Infrastructure.Keys
{
    public sealed class CacheKey
    {
        public const string DateFormat = "MM/dd/yy";

        private readonly string _currency;
        private readonly string _date;

        public CacheKey(string currency, string date)
        {
            if (string.IsNullOrEmpty(currency))
            {
                throw new ArgumentException($"'{nameof(currency)}' cannot be null or empty.", nameof(currency));
            }

            if (string.IsNullOrEmpty(date))
            {
                throw new ArgumentException($"'{nameof(date)}' cannot be null or empty.", nameof(date));
            }

            _currency = currency;
            _date = date;
        }

        public override bool Equals(object obj)
        {
            return obj is CacheKey cacheKey
                && _currency.Equals(cacheKey._currency)
                && _date.Equals(cacheKey._date);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_currency, _date);
        }
    }
}
