using Catalog_Service.src._01_Domain.Core.Primitives;

namespace Catalog_Service.src._01_Domain.Core.Entities
{
    public class Category : AggregateRoot
    {
        private readonly List<Product> _products = new();
        private readonly List<Category> _subCategories = new();

        public string Name { get; private set; }
        public string Description { get; private set; }
        public Slug Slug { get; private set; }
        public int? ParentCategoryId { get; private set; }
        public int DisplayOrder { get; private set; }
        public string? ImageUrl { get; private set; }
        public bool IsActive { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }
        public string? MetaTitle { get; private set; }
        public string? MetaDescription { get; private set; }

        // Navigation properties
        public Category ParentCategory { get; private set; }
        public IReadOnlyCollection<Category> SubCategories => _subCategories.AsReadOnly();
        public IReadOnlyCollection<Product> Products => _products.AsReadOnly();

        // For EF Core
        protected Category() { }

        public Category(
            string name,
            string description,
            int displayOrder,
            int? parentCategoryId = null,
            string? imageUrl = null,
            string? metaTitle = null,
            string? metaDescription = null)
        {
            Name = name;
            Description = description;
            DisplayOrder = displayOrder;
            ParentCategoryId = parentCategoryId;
            ImageUrl = imageUrl;
            IsActive = true;
            CreatedAt = DateTime.UtcNow;
            MetaTitle = metaTitle;
            MetaDescription = metaDescription;
        }

        public void UpdateDetails(
            string name,
            string description,
            int displayOrder,
            string? imageUrl = null,
            string? metaTitle = null,
            string? metaDescription = null)
        {
            Name = name;
            Description = description;
            DisplayOrder = displayOrder;
            ImageUrl = imageUrl;
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

        public void SetParentCategory(int? parentCategoryId)
        {
            ParentCategoryId = parentCategoryId;
            UpdatedAt = DateTime.UtcNow;
        }

        public void AddSubCategory(Category category)
        {
            _subCategories.Add(category);
            UpdatedAt = DateTime.UtcNow;
        }

        public void RemoveSubCategory(Category category)
        {
            _subCategories.Remove(category);
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
