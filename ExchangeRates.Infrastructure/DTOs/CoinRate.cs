using System;

namespace ExchangeRates.Infrastructure.DTOs
{
    public sealed class CoinRate
    {
        public string PriceUsd { get; set; }
        public long Time { get; set; }
        public DateTime? Date { get; set; }
        public string Currency { get; set; }
    }
}