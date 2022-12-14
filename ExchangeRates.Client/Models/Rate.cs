using System;

namespace ExchangeRates.Client.Models
{
    public sealed class Rate
    {
        public string Currency { get; set; }

        public DateTime Date { get; set; }

        public double Value { get; set; }

        public int Amount { get; set; }

    }
}
