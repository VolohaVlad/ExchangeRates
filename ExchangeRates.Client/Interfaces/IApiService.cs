using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using ExchangeRates.Client.Models;

namespace ExchangeRates.Client.Interfaces
{
    public interface IApiService
    {
        Task<IEnumerable<Rate>> GetRatesAsync(string currency, DateTime start, DateTime end);
    }
}
