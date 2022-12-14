using ExchangeRates.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExchangeRates.Core.Interfaces
{
    public interface IRateRepository
    {
        Task SetAsync(IEnumerable<Rate> entity);

        Task<IEnumerable<Rate>> GetAsync();

        Task AppendAsync(Rate entity);

        Task AppendAsync(IEnumerable<Rate> entity);
    }
}
