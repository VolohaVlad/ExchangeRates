namespace ExchangeRates.Infrastructure.Options
{
    public sealed class JsonRepoOptions
    {
        public const string SectionName = "JsonRepo";

        public string Path { get; set; }

        public string FileName { get; set; }
    }
}
