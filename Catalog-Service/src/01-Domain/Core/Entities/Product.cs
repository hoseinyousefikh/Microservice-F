using Catalog_Service.src._01_Domain.Core.Enums;
using Catalog_Service.src._01_Domain.Core.Primitives;

namespace Catalog_Service.src._01_Domain.Core.Entities
{
    public class Product : AggregateRoot
    {
        private readonly List<ProductVariant> _variants = new();
        private readonly List<ProductReview> _reviews = new();
        private readonly List<ProductTag> _tags = new();
        private readonly List<ImageResource> _images = new();
        private readonly List<ProductAttribute> _attributes = new();

        public string Name { get; private set; }
        public string Description { get; private set; }
        public Slug Slug { get; private set; }
        public Money Price { get; private set; }
        public Money? OriginalPrice { get; private set; }
        public ProductStatus Status { get; private set; }
        public int BrandId { get; private set; }
        public int CategoryId { get; private set; }
        public int ViewCount { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }
        public DateTime? PublishedAt { get; private set; }
        public Dimensions Dimensions { get; private set; }
        public Weight Weight { get; private set; }
        public bool IsFeatured { get; private set; }
        public string Sku { get; private set; }
        public string? MetaTitle { get; private set; }
        public string? MetaDescription { get; private set; }
        public int StockQuantity { get; private set; }
        public StockStatus StockStatus { get; private set; }
        public string CreatedByUserId { get; private set; }

        // Navigation properties
        public Brand Brand { get; private set; }
        public Category Category { get; private set; }
        public IReadOnlyCollection<ProductVariant> Variants => _variants.AsReadOnly();
        public IReadOnlyCollection<ProductReview> Reviews => _reviews.AsReadOnly();
        public IReadOnlyCollection<ProductTag> Tags => _tags.AsReadOnly();
        public IReadOnlyCollection<ImageResource> Images => _images.AsReadOnly();
        public IReadOnlyCollection<ProductAttribute> Attributes => _attributes.AsReadOnly();

        // For EF Core
        protected Product() { }

        public Product(
            string name,
            string description,
            Money price,
            int brandId,
            int categoryId,
            string sku,
            Dimensions dimensions,
            Weight weight,
            string createdByUserId,
            string? metaTitle = null,
            string? metaDescription = null)
        {
            Name = name;
            Description = description;
            Price = price;
            BrandId = brandId;
            CategoryId = categoryId;
            Sku = sku;
            Dimensions = dimensions;
            Weight = weight;
            CreatedByUserId = createdByUserId;
            MetaTitle = metaTitle;
            MetaDescription = metaDescription;
            Status = ProductStatus.Draft;
            CreatedAt = DateTime.UtcNow;
            StockQuantity = 0;
            StockStatus = StockStatus.OutOfStock;
            IsFeatured = false;
            ViewCount = 0;
        }

        public void UpdateDetails(
      string name,
      string description,
      Money price,
      Money? originalPrice,
      Dimensions dimensions,
      Weight weight,
      string? metaTitle = null,
      string? metaDescription = null)
        {
            Name = name;
            Description = description;
            Price = price;
            OriginalPrice = originalPrice;
            Dimensions = dimensions;
            Weight = weight;
            MetaTitle = metaTitle;
            MetaDescription = metaDescription;
            UpdatedAt = DateTime.UtcNow;
        }

        public void SetSlug(Slug slug)
        {
            Slug = slug;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Publish()
        {
            if (Status == ProductStatus.Published) return;

            Status = ProductStatus.Published;
            PublishedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Unpublish()
        {
            if (Status == ProductStatus.Draft) return;

            Status = ProductStatus.Draft;
            PublishedAt = null;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Archive()
        {
            Status = ProductStatus.Archived;
            UpdatedAt = DateTime.UtcNow;
        }

        public void SetAsFeatured()
        {
            IsFeatured = true;
            UpdatedAt = DateTime.UtcNow;
        }

        public void RemoveFromFeatured()
        {
            IsFeatured = false;
            UpdatedAt = DateTime.UtcNow;
        }

        public void IncrementViewCount()
        {
            ViewCount++;
        }

        public void UpdateStock(int quantity)
        {
            if (quantity < 0)
                throw new ArgumentException("Stock quantity cannot be negative", nameof(quantity));

            StockQuantity = quantity;
            StockStatus = quantity > 0 ? StockStatus.InStock : StockStatus.OutOfStock;
            UpdatedAt = DateTime.UtcNow;
        }

        public void AddVariant(ProductVariant variant)
        {
            _variants.Add(variant);
            UpdatedAt = DateTime.UtcNow;
        }

        public void RemoveVariant(ProductVariant variant)
        {
            _variants.Remove(variant);
            UpdatedAt = DateTime.UtcNow;
        }

        public void AddReview(ProductReview review)
        {
            _reviews.Add(review);
            UpdatedAt = DateTime.UtcNow;
        }

        public void AddTag(ProductTag tag)
        {
            if (!_tags.Exists(t => t.TagText.Equals(tag.TagText, StringComparison.OrdinalIgnoreCase)))
            {
                _tags.Add(tag);
                UpdatedAt = DateTime.UtcNow;
            }
        }

        public void RemoveTag(ProductTag tag)
        {
            _tags.Remove(tag);
            UpdatedAt = DateTime.UtcNow;
        }

        public void AddImage(ImageResource image)
        {
            _images.Add(image);
            UpdatedAt = DateTime.UtcNow;
        }

        public void RemoveImage(ImageResource image)
        {
            _images.Remove(image);
            UpdatedAt = DateTime.UtcNow;
        }

        public void AddAttribute(ProductAttribute attribute)
        {
            _attributes.Add(attribute);
            UpdatedAt = DateTime.UtcNow;
        }
        public void SetStockStatus(StockStatus status)
        {
            StockStatus = status;
            UpdatedAt = DateTime.UtcNow;
        }
        public void RemoveAttribute(ProductAttribute attribute)
        {
            _attributes.Remove(attribute);
            UpdatedAt = DateTime.UtcNow;
        }
    }
}