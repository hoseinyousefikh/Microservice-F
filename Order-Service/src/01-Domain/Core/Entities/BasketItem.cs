using Order_Service.src._01_Domain.Core.Common;

namespace Order_Service.src._01_Domain.Core.Entities
{
    public class BasketItem : BaseEntity
    {
        public Guid ProductId { get; private set; }
        public string ProductName { get; private set; }
        public int Quantity { get; private set; }

        // Parameterless constructor for EF Core
        private BasketItem() : base() { }

        public BasketItem(Guid id, Guid productId, string productName, int quantity) : base(id)
        {
            if (string.IsNullOrWhiteSpace(productName))
                throw new ArgumentException("Product name cannot be empty.", nameof(productName));
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));

            ProductId = productId;
            ProductName = productName;
            Quantity = quantity;
        }

        public void UpdateQuantity(int newQuantity)
        {
            if (newQuantity <= 0)
            {
                throw new ArgumentException("Quantity must be greater than zero.", nameof(newQuantity));
            }
            Quantity = newQuantity;
            UpdateTimestamp();
        }
    }
}
