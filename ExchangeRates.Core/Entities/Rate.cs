using System;

namespace ExchangeRates.Core.Entities
{
    public sealed class Rate
    {
        public string Currency { get; set; }

        public DateTime Date { get; set; }

        public decimal? Value { get; set; }

        public int Amount { get; set; }

        public override bool Equals(object obj)
        {
            return obj is Rate rate
                && rate.Currency.Equals(Currency)
                && rate.Date.Equals(Date)
                && rate.Value.Equals(Value)
                && rate.Amount.Equals(Amount);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Currency, Date, Value, Amount);
        }
    }
}
