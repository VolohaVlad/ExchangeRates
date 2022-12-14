using System.Collections.Generic;

namespace ExchangeRates.Infrastructure.Options
{
    public sealed class NbRBOptions
    {
        public const string SectionName = "NbRBApi";

        public string Uri { get; set; }

        public IList<string> Currencies { get; set; } = new List<string>();
    }
}
