using ExchangeRates.Core.Entities;
using ExchangeRates.Core.Interfaces;
using ExchangeRates.Infrastructure.Helpers;
using ExchangeRates.Infrastructure.Keys;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExchangeRates.Infrastructure.Services
{
    public sealed class RateCacheService : IRateCacheService
    {
        private readonly INbRBService _nbRBService;
        private readonly IBitcoinService _bitcoinService;
        private readonly IMemoryCache _memoryCache;
        private readonly IRateRepository _repository;

        public RateCacheService(INbRBService nbRBService,
            IBitcoinService bitcoinService,
            IMemoryCache memoryCache,
            IRateRepository repository)
        {
            _nbRBService = nbRBService;
            _bitcoinService = bitcoinService;
            _memoryCache = memoryCache;
            _repository = repository;
        }

        public async Task<IEnumerable<Rate>> GetRateInPeriodAsync(string currency, DateTime start, DateTime end)
        {
            if (start > end)
            {
                throw new ArgumentException("Start cann't be more then End");
            }

            var result = new List<Rate>();
            var interval = new CacheInterval { Currency = currency };
            while (start <= end)
            {
                var dateKey = start.ToCacheKeyFormat();
                var key = new CacheKey(currency, dateKey);
                if (!_memoryCache.TryGetValue(key, out Rate rate))
                {
                    if (interval.IntervalStart == null)
                    {
                        interval.IntervalStart = start;
                    }

                    interval.IntervalEnd = start;
                }
                else
                {
                    result.Add(rate);
                    if (interval.IntervalStart != null)
                    {
                        List<Rate> newRates;
                        if (currency == "BTC")
                        {
                            newRates = (await _bitcoinService.GetRatesInPeriodAsync(currency, interval.IntervalStart.Value, interval.IntervalEnd.Value)).ToList();
                        }
                        else
                        {
                            newRates = (await _nbRBService.GetRateInPeriodAsync(currency, interval.IntervalStart.Value, interval.IntervalEnd.Value)).ToList();
                        }

                        result.AddRange(newRates);
                        await _repository.AppendAsync(newRates);
                        foreach (var r in newRates)
                        {
                            _memoryCache.Set(new CacheKey(r.Currency, r.Date.ToCacheKeyFormat()), r);
                        }
                    }
                    interval.IntervalStart = null;
                    interval.IntervalEnd = null;
                }
                start = start.AddDays(1);
            }

            if (interval.IntervalStart != null)
            {
                List<Rate> newRates;
                if (currency == "BTC")
                {
                    newRates = (await _bitcoinService.GetRatesInPeriodAsync(currency, interval.IntervalStart.Value, interval.IntervalEnd.Value)).ToList();
                }
                else
                {
                    newRates = (await _nbRBService.GetRateInPeriodAsync(currency, interval.IntervalStart.Value, interval.IntervalEnd.Value)).ToList();
                }
                result.AddRange(newRates);
                await _repository.AppendAsync(newRates);
                foreach (var r in newRates)
                {
                    _memoryCache.Set(new CacheKey(r.Currency, r.Date.ToCacheKeyFormat()), r);
                }
            }

            return result;
        }
    }

    public class CacheInterval
    {
        public string Currency { get; set; }
        public DateTime? IntervalStart { get; set; }
        public DateTime? IntervalEnd { get; set; }
    }
}
