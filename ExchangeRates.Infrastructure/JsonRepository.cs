using ExchangeRates.Core.Entities;
using ExchangeRates.Core.Interfaces;
using ExchangeRates.Infrastructure.Options;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace ExchangeRates.Infrastructure
{
    public sealed class JsonRepository : IRateRepository
    {
        private readonly JsonRepoOptions _option;

        public JsonRepository(IOptions<JsonRepoOptions> config)
        {
            _option = config.Value;
        }

        public async Task<IEnumerable<Rate>> GetAsync()
        {
            if (!Directory.Exists(_option.Path))
            {
                Directory.CreateDirectory(_option.Path);
            }

            var fileInfo = new FileInfo(_option.Path + "/" + _option.FileName);

            if (!fileInfo.Exists)
            {
                using var streamForCreateFile = new FileStream(_option.Path + "/" + _option.FileName, FileMode.OpenOrCreate);
                await JsonSerializer.SerializeAsync<IEnumerable<Rate>>(streamForCreateFile, new List<Rate>());
            }

            using var stream = new FileStream(_option.Path + "/" + _option.FileName, FileMode.OpenOrCreate);
            return await JsonSerializer.DeserializeAsync<IEnumerable<Rate>>(stream);
        }

        public async Task SetAsync(IEnumerable<Rate> entity)
        {
            if (!Directory.Exists(_option.Path))
            {
                Directory.CreateDirectory(_option.Path);
            }

            using var stream = new FileStream(_option.Path + "/" + _option.FileName, FileMode.OpenOrCreate);
            await JsonSerializer.SerializeAsync(stream, entity, new JsonSerializerOptions { WriteIndented = true });
        }

        public async Task AppendAsync(Rate rate)
        {
            var ratesInStorage = (await GetAsync()).ToList();
            if (ratesInStorage.Contains(rate))
            {
                return;
            }

            ratesInStorage.Add(rate);
            await SetAsync(ratesInStorage);
        }

        public async Task AppendAsync(IEnumerable<Rate> rates)
        {
            var ratesInStorage = (await GetAsync()).ToList();
            ratesInStorage.AddRange(rates.Where(rate => !ratesInStorage.Contains(rate)));
            await SetAsync(ratesInStorage);
        }
    }
}
