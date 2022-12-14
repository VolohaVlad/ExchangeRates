using AutoMapper;
using ExchangeRates.Core.Entities;
using ExchangeRates.Core.Interfaces;
using ExchangeRates.Infrastructure.DTOs;
using ExchangeRates.Infrastructure.Helpers;
using ExchangeRates.Infrastructure.Options;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace ExchangeRates.Infrastructure.Services
{
    public sealed class BitcoinService : IBitcoinService
    {
        private readonly IHttpClientFactory _factory;
        private readonly BitcoinOptions _options;
        private readonly IMapper _mapper;

        public BitcoinService(IHttpClientFactory factory,
            IMapper mapper,
            IOptions<BitcoinOptions> config)
        {
            _options = config.Value;
            _factory = factory;
            _mapper = mapper;
        }

        public async Task<IEnumerable<Rate>> GetRatesInPeriodAsync(string currency, DateTime startDate, DateTime endDate)
        {
            if (!_options.Currencies.TryGetValue(currency, out var currencyString))
            {
                throw new ArgumentException("Invalid type of currencies");
            }

            if (startDate > endDate)
            {
                throw new ArgumentException("StartDate cann't be more then EndDate");
            }

            var client = _factory.CreateClient();
            client.BaseAddress = new Uri(_options.Uri.Replace("__currency__", currencyString));

            var response = await client.GetAsync($"?interval=d1&start={startDate.ToUnixTimeMilliseconds()}&end={endDate.ToUnixTimeMilliseconds()}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var json = JsonSerializer.Deserialize<CoinHistory>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var rates = _mapper.Map<IEnumerable<Rate>>(json.Data);
            foreach (var item in rates)
            {
                item.Currency = currency;
                item.Amount = 1;
            }

            return rates;
        }
    }
}
