namespace Catalog_Service.src._01_Domain.Core.Entities
{
    public class ProductAttribute : Entity
    {
        public int ProductId { get; private set; }
        public int? ProductVariantId { get; private set; }
        public string Name { get; private set; }
        public string Value { get; private set; }
        public bool IsVariantSpecific { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }

        // Navigation properties
        public Product Product { get; private set; }
        public ProductVariant ProductVariant { get; private set; }

        // For EF Core
        protected ProductAttribute() { }

        public ProductAttribute(
            int productId,
            string name,
            string value,
            int? productVariantId = null,
            bool isVariantSpecific = false)
        {
            ProductId = productId;
            Name = name;
            Value = value;
            ProductVariantId = productVariantId;
            IsVariantSpecific = isVariantSpecific;
            CreatedAt = DateTime.UtcNow;
        }

        public void UpdateValue(string value)
        {
            Value = value;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateName(string name)
        {
            Name = name;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
