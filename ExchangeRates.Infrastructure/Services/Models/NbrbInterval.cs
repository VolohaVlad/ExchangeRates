using System;

namespace ExchangeRates.Infrastructure.Services.Models
{
    public sealed class NbrbInterval
    {
        public NbrbInterval(int cur_ID, DateTime start, DateTime end)
        {
            Cur_ID = cur_ID;
            Start = start;
            End = end;
        }

        public int Cur_ID { get; private set; }
        public DateTime Start { get; private set; }
        public DateTime End { get; private set; }
    }
}
