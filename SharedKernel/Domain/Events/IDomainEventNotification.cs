using SharedKernel.Domain.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedKernel.Domain.Events
{
    public interface IDomainEventNotification<out T> where T : IDomainEvent
    {
        T DomainEvent { get; }
        DateTime OccurredOn { get; }
    }
}
