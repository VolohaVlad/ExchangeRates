using ExchangeRates.Core.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExchangeRates.Core.Interfaces
{
    public interface IBitcoinService
    {
        Task<IEnumerable<Rate>> GetRatesInPeriodAsync(string currency, DateTime startDate, DateTime endDate);
    }
}
