using AutoMapper;
using ExchangeRates.Core.Entities;
using ExchangeRates.Core.Interfaces;
using ExchangeRates.Infrastructure.DTOs;
using ExchangeRates.Infrastructure.Helpers;
using ExchangeRates.Infrastructure.Services.Models;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace ExchangeRates.Infrastructure.Services
{
    public sealed class NbRBService : INbRBService
    {
        private readonly IHttpClientFactory _factory;
        private readonly NbRBOptions _options;
        private readonly IMapper _mapper;

        public NbRBService(IHttpClientFactory factory,
            IMapper mapper,
            IOptions<NbRBOptions> config)
        {
            _options = config.Value;
            _factory = factory;
            _mapper = mapper;
        }

        public async Task<IEnumerable<Currency>> GetCurrenciesAsync()
        {
            var client = _factory.CreateClient();
            client.BaseAddress = new Uri(_options.Uri);
            var response = await client.GetAsync("currencies");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var json = JsonSerializer.Deserialize<IEnumerable<NbRBCurrency>>(content);

            return _mapper.Map<IEnumerable<Currency>>(json);
        }

        public async Task<Currency> GetCurrencyAsync(int curId)
        {
            var client = _factory.CreateClient();
            client.BaseAddress = new Uri(_options.Uri);

            var response = await client.GetAsync($"currencies/{curId}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var json = JsonSerializer.Deserialize<NbRBCurrency>(content);

            return _mapper.Map<Currency>(json);
        }

        public async Task<IEnumerable<Rate>> GetRateInPeriodAsync(string currency, DateTime startDate, DateTime endDate)
        {
            if (!_options.Currencies.Contains(currency))
            {
                throw new ArgumentException("Invalid type of currencies");
            }

            if (startDate > endDate)
            {
                throw new ArgumentException("StartDate cann't be more then EndDate");
            }

            startDate = new DateTime(startDate.Year, startDate.Month, startDate.Day, 0, 0, 0);
            endDate = new DateTime(endDate.Year, endDate.Month, endDate.Day, 0, 0, 0);
            var currencies = await GetCurrenciesAsync();

            var searchedCurrencies = currencies.Where(c => c.Cur_Abbreviation.Equals(currency, StringComparison.InvariantCultureIgnoreCase)).ToList();

            var intervals = new List<NbrbInterval>();

            foreach (var cur in searchedCurrencies)
            {
                if (startDate >= cur.Cur_DateStart && startDate <= cur.Cur_DateEnd)
                {
                    while (startDate <= endDate && startDate <= cur.Cur_DateEnd)
                    {
                        var intervalStart = startDate;
                        var intervalEnd = startDate;
                        var endOfMonth = startDate.ToLastDayInMonth();
                        if (cur.Cur_DateEnd <= endDate)
                        {
                            if (endOfMonth <= cur.Cur_DateEnd)
                            {
                                intervalEnd = endOfMonth;
                            }
                            else
                            {
                                intervalEnd = cur.Cur_DateEnd;
                            }
                        }
                        else
                        {
                            if (endOfMonth <= endDate)
                            {
                                intervalEnd = endOfMonth;
                            }
                            else
                            {
                                intervalEnd = endDate;
                            }
                        }
                        startDate = intervalEnd.AddDays(1);
                        intervals.Add(new NbrbInterval(cur.Cur_ID, intervalStart, intervalEnd));
                    }
                }
            }

            var client = _factory.CreateClient();
            client.BaseAddress = new Uri(_options.Uri);
            var rates = new List<Rate>();
            foreach (var interval in intervals)
            {
                var response = await client.GetAsync($"rates/dynamics/{interval.Cur_ID}?startDate={interval.Start:yyyy-MM-dd}&enddate={interval.End:yyyy-MM-dd}");
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                var json = JsonSerializer.Deserialize<IEnumerable<NbRBRateShort>>(content);
                var responseRates = _mapper.Map<IEnumerable<Rate>>(json);
                var rateAmount = searchedCurrencies.FirstOrDefault(x => x.Cur_ID == interval.Cur_ID).Cur_Scale;
                foreach (var item in responseRates)
                {
                    item.Amount = rateAmount;
                    item.Currency = currency;
                }
                rates.AddRange(responseRates);
            }

            return rates;
        }
    }
}
