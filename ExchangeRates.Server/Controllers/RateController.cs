using ExchangeRates.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace ExchangeRates.Server.Controllers
{
    [ApiController]
    [Route("api/rate")]
    public sealed class RateController : ControllerBase
    {
        private readonly IRateCacheService _cacheService;

        public RateController(IRateCacheService cacheService)
        {
            _cacheService = cacheService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCurrencyRateAsync(string currency, DateTime startDate, DateTime endDate)
        {
            ValidateInterval(startDate, endDate);
            var rates = await _cacheService.GetRateInPeriodAsync(currency, startDate, endDate);

            return Ok(rates);
        }

        private void ValidateInterval(DateTime startDate, DateTime endDate)
        {
            if (startDate >= endDate)
            {
                throw new ValidationException("Start Date can be less then End Date");
            }

            if (endDate < DateTime.Now.AddYears(-5).AddDays(-1) ||
                startDate < DateTime.Now.AddYears(-5).AddDays(-1))
            {
                throw new ValidationException("Please enter dates between the last 5 years");
            }
        }
    }
}
