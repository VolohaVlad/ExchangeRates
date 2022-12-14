using ExchangeRates.Core.Interfaces;
using ExchangeRates.Infrastructure.Helpers;
using ExchangeRates.Infrastructure.Keys;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace ExchangeRates.Server.Helpers
{
    public class CacheLoader : IHostedService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IRateRepository _repository;

        public CacheLoader(IMemoryCache memoryCache,
            IRateRepository repository)
        {
            _memoryCache = memoryCache;
            _repository = repository;
        }

        public async Task LoadAsync()
        {
            var rates = await _repository.GetAsync();

            foreach (var item in rates)
            {
                _memoryCache.Set(new CacheKey(item.Currency, item.Date.ToCacheKeyFormat()), item);
            }
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            LoadAsync().GetAwaiter();

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
