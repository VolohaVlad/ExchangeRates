using System.Collections.Generic;

namespace ExchangeRates.Infrastructure.DTOs
{
    public sealed class CoinHistory
    {
        public IEnumerable<CoinRate> Data { get; set; }

        public long Timestamp { get; set; }
    }
}
