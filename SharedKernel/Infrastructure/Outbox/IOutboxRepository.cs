using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedKernel.Infrastructure.Outbox
{
    public interface IOutboxRepository
    {
        Task AddAsync(OutboxMessage message);
        Task<OutboxMessage?> GetByIdAsync(Guid id);
        Task MarkAsProcessedAsync(Guid id, DateTime processedOn);
        Task MarkAsFailedAsync(Guid id, string error);
    }
}
