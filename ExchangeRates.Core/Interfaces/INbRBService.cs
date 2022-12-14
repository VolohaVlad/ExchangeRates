using ExchangeRates.Core.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExchangeRates.Core.Interfaces
{
    public interface INbRBService
    {
        Task<IEnumerable<Currency>> GetCurrenciesAsync();
        Task<Currency> GetCurrencyAsync(int curId);
        Task<IEnumerable<Rate>> GetRateInPeriodAsync(string currency, DateTime start, DateTime end);
    }
}
