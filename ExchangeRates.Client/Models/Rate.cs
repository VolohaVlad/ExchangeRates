namespace ExchangeRates.Client.Models
{
    public sealed class Rate
    {
        public string Currency { get; set; }

        public string Date { get; set; }

        public decimal? Value { get; set; }

        public int Amount { get; set; }

    }
}
