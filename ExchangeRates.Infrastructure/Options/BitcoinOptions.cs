using System.Collections.Generic;

namespace ExchangeRates.Infrastructure.Options
{
    public sealed class BitcoinOptions
    {
        public const string SectionName = "BitcoinApi";

        public string Uri { get; set; }

        public IDictionary<string, string> Currencies { get; set; } = new Dictionary<string, string>();
    }
}
