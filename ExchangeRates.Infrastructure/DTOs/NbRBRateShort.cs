using System;
using System.ComponentModel.DataAnnotations;

namespace ExchangeRates.Infrastructure.DTOs
{
    public sealed class NbRBRateShort
    {
        public int Cur_ID { get; set; }
        [Key]
        public DateTime Date { get; set; }
        public decimal? Cur_OfficialRate { get; set; }
    }
}