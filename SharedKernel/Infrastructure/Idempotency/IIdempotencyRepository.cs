using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedKernel.Infrastructure.Idempotency
{
    public interface IIdempotencyRepository
    {
        Task<IdempotentRequest?> GetByRequestHashAsync(string requestHash);
        Task AddAsync(IdempotentRequest request);
        Task<bool> ExistsAsync(string requestHash);
    }
}
