using Order_Service.src._01_Domain.Core.Common;
using Order_Service.src._01_Domain.Core.ValueObjects;

namespace Order_Service.src._01_Domain.Core.Entities
{
    public class OrderItem : BaseEntity
    {
        public Guid ProductId { get; private set; }
        public string ProductName { get; private set; }
        public int Quantity { get; private set; }
        public Money UnitPrice { get; private set; }
        public Money TotalPrice => UnitPrice * Quantity;

        // Parameterless constructor for EF Core
        private OrderItem() : base() { }

        public OrderItem(Guid id, Guid productId, string productName, int quantity, Money unitPrice) : base(id)
        {
            if (string.IsNullOrWhiteSpace(productName))
                throw new ArgumentException("Product name cannot be empty.", nameof(productName));
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));
            if (unitPrice == null)
                throw new ArgumentNullException(nameof(unitPrice));

            ProductId = productId;
            ProductName = productName;
            Quantity = quantity;
            UnitPrice = unitPrice;
        }
    }
}
