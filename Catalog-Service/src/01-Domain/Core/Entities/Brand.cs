using Catalog_Service.src._01_Domain.Core.Primitives;

namespace Catalog_Service.src._01_Domain.Core.Entities
{
    public class Brand : AggregateRoot
    {
        private readonly List<Product> _products = new();

        public string Name { get; private set; }
        public string Description { get; private set; }
        public Slug Slug { get; private set; }
        public string? LogoUrl { get; private set; }
        public string? WebsiteUrl { get; private set; }
        public bool IsActive { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }
        public string? MetaTitle { get; private set; }
        public string? MetaDescription { get; private set; }

        // Navigation properties
        public IReadOnlyCollection<Product> Products => _products.AsReadOnly();

        // For EF Core
        protected Brand() { }

        public Brand(
            string name,
            string description,
            string? logoUrl = null,
            string? websiteUrl = null,
            string? metaTitle = null,
            string? metaDescription = null)
        {
            Name = name;
            Description = description;
            LogoUrl = logoUrl;
            WebsiteUrl = websiteUrl;
            IsActive = true;
            CreatedAt = DateTime.UtcNow;
            MetaTitle = metaTitle;
            MetaDescription = metaDescription;
        }

        public void UpdateDetails(
            string name,
            string description,
            string? logoUrl = null,
            string? websiteUrl = null,
            string? metaTitle = null,
            string? metaDescription = null)
        {
            Name = name;
            Description = description;
            LogoUrl = logoUrl;
            WebsiteUrl = websiteUrl;
            MetaTitle = metaTitle;
            MetaDescription = metaDescription;
            UpdatedAt = DateTime.UtcNow;
        }

        public void SetSlug(Slug slug)
        {
            Slug = slug;
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
    }
}

