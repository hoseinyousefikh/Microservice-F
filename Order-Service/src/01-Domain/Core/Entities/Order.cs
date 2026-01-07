using Order_Service.src._01_Domain.Core.Common;
using Order_Service.src._01_Domain.Core.Enums;
using Order_Service.src._01_Domain.Core.ValueObjects;

namespace Order_Service.src._01_Domain.Core.Entities
{
    public class Order : BaseEntity
    {
        public string UserId { get; private set; }
        public OrderNumber OrderNumber { get; private set; }
        public OrderStatus Status { get; private set; }
        public PaymentStatus PaymentStatus { get; private set; }
        public ShippingAddress ShippingAddress { get; private set; }
        public Money TotalPrice { get; private set; }

        private readonly List<OrderItem> _items = new();
        public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

        // Parameterless constructor for EF Core
        private Order() : base() { }

        public Order(Guid id, string userId, OrderNumber orderNumber, ShippingAddress shippingAddress) : base(id)
        {
            UserId = userId ?? throw new ArgumentNullException(nameof(userId));
            OrderNumber = orderNumber ?? throw new ArgumentNullException(nameof(orderNumber));
            ShippingAddress = shippingAddress ?? throw new ArgumentNullException(nameof(shippingAddress));
            Status = OrderStatus.Pending;
            PaymentStatus = PaymentStatus.Pending;
            TotalPrice = Money.Zero(); // Initial price
        }

        public void AddItem(OrderItem item)
        {
            if (item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (Status != OrderStatus.Pending)
            {
                throw new InvalidOperationException("Cannot add items to an order that is not in 'Pending' status.");
            }

            _items.Add(item);
            RecalculateTotalPrice();
            UpdateTimestamp();
        }

        public void MarkAsPaid()
        {
            if (Status != OrderStatus.Pending)
            {
                throw new InvalidOperationException("Only pending orders can be marked as paid.");
            }
            PaymentStatus = PaymentStatus.Succeeded;
            UpdateTimestamp();
        }

        public void MarkAsShipped()
        {
            if (PaymentStatus != PaymentStatus.Succeeded)
            {
                throw new InvalidOperationException("Cannot ship an order that has not been paid for.");
            }
            Status = OrderStatus.Shipped;
            UpdateTimestamp();
        }

        public void Cancel()
        {
            if (Status == OrderStatus.Delivered || Status == OrderStatus.Shipped)
            {
                throw new InvalidOperationException("Cannot cancel a delivered or shipped order.");
            }
            Status = OrderStatus.Cancelled;
            UpdateTimestamp();
        }

        private void RecalculateTotalPrice()
        {
            var totalAmount = _items.Sum(item => item.TotalPrice.Amount);
            TotalPrice = Money.FromDecimal(totalAmount, _items.FirstOrDefault()?.UnitPrice.Currency ?? "USD");
        }
    }
}
