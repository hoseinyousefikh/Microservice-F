using Catalog_Service.src._01_Domain.Core.Contracts.Repositories;
using Catalog_Service.src._01_Domain.Core.Contracts.Services;
using Catalog_Service.src._01_Domain.Core.Entities;
using Catalog_Service.src.CrossCutting.Exceptions;

namespace Catalog_Service.src._01_Domain.Services
{
    public class ProductTagService : IProductTagService
    {
        private readonly IProductTagRepository _productTagRepository;
        private readonly IProductRepository _productRepository;
        private readonly ILogger<ProductTagService> _logger;

        public ProductTagService(
            IProductTagRepository productTagRepository,
            IProductRepository productRepository,
            ILogger<ProductTagService> logger)
        {
            _productTagRepository = productTagRepository;
            _productRepository = productRepository;
            _logger = logger;
        }

        public async Task<ProductTag> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var tag = await _productTagRepository.GetByIdAsync(id, cancellationToken);
            if (tag == null)
            {
                _logger.LogWarning("Product tag with ID {TagId} not found", id);
                throw new NotFoundException($"Product tag with ID {id} not found");
            }
            return tag;
        }

        public async Task<IEnumerable<ProductTag>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _productTagRepository.GetAllAsync(cancellationToken);
        }

        public async Task<IEnumerable<ProductTag>> GetByProductIdAsync(int productId, CancellationToken cancellationToken = default)
        {
            // Check if product exists
            var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
            if (product == null)
                throw new NotFoundException($"Product with ID {productId} not found");

            return await _productTagRepository.GetByProductIdAsync(productId, cancellationToken);
        }

        public async Task<ProductTag> CreateAsync(int productId, string tagText, CancellationToken cancellationToken = default)
        {
            // Validate inputs
            if (string.IsNullOrWhiteSpace(tagText))
                throw new ArgumentException("Tag text is required", nameof(tagText));

            // Check if product exists
            var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
            if (product == null)
                throw new NotFoundException($"Product with ID {productId} not found");

            // Check if tag already exists for this product
            if (await _productTagRepository.ProductHasTagAsync(productId, tagText, cancellationToken))
                throw new DuplicateEntityException($"Product already has tag '{tagText}'");

            // Create tag
            var tag = new ProductTag(productId, tagText);

            // Add to repository
            tag = await _productTagRepository.AddAsync(tag, cancellationToken);
            await _productTagRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Created new product tag with ID {TagId} for product with ID {ProductId}", tag.Id, productId);
            return tag;
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var tag = await GetByIdAsync(id, cancellationToken);

            _productTagRepository.Remove(tag);
            await _productTagRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Deleted product tag with ID {TagId}", id);
        }

        public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _productTagRepository.ExistsAsync(id, cancellationToken);
        }

        public async Task<(IEnumerable<ProductTag> Tags, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize, int? productId = null, string searchTerm = null, CancellationToken cancellationToken = default)
        {
            // Check if product exists (if specified)
            if (productId.HasValue && !await _productRepository.ExistsAsync(productId.Value, cancellationToken))
                throw new NotFoundException($"Product with ID {productId} not found");

            return await _productTagRepository.GetPagedAsync(pageNumber, pageSize, productId, searchTerm, cancellationToken);
        }

        public async Task<IEnumerable<string>> GetTagsByProductIdAsync(int productId, CancellationToken cancellationToken = default)
        {
            // Check if product exists
            var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
            if (product == null)
                throw new NotFoundException($"Product with ID {productId} not found");

            return await _productTagRepository.GetTagsByProductIdAsync(productId, cancellationToken);
        }

        public async Task<IEnumerable<int>> GetProductIdsByTagAsync(string tagText, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(tagText))
                throw new ArgumentException("Tag text is required", nameof(tagText));

            return await _productTagRepository.GetProductIdsByTagAsync(tagText, cancellationToken);
        }

        public async Task<IEnumerable<Product>> GetProductsByTagAsync(string tagText, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(tagText))
                throw new ArgumentException("Tag text is required", nameof(tagText));

            return await _productTagRepository.GetProductsByTagAsync(tagText, cancellationToken);
        }

        public async Task<bool> ProductHasTagAsync(int productId, string tagText, CancellationToken cancellationToken = default)
        {
            // Check if product exists
            var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
            if (product == null)
                throw new NotFoundException($"Product with ID {productId} not found");

            return await _productTagRepository.ProductHasTagAsync(productId, tagText, cancellationToken);
        }

        public async Task<IEnumerable<string>> GetPopularTagsAsync(int count, CancellationToken cancellationToken = default)
        {
            return await _productTagRepository.GetPopularTagsAsync(count, cancellationToken);
        }

        public async Task<IEnumerable<string>> GetMostUsedTagsAsync(int count, CancellationToken cancellationToken = default)
        {
            return await _productTagRepository.GetMostUsedTagsAsync(count, cancellationToken);
        }

        public async Task<IDictionary<string, int>> GetTagUsageCountAsync(IEnumerable<string> tags, CancellationToken cancellationToken = default)
        {
            if (tags == null || !tags.Any())
                throw new ArgumentException("Tags collection is required", nameof(tags));

            return await _productTagRepository.GetTagUsageCountAsync(tags, cancellationToken);
        }

        public async Task<int> CountAsync(CancellationToken cancellationToken = default)
        {
            return await _productTagRepository.CountAsync(cancellationToken);
        }

        public async Task<int> CountByProductIdAsync(int productId, CancellationToken cancellationToken = default)
        {
            // Check if product exists
            var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
            if (product == null)
                throw new NotFoundException($"Product with ID {productId} not found");

            return await _productTagRepository.CountByProductIdAsync(productId, cancellationToken);
        }

        public async Task<int> CountByTagTextAsync(string tagText, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(tagText))
                throw new ArgumentException("Tag text is required", nameof(tagText));

            return await _productTagRepository.CountByTagTextAsync(tagText, cancellationToken);
        }

        public async Task RemoveAllProductTagsAsync(int productId, CancellationToken cancellationToken = default)
        {
            // Check if product exists
            var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
            if (product == null)
                throw new NotFoundException($"Product with ID {productId} not found");

            await _productTagRepository.RemoveAllProductTagsAsync(productId, cancellationToken);
            _logger.LogInformation("Removed all tags for product with ID {ProductId}", productId);
        }

        public async Task RemoveAllTagsByTextAsync(string tagText, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(tagText))
                throw new ArgumentException("Tag text is required", nameof(tagText));

            await _productTagRepository.RemoveAllTagsByTextAsync(tagText, cancellationToken);
            _logger.LogInformation("Removed all tags with text '{TagText}'", tagText);
        }

        public async Task UpdateTagTextAsync(int productId, string oldTagText, string newTagText, CancellationToken cancellationToken = default)
        {
            // Check if product exists
            var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
            if (product == null)
                throw new NotFoundException($"Product with ID {productId} not found");

            if (string.IsNullOrWhiteSpace(oldTagText))
                throw new ArgumentException("Old tag text is required", nameof(oldTagText));

            if (string.IsNullOrWhiteSpace(newTagText))
                throw new ArgumentException("New tag text is required", nameof(newTagText));

            await _productTagRepository.UpdateTagTextAsync(productId, oldTagText, newTagText, cancellationToken);
            _logger.LogInformation("Updated tag text from '{OldTagText}' to '{NewTagText}' for product with ID {ProductId}", oldTagText, newTagText, productId);
        }

        public async Task CopyTagsBetweenProductsAsync(int sourceProductId, int targetProductId, CancellationToken cancellationToken = default)
        {
            // Check if both products exist
            var sourceProduct = await _productRepository.GetByIdAsync(sourceProductId, cancellationToken);
            if (sourceProduct == null)
                throw new NotFoundException($"Source product with ID {sourceProductId} not found");

            var targetProduct = await _productRepository.GetByIdAsync(targetProductId, cancellationToken);
            if (targetProduct == null)
                throw new NotFoundException($"Target product with ID {targetProductId} not found");

            await _productTagRepository.CopyTagsBetweenProductsAsync(sourceProductId, targetProductId, cancellationToken);
            _logger.LogInformation("Copied tags from product with ID {SourceProductId} to product with ID {TargetProductId}", sourceProductId, targetProductId);
        }

        public async Task<IEnumerable<string>> GetTagsContainingAsync(string searchText, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(searchText))
                throw new ArgumentException("Search text is required", nameof(searchText));

            return await _productTagRepository.GetTagsContainingAsync(searchText, cancellationToken);
        }

        public async Task<IEnumerable<string>> GetTagsStartingWithAsync(string prefix, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(prefix))
                throw new ArgumentException("Prefix is required", nameof(prefix));

            return await _productTagRepository.GetTagsStartingWithAsync(prefix, cancellationToken);
        }

        public async Task<IEnumerable<string>> GetTagsEndingWithAsync(string suffix, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(suffix))
                throw new ArgumentException("Suffix is required", nameof(suffix));

            return await _productTagRepository.GetTagsEndingWithAsync(suffix, cancellationToken);
        }

        public async Task<IEnumerable<string>> GetSimilarTagsAsync(string tagText, int count, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(tagText))
                throw new ArgumentException("Tag text is required", nameof(tagText));

            return await _productTagRepository.GetSimilarTagsAsync(tagText, count, cancellationToken);
        }

        public async Task<IEnumerable<string>> GetRecommendedTagsForProductAsync(int productId, int count, CancellationToken cancellationToken = default)
        {
            // Check if product exists
            var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
            if (product == null)
                throw new NotFoundException($"Product with ID {productId} not found");

            return await _productTagRepository.GetRecommendedTagsForProductAsync(productId, count, cancellationToken);
        }

        public async Task<IDictionary<string, int>> GetRelatedTagsAsync(string tagText, int count, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(tagText))
                throw new ArgumentException("Tag text is required", nameof(tagText));

            return await _productTagRepository.GetRelatedTagsAsync(tagText, count, cancellationToken);
        }

        public async Task<IEnumerable<string>> GetTrendingTagsAsync(DateTime sinceDate, int count, CancellationToken cancellationToken = default)
        {
            return await _productTagRepository.GetTrendingTagsAsync(sinceDate, count, cancellationToken);
        }
    }
}
