using Catalog_Service.src._01_Domain.Core.Enums;
using Catalog_Service.src._01_Domain.Core.Primitives;

namespace Catalog_Service.src._01_Domain.Core.Entities
{
    public class ProductVariant : Entity
    {
        private readonly List<ProductAttribute> _attributes = new();

        public int ProductId { get; private set; }
        public string Sku { get; private set; }
        public string Name { get; private set; }
        public Money Price { get; private set; }
        public Money? OriginalPrice { get; private set; }
        public int StockQuantity { get; private set; }
        public StockStatus StockStatus { get; private set; }
        public Dimensions Dimensions { get; private set; }
        public Weight Weight { get; private set; }
        public string? ImageUrl { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }
        public bool IsActive { get; private set; }

        // Navigation properties
        public Product Product { get; private set; }
        public IReadOnlyCollection<ProductAttribute> Attributes => _attributes.AsReadOnly();

        // For EF Core
        protected ProductVariant() { }

        public ProductVariant(
            int productId,
            string sku,
            string name,
            Money price,
            Dimensions dimensions,
            Weight weight,
            string? imageUrl = null,
            Money? originalPrice = null)
        {
            ProductId = productId;
            Sku = sku;
            Name = name;
            Price = price;
            OriginalPrice = originalPrice;
            Dimensions = dimensions;
            Weight = weight;
            ImageUrl = imageUrl;
            CreatedAt = DateTime.UtcNow;
            IsActive = true;
            StockQuantity = 0;
            StockStatus = StockStatus.OutOfStock;
        }

        public void UpdateDetails(
            string name,
            Money price,
            Money? originalPrice,
            Dimensions dimensions,
            Weight weight,
            string? imageUrl = null)
        {
            Name = name;
            Price = price;
            OriginalPrice = originalPrice;
            Dimensions = dimensions;
            Weight = weight;
            ImageUrl = imageUrl;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateStock(int quantity)
        {
            if (quantity < 0)
                throw new ArgumentException("Stock quantity cannot be negative", nameof(quantity));

            StockQuantity = quantity;
            StockStatus = quantity > 0 ? StockStatus.InStock : StockStatus.OutOfStock;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Activate()
        {
            if (IsActive) return;

            IsActive = true;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Deactivate()
        {
            if (!IsActive) return;

            IsActive = false;
            UpdatedAt = DateTime.UtcNow;
        }

        public void AddAttribute(ProductAttribute attribute)
        {
            _attributes.Add(attribute);
            UpdatedAt = DateTime.UtcNow;
        }

        public void RemoveAttribute(ProductAttribute attribute)
        {
            _attributes.Remove(attribute);
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
