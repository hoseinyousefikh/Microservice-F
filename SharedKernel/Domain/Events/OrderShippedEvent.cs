using SharedKernel.Domain.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedKernel.Domain.Events
{
    public class OrderShippedEvent : DomainEvent
    {
        public Guid OrderId { get; }
        public DateTime ShippedDate { get; }
        public string TrackingNumber { get; }

        public OrderShippedEvent(Guid orderId, DateTime shippedDate, string trackingNumber)
        {
            OrderId = orderId;
            ShippedDate = shippedDate;
            TrackingNumber = trackingNumber;
        }
    }
}
