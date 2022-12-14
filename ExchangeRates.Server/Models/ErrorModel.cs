namespace ExchangeRates.Server.Models
{
    public sealed class ErrorModel
    {
        public string Code { get; set; }

        public string Message { get; set; }

        public string Field { get; set; }

        public ErrorModel()
        {
        }

        public ErrorModel(string message)
        {
            Message = message;
        }

        public ErrorModel(string field, string message)
        {
            Field = field;
            Message = message;
        }
    }
}
