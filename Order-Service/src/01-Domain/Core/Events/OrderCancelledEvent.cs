using Order_Service.src._01_Domain.Core.Entities;
using Order_Service.src._01_Domain.Core.ValueObjects;

namespace Order_Service.src._01_Domain.Core.Events
{
    public class OrderCancelledEvent : IDomainEvent
    {
        public Guid OrderId { get; }
        public OrderNumber OrderNumber { get; }
        public string UserId { get; }
        public DateTime OccurredOn { get; }

        public OrderCancelledEvent(Order order)
        {
            OrderId = order.Id;
            OrderNumber = order.OrderNumber;
            UserId = order.UserId;
            OccurredOn = DateTime.UtcNow;
        }
    }
}
