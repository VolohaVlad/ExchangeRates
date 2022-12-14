using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExchangeRates.Client.ViewModels
{
    public class RateViewModel : ObservableObject
    {
        public string Currency { get; set; }

        public string Date { get; set; }

        public decimal? Value { get; set; }

        public int Amount { get; set; }
    }
}
