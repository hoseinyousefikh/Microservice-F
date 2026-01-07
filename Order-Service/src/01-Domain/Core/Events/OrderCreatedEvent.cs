using Order_Service.src._01_Domain.Core.Entities;
using Order_Service.src._01_Domain.Core.ValueObjects;

namespace Order_Service.src._01_Domain.Core.Events
{
    public class OrderCreatedEvent : IDomainEvent
    {
        public Guid OrderId { get; }
        public OrderNumber OrderNumber { get; }
        public string UserId { get; }
        public IReadOnlyList<OrderItemDetails> Items { get; }
        public Money TotalPrice { get; }
        public ShippingAddress ShippingAddress { get; }
        public DateTime OccurredOn { get; }

        public OrderCreatedEvent(Order order)
        {
            OrderId = order.Id;
            OrderNumber = order.OrderNumber;
            UserId = order.UserId;
            TotalPrice = order.TotalPrice;
            ShippingAddress = order.ShippingAddress;
            OccurredOn = DateTime.UtcNow;

            Items = order.Items.Select(item => new OrderItemDetails
            {
                ProductId = item.ProductId,
                ProductName = item.ProductName,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice
            }).ToList().AsReadOnly();
        }
    }

    // A DTO to carry item details without exposing the whole entity
    public record OrderItemDetails
    {
        public Guid ProductId { get; init; }
        public string ProductName { get; init; }
        public int Quantity { get; init; }
        public Money UnitPrice { get; init; }
    }
}
