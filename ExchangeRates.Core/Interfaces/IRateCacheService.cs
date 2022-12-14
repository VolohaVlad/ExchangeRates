using ExchangeRates.Core.Entities;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace ExchangeRates.Core.Interfaces
{
    public interface IRateCacheService
    {
        Task<IEnumerable<Rate>> GetRateInPeriodAsync(string currency, DateTime start, DateTime end);
    }
}
