using SharedKernel.Domain.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedKernel.Application.Abstractions
{
    public interface IEventBus
    {
        Task PublishAsync<T>(T @event) where T : IDomainEvent;
        Task SubscribeAsync<T>(Func<T, Task> handler) where T : IDomainEvent;
        Task UnsubscribeAsync<T>(Func<T, Task> handler) where T : IDomainEvent;
    }
}
