using Catalog_Service.src._01_Domain.Core.Contracts.Repositories;
using Catalog_Service.src._01_Domain.Core.Contracts.Services;
using Catalog_Service.src._01_Domain.Core.Entities;
using Catalog_Service.src._01_Domain.Core.Primitives;
using Catalog_Service.src.CrossCutting.Exceptions;

namespace Catalog_Service.src._01_Domain.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IProductRepository _productRepository;
        private readonly ISlugService _slugService;
        private readonly ILogger<CategoryService> _logger;

        public CategoryService(
            ICategoryRepository categoryRepository,
            IProductRepository productRepository,
            ISlugService slugService,
            ILogger<CategoryService> logger)
        {
            _categoryRepository = categoryRepository;
            _productRepository = productRepository;
            _slugService = slugService;
            _logger = logger;
        }

        public async Task<Category> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var category = await _categoryRepository.GetByIdAsync(id, cancellationToken);
            if (category == null)
            {
                _logger.LogWarning("Category with ID {CategoryId} not found", id);
                throw new NotFoundException($"Category with ID {id} not found");
            }
            return category;
        }

        public async Task<Category> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
        {
            var category = await _categoryRepository.GetBySlugAsync(Slug.FromString(slug), cancellationToken);
            if (category == null)
            {
                _logger.LogWarning("Category with slug {CategorySlug} not found", slug);
                throw new NotFoundException($"Category with slug {slug} not found");
            }
            return category;
        }

        public async Task<IEnumerable<Category>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _categoryRepository.GetAllAsync(cancellationToken);
        }

        public async Task<IEnumerable<Category>> GetActiveCategoriesAsync(CancellationToken cancellationToken = default)
        {
            return await _categoryRepository.GetActiveCategoriesAsync(cancellationToken);
        }

        public async Task<Category> CreateAsync(string name, string description, int displayOrder, string createdByUserId, int? parentCategoryId = null, string? imageUrl = null, string? metaTitle = null, string? metaDescription = null, CancellationToken cancellationToken = default)
        {
            // Validate inputs
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Category name is required", nameof(name));

            if (string.IsNullOrWhiteSpace(createdByUserId))
                throw new ArgumentException("CreatedByUserId is required", nameof(createdByUserId));

            // Check if parent category exists (if specified)
            if (parentCategoryId.HasValue)
            {
                var parentCategory = await _categoryRepository.GetByIdAsync(parentCategoryId.Value, cancellationToken);
                if (parentCategory == null)
                    throw new NotFoundException($"Parent category with ID {parentCategoryId} not found");

                // Check for circular reference
                if (await _categoryRepository.WouldCreateCircularReferenceAsync(parentCategoryId.Value, 0, cancellationToken))
                    throw new BusinessRuleException("Creating this category would create a circular reference");
            }

            // Create category
            var category = new Category(name, description, createdByUserId, displayOrder, parentCategoryId, imageUrl, metaTitle, metaDescription);

            // Generate and set slug
            var slug = await _slugService.CreateUniqueSlugForCategoryAsync(
                title: name,
                cancellationToken: cancellationToken
            ); category.SetSlug(slug);

            // Add to repository
            category = await _categoryRepository.AddAsync(category, cancellationToken);
            await _categoryRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Created new category with ID {CategoryId} and name {CategoryName}", category.Id, category.Name);
            return category;
        }

        public async Task UpdateAsync(int id, string name, string description, int displayOrder, string? imageUrl = null, string? metaTitle = null, string? metaDescription = null, CancellationToken cancellationToken = default)
        {
            var category = await GetByIdAsync(id, cancellationToken);

            // Validate inputs
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Category name is required", nameof(name));

            // Update category details
            category.UpdateDetails(name, description, displayOrder, imageUrl, metaTitle, metaDescription);

            // Update slug if name changed
            if (category.Name != name)
            {
                var slug = await _slugService.CreateUniqueSlugForCategoryAsync(name, id, cancellationToken);
                category.SetSlug(slug);
            }

            _categoryRepository.Update(category);
            await _categoryRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Updated category with ID {CategoryId}", id);
        }
        public async Task<IEnumerable<Category>> GetCategoryTreeAsync(CancellationToken cancellationToken = default)
        {
            var allCategories = await _categoryRepository.GetAllWithSubCategoriesAsync(cancellationToken);

            var categoryLookup = allCategories.ToDictionary(c => c.Id);

            var rootCategories = new List<Category>();

            foreach (var category in allCategories)
            {

                if (category.ParentCategoryId.HasValue)
                {

                    if (categoryLookup.TryGetValue(category.ParentCategoryId.Value, out var parentCategory))
                    {

                        if (!parentCategory.SubCategories.Any(sc => sc.Id == category.Id))
                        {

                            parentCategory.AddSubCategory(category);
                        }
                    }
                }
                else
                {
                    rootCategories.Add(category);
                }
            }

            return rootCategories;
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var category = await GetByIdAsync(id, cancellationToken);

            // Check if category has subcategories
            if (await _categoryRepository.HasSubCategoriesAsync(id, cancellationToken))
                throw new BusinessRuleException("Cannot delete category that has subcategories");

            // Check if category has products
            if (await _categoryRepository.HasProductsAsync(id, cancellationToken))
                throw new BusinessRuleException("Cannot delete category that has products");

            _categoryRepository.Remove(category);
            await _categoryRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Deleted category with ID {CategoryId}", id);
        }

        public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _categoryRepository.ExistsAsync(id, cancellationToken);
        }

        public async Task<IEnumerable<Category>> GetRootCategoriesAsync(CancellationToken cancellationToken = default)
        {
            return await _categoryRepository.GetRootCategoriesAsync(cancellationToken);
        }

        public async Task<IEnumerable<Category>> GetSubCategoriesAsync(int parentCategoryId, CancellationToken cancellationToken = default)
        {
            // Check if parent category exists
            var parentCategory = await _categoryRepository.GetByIdAsync(parentCategoryId, cancellationToken);
            if (parentCategory == null)
                throw new NotFoundException($"Parent category with ID {parentCategoryId} not found");

            return await _categoryRepository.GetSubCategoriesAsync(parentCategoryId, cancellationToken);
        }

        public async Task<IEnumerable<Category>> GetAllDescendantsAsync(int categoryId, CancellationToken cancellationToken = default)
        {
            return await _categoryRepository.GetAllDescendantsAsync(categoryId, cancellationToken);
        }

        public async Task<Category> GetParentCategoryAsync(int categoryId, CancellationToken cancellationToken = default)
        {
            return await _categoryRepository.GetParentCategoryAsync(categoryId, cancellationToken);
        }

        public async Task<IEnumerable<Category>> GetCategoryPathAsync(int categoryId, CancellationToken cancellationToken = default)
        {
            return await _categoryRepository.GetCategoryPathAsync(categoryId, cancellationToken);
        }

        public async Task<bool> HasSubCategoriesAsync(int categoryId, CancellationToken cancellationToken = default)
        {
            return await _categoryRepository.HasSubCategoriesAsync(categoryId, cancellationToken);
        }

        public async Task<bool> HasProductsAsync(int categoryId, CancellationToken cancellationToken = default)
        {
            return await _categoryRepository.HasProductsAsync(categoryId, cancellationToken);
        }

        public async Task<(IEnumerable<Category> Categories, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize, string searchTerm = null, bool onlyActive = true, int? parentCategoryId = null, string sortBy = null, bool sortAscending = true, CancellationToken cancellationToken = default)
        {
            return await _categoryRepository.GetPagedAsync(pageNumber, pageSize, searchTerm, onlyActive, parentCategoryId, sortBy, sortAscending, cancellationToken);
        }

        public async Task<IEnumerable<Category>> GetOrderedByDisplayOrderAsync(CancellationToken cancellationToken = default)
        {
            return await _categoryRepository.GetOrderedByDisplayOrderAsync(cancellationToken);
        }

        public async Task<int> GetMaxDisplayOrderAsync(int? parentCategoryId = null, CancellationToken cancellationToken = default)
        {
            return await _categoryRepository.GetMaxDisplayOrderAsync(parentCategoryId, cancellationToken);
        }

        public async Task UpdateDisplayOrderAsync(int id, int displayOrder, CancellationToken cancellationToken = default)
        {
            await _categoryRepository.UpdateDisplayOrderAsync(id, displayOrder, cancellationToken);
            _logger.LogInformation("Updated display order for category with ID {CategoryId} to {DisplayOrder}", id, displayOrder);
        }

        public async Task MoveCategoryAsync(int categoryId, int? newParentId, CancellationToken cancellationToken = default)
        {
            var category = await GetByIdAsync(categoryId, cancellationToken);

            // Check if new parent exists (if specified)
            if (newParentId.HasValue)
            {
                var newParent = await _categoryRepository.GetByIdAsync(newParentId.Value, cancellationToken);
                if (newParent == null)
                    throw new NotFoundException($"New parent category with ID {newParentId} not found");

                // Check for circular reference
                if (await _categoryRepository.WouldCreateCircularReferenceAsync(newParentId.Value, categoryId, cancellationToken))
                    throw new BusinessRuleException("Moving this category would create a circular reference");
            }

            await _categoryRepository.MoveCategoryAsync(categoryId, newParentId, cancellationToken);
            _logger.LogInformation("Moved category with ID {CategoryId} to new parent with ID {NewParentId}", categoryId, newParentId);
        }

        public async Task<bool> WouldCreateCircularReferenceAsync(int parentId, int childId, CancellationToken cancellationToken = default)
        {
            return await _categoryRepository.WouldCreateCircularReferenceAsync(parentId, childId, cancellationToken);
        }

        public async Task ActivateAsync(int id, CancellationToken cancellationToken = default)
        {
            await _categoryRepository.ActivateAsync(id, cancellationToken);
            _logger.LogInformation("Activated category with ID {CategoryId}", id);
        }

        public async Task DeactivateAsync(int id, CancellationToken cancellationToken = default)
        {
            await _categoryRepository.DeactivateAsync(id, cancellationToken);
            _logger.LogInformation("Deactivated category with ID {CategoryId}", id);
        }

        public async Task ActivateWithSubCategoriesAsync(int id, CancellationToken cancellationToken = default)
        {
            await _categoryRepository.ActivateWithSubCategoriesAsync(id, cancellationToken);
            _logger.LogInformation("Activated category with ID {CategoryId} and all its subcategories", id);
        }

        public async Task DeactivateWithSubCategoriesAsync(int id, CancellationToken cancellationToken = default)
        {
            await _categoryRepository.DeactivateWithSubCategoriesAsync(id, cancellationToken);
            _logger.LogInformation("Deactivated category with ID {CategoryId} and all its subcategories", id);
        }

        public async Task SetSlugAsync(int id, string title, CancellationToken cancellationToken = default)
        {
            var category = await GetByIdAsync(id, cancellationToken);
            var slug = await _slugService.CreateUniqueSlugForCategoryAsync(title, id, cancellationToken);
            category.SetSlug(slug);
            _categoryRepository.Update(category);
            await _categoryRepository.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Updated slug for category with ID {CategoryId}", id);
        }

        public async Task<int> GetTotalProductsCountAsync(int categoryId, CancellationToken cancellationToken = default)
        {
            return await _categoryRepository.GetTotalProductsCountAsync(categoryId, cancellationToken);
        }

        public async Task<int> GetActiveProductsCountAsync(int categoryId, CancellationToken cancellationToken = default)
        {
            return await _categoryRepository.GetActiveProductsCountAsync(categoryId, cancellationToken);
        }

        public async Task<int> GetTotalSubCategoriesCountAsync(int categoryId, CancellationToken cancellationToken = default)
        {
            return await _categoryRepository.GetTotalSubCategoriesCountAsync(categoryId, cancellationToken);
        }
    }
}