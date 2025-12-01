using Catalog_Service.src._01_Domain.Core.Contracts.Repositories;
using Catalog_Service.src._01_Domain.Core.Contracts.Services;
using Catalog_Service.src._01_Domain.Core.Entities;
using Catalog_Service.src.CrossCutting.Exceptions;

namespace Catalog_Service.src._01_Domain.Services
{
    public class ProductAttributeService : IProductAttributeService
    {
        private readonly IProductAttributeRepository _productAttributeRepository;
        private readonly IProductRepository _productRepository;
        private readonly IProductVariantRepository _productVariantRepository;
        private readonly ILogger<ProductAttributeService> _logger;

        public ProductAttributeService(
            IProductAttributeRepository productAttributeRepository,
            IProductRepository productRepository,
            IProductVariantRepository productVariantRepository,
            ILogger<ProductAttributeService> logger)
        {
            _productAttributeRepository = productAttributeRepository;
            _productRepository = productRepository;
            _productVariantRepository = productVariantRepository;
            _logger = logger;
        }

        public async Task<ProductAttribute> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var attribute = await _productAttributeRepository.GetByIdAsync(id, cancellationToken);
            if (attribute == null)
            {
                _logger.LogWarning("Product attribute with ID {AttributeId} not found", id);
                throw new NotFoundException($"Product attribute with ID {id} not found");
            }
            return attribute;
        }

        public async Task<IEnumerable<ProductAttribute>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _productAttributeRepository.GetAllAsync(cancellationToken);
        }

        public async Task<IEnumerable<ProductAttribute>> GetByProductIdAsync(int productId, CancellationToken cancellationToken = default)
        {
            // Check if product exists
            var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
            if (product == null)
                throw new NotFoundException($"Product with ID {productId} not found");

            return await _productAttributeRepository.GetByProductIdAsync(productId, cancellationToken);
        }

        public async Task<IEnumerable<ProductAttribute>> GetByProductVariantIdAsync(int productVariantId, CancellationToken cancellationToken = default)
        {
            // Check if product variant exists
            var variant = await _productVariantRepository.GetByIdAsync(productVariantId, cancellationToken);
            if (variant == null)
                throw new NotFoundException($"Product variant with ID {productVariantId} not found");

            return await _productAttributeRepository.GetByProductVariantIdAsync(productVariantId, cancellationToken);
        }

        public async Task<ProductAttribute> CreateAsync(int productId, string name, string value, int? productVariantId = null, bool isVariantSpecific = false, CancellationToken cancellationToken = default)
        {
            // Validate inputs
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Attribute name is required", nameof(name));

            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Attribute value is required", nameof(value));

            // Check if product exists
            var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
            if (product == null)
                throw new NotFoundException($"Product with ID {productId} not found");

            // If variant-specific, check if variant exists
            if (productVariantId.HasValue)
            {
                var variant = await _productVariantRepository.GetByIdAsync(productVariantId.Value, cancellationToken);
                if (variant == null)
                    throw new NotFoundException($"Product variant with ID {productVariantId} not found");

                if (variant.ProductId != productId)
                    throw new BusinessRuleException("Variant does not belong to the specified product");
            }

            // Create attribute
            var attribute = new ProductAttribute(productId, name, value, productVariantId, isVariantSpecific);

            // Add to repository
            attribute = await _productAttributeRepository.AddAsync(attribute, cancellationToken);
            await _productAttributeRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Created new product attribute with ID {AttributeId} for product with ID {ProductId}", attribute.Id, productId);
            return attribute;
        }

        public async Task UpdateAsync(int id, string name, string value, CancellationToken cancellationToken = default)
        {
            var attribute = await GetByIdAsync(id, cancellationToken);

            // Validate inputs
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Attribute name is required", nameof(name));

            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Attribute value is required", nameof(value));

            attribute.UpdateName(name);
            attribute.UpdateValue(value);
            _productAttributeRepository.Update(attribute);
            await _productAttributeRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Updated product attribute with ID {AttributeId}", id);
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var attribute = await GetByIdAsync(id, cancellationToken);

            _productAttributeRepository.Remove(attribute);
            await _productAttributeRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Deleted product attribute with ID {AttributeId}", id);
        }

        public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _productAttributeRepository.ExistsAsync(id, cancellationToken);
        }

        public async Task<(IEnumerable<ProductAttribute> Attributes, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize, int? productId = null, int? productVariantId = null, string name = null, bool onlyVariantSpecific = false, CancellationToken cancellationToken = default)
        {
            // Check if product exists (if specified)
            if (productId.HasValue && !await _productRepository.ExistsAsync(productId.Value, cancellationToken))
                throw new NotFoundException($"Product with ID {productId} not found");

            // Check if product variant exists (if specified)
            if (productVariantId.HasValue && !await _productVariantRepository.ExistsAsync(productVariantId.Value, cancellationToken))
                throw new NotFoundException($"Product variant with ID {productVariantId} not found");

            return await _productAttributeRepository.GetPagedAsync(pageNumber, pageSize, productId, productVariantId, name, onlyVariantSpecific, cancellationToken);
        }

        public async Task<IEnumerable<ProductAttribute>> GetProductAttributesAsync(int productId, CancellationToken cancellationToken = default)
        {
            // Check if product exists
            var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
            if (product == null)
                throw new NotFoundException($"Product with ID {productId} not found");

            return await _productAttributeRepository.GetProductAttributesAsync(productId, cancellationToken);
        }

        public async Task<IEnumerable<ProductAttribute>> GetVariantAttributesAsync(int productVariantId, CancellationToken cancellationToken = default)
        {
            // Check if product variant exists
            var variant = await _productVariantRepository.GetByIdAsync(productVariantId, cancellationToken);
            if (variant == null)
                throw new NotFoundException($"Product variant with ID {productVariantId} not found");

            return await _productAttributeRepository.GetVariantAttributesAsync(productVariantId, cancellationToken);
        }

        public async Task<IEnumerable<string>> GetUniqueAttributeNamesAsync(int productId, CancellationToken cancellationToken = default)
        {
            // Check if product exists
            var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
            if (product == null)
                throw new NotFoundException($"Product with ID {productId} not found");

            return await _productAttributeRepository.GetUniqueAttributeNamesAsync(productId, cancellationToken);
        }

        public async Task<IEnumerable<string>> GetUniqueAttributeValuesAsync(int productId, string attributeName, CancellationToken cancellationToken = default)
        {
            // Check if product exists
            var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
            if (product == null)
                throw new NotFoundException($"Product with ID {productId} not found");

            return await _productAttributeRepository.GetUniqueAttributeValuesAsync(productId, attributeName, cancellationToken);
        }

        public async Task<IEnumerable<ProductAttribute>> GetCommonAttributesAsync(IEnumerable<int> productIds, CancellationToken cancellationToken = default)
        {
            // Check if all products exist
            foreach (var productId in productIds)
            {
                if (!await _productRepository.ExistsAsync(productId, cancellationToken))
                    throw new NotFoundException($"Product with ID {productId} not found");
            }

            return await _productAttributeRepository.GetCommonAttributesAsync(productIds, cancellationToken);
        }

        public async Task<IEnumerable<ProductAttribute>> GetDistinctAttributesAsync(int productId, CancellationToken cancellationToken = default)
        {
            // Check if product exists
            var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
            if (product == null)
                throw new NotFoundException($"Product with ID {productId} not found");

            return await _productAttributeRepository.GetDistinctAttributesAsync(productId, cancellationToken);
        }

        public async Task RemoveAllProductAttributesAsync(int productId, CancellationToken cancellationToken = default)
        {
            // Check if product exists
            var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
            if (product == null)
                throw new NotFoundException($"Product with ID {productId} not found");

            await _productAttributeRepository.RemoveAllProductAttributesAsync(productId, cancellationToken);
            _logger.LogInformation("Removed all product attributes for product with ID {ProductId}", productId);
        }

        public async Task RemoveAllVariantAttributesAsync(int productVariantId, CancellationToken cancellationToken = default)
        {
            // Check if product variant exists
            var variant = await _productVariantRepository.GetByIdAsync(productVariantId, cancellationToken);
            if (variant == null)
                throw new NotFoundException($"Product variant with ID {productVariantId} not found");

            await _productAttributeRepository.RemoveAllVariantAttributesAsync(productVariantId, cancellationToken);
            _logger.LogInformation("Removed all variant attributes for product variant with ID {ProductVariantId}", productVariantId);
        }

        public async Task RemoveAllAttributesByNameAsync(int productId, string attributeName, CancellationToken cancellationToken = default)
        {
            // Check if product exists
            var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
            if (product == null)
                throw new NotFoundException($"Product with ID {productId} not found");

            await _productAttributeRepository.RemoveAllAttributesByNameAsync(productId, attributeName, cancellationToken);
            _logger.LogInformation("Removed all attributes with name '{AttributeName}' for product with ID {ProductId}", attributeName, productId);
        }

        public async Task UpdateAllAttributesValueAsync(int productId, string attributeName, string newValue, CancellationToken cancellationToken = default)
        {
            // Check if product exists
            var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
            if (product == null)
                throw new NotFoundException($"Product with ID {productId} not found");

            await _productAttributeRepository.UpdateAllAttributesValueAsync(productId, attributeName, newValue, cancellationToken);
            _logger.LogInformation("Updated all attributes with name '{AttributeName}' to '{NewValue}' for product with ID {ProductId}", attributeName, newValue, productId);
        }

        public async Task UpdateAllVariantAttributesValueAsync(int productVariantId, string attributeName, string newValue, CancellationToken cancellationToken = default)
        {
            // Check if product variant exists
            var variant = await _productVariantRepository.GetByIdAsync(productVariantId, cancellationToken);
            if (variant == null)
                throw new NotFoundException($"Product variant with ID {productVariantId} not found");

            await _productAttributeRepository.UpdateAllVariantAttributesValueAsync(productVariantId, attributeName, newValue, cancellationToken);
            _logger.LogInformation("Updated all variant attributes with name '{AttributeName}' to '{NewValue}' for product variant with ID {ProductVariantId}", attributeName, newValue, productVariantId);
        }

        public async Task CopyProductAttributesToVariantAsync(int productId, int productVariantId, CancellationToken cancellationToken = default)
        {
            // Check if product exists
            var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
            if (product == null)
                throw new NotFoundException($"Product with ID {productId} not found");

            // Check if product variant exists
            var variant = await _productVariantRepository.GetByIdAsync(productVariantId, cancellationToken);
            if (variant == null)
                throw new NotFoundException($"Product variant with ID {productVariantId} not found");

            if (variant.ProductId != productId)
                throw new BusinessRuleException("Variant does not belong to the specified product");

            await _productAttributeRepository.CopyProductAttributesToVariantAsync(productId, productVariantId, cancellationToken);
            _logger.LogInformation("Copied product attributes to variant with ID {ProductVariantId} for product with ID {ProductId}", productVariantId, productId);
        }

        public async Task CopyVariantAttributesToProductAsync(int productVariantId, int productId, CancellationToken cancellationToken = default)
        {
            // Check if product exists
            var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
            if (product == null)
                throw new NotFoundException($"Product with ID {productId} not found");

            // Check if product variant exists
            var variant = await _productVariantRepository.GetByIdAsync(productVariantId, cancellationToken);
            if (variant == null)
                throw new NotFoundException($"Product variant with ID {productVariantId} not found");

            if (variant.ProductId != productId)
                throw new BusinessRuleException("Variant does not belong to the specified product");

            await _productAttributeRepository.CopyVariantAttributesToProductAsync(productVariantId, productId, cancellationToken);
            _logger.LogInformation("Copied variant attributes to product with ID {ProductId} from variant with ID {ProductVariantId}", productId, productVariantId);
        }

        public async Task CopyAttributesBetweenProductsAsync(int sourceProductId, int targetProductId, CancellationToken cancellationToken = default)
        {
            // Check if both products exist
            var sourceProduct = await _productRepository.GetByIdAsync(sourceProductId, cancellationToken);
            if (sourceProduct == null)
                throw new NotFoundException($"Source product with ID {sourceProductId} not found");

            var targetProduct = await _productRepository.GetByIdAsync(targetProductId, cancellationToken);
            if (targetProduct == null)
                throw new NotFoundException($"Target product with ID {targetProductId} not found");

            await _productAttributeRepository.CopyAttributesBetweenProductsAsync(sourceProductId, targetProductId, cancellationToken);
            _logger.LogInformation("Copied attributes from product with ID {SourceProductId} to product with ID {TargetProductId}", sourceProductId, targetProductId);
        }

        public async Task<IEnumerable<ProductAttribute>> GetAttributesContainingValueAsync(int productId, string searchValue, CancellationToken cancellationToken = default)
        {
            // Check if product exists
            var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
            if (product == null)
                throw new NotFoundException($"Product with ID {productId} not found");

            return await _productAttributeRepository.GetAttributesContainingValueAsync(productId, searchValue, cancellationToken);
        }

        public async Task<IEnumerable<ProductAttribute>> GetAttributesByNameAsync(int productId, string attributeName, CancellationToken cancellationToken = default)
        {
            // Check if product exists
            var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
            if (product == null)
                throw new NotFoundException($"Product with ID {productId} not found");

            return await _productAttributeRepository.GetAttributesByNameAsync(productId, attributeName, cancellationToken);
        }

        public async Task<IEnumerable<ProductAttribute>> GetAttributesByValueAsync(int productId, string attributeValue, CancellationToken cancellationToken = default)
        {
            // Check if product exists
            var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
            if (product == null)
                throw new NotFoundException($"Product with ID {productId} not found");

            return await _productAttributeRepository.GetAttributesByValueAsync(productId, attributeValue, cancellationToken);
        }
    }
}
