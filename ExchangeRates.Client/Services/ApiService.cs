using ExchangeRates.Client.Interfaces;
using ExchangeRates.Client.Models;
using ExchangeRates.Client.Options;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace ExchangeRates.Client.Services
{
    public sealed class ApiService : IApiService
    {
        private readonly ApiOptions _options;
        private readonly IHttpClientFactory _factory;

        public ApiService(IHttpClientFactory factory,
            IOptions<ApiOptions> config)
        {
            _options = config.Value;
            _factory = factory;
        }

        public async Task<IEnumerable<Rate>> GetRatesAsync(string currency, DateTime start, DateTime end)
        {
            var client = _factory.CreateClient();
            client.BaseAddress = new Uri(_options.Uri);

            var response = await client.GetAsync($"rate?currency={currency}&startDate={start:yyyy-MM-dd}&endDate={end:yyyy-MM-dd}")
                .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync()
                .ConfigureAwait(false);

            var json = JsonSerializer.Deserialize<IEnumerable<Rate>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true});

            return json;
        }
    }
}
