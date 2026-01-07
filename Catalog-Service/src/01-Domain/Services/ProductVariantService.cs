using Catalog_Service.src._01_Domain.Core.Contracts.Repositories;
using Catalog_Service.src._01_Domain.Core.Contracts.Services;
using Catalog_Service.src._01_Domain.Core.Entities;
using Catalog_Service.src._01_Domain.Core.Enums;
using Catalog_Service.src._01_Domain.Core.Primitives;
using Catalog_Service.src.CrossCutting.Exceptions;

namespace Catalog_Service.src._01_Domain.Services
{
    public class ProductVariantService : IProductVariantService
    {
        private readonly IProductVariantRepository _productVariantRepository;
        private readonly IProductRepository _productRepository;
        private readonly IImageRepository _imageRepository;
        private readonly IProductAttributeRepository _productAttributeRepository;
        private readonly ILogger<ProductVariantService> _logger;

        public ProductVariantService(
            IProductVariantRepository productVariantRepository,
            IProductRepository productRepository,
            IImageRepository imageRepository,
            IProductAttributeRepository productAttributeRepository,
            ILogger<ProductVariantService> logger)
        {
            _productVariantRepository = productVariantRepository;
            _productRepository = productRepository;
            _imageRepository = imageRepository;
            _productAttributeRepository = productAttributeRepository;
            _logger = logger;
        }

        public async Task<ProductVariant> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var variant = await _productVariantRepository.GetByIdAsync(id, cancellationToken);
            if (variant == null)
            {
                _logger.LogWarning("Product variant with ID {VariantId} not found", id);
                throw new NotFoundException($"Product variant with ID {id} not found");
            }
            return variant;
        }

        public async Task<ProductVariant> GetBySkuAsync(string sku, CancellationToken cancellationToken = default)
        {
            var variant = await _productVariantRepository.GetBySkuAsync(sku, cancellationToken);
            if (variant == null)
            {
                _logger.LogWarning("Product variant with SKU {VariantSku} not found", sku);
                throw new NotFoundException($"Product variant with SKU {sku} not found");
            }
            return variant;
        }

        public async Task<IEnumerable<ProductVariant>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _productVariantRepository.GetAllAsync(cancellationToken);
        }

        public async Task<IEnumerable<ProductVariant>> GetByProductIdAsync(int productId, CancellationToken cancellationToken = default)
        {
            // Check if product exists
            var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
            if (product == null)
                throw new NotFoundException($"Product with ID {productId} not found");

            return await _productVariantRepository.GetByProductIdAsync(productId, cancellationToken);
        }

        public async Task<IEnumerable<ProductVariant>> GetActiveVariantsAsync(int productId, CancellationToken cancellationToken = default)
        {
            // Check if product exists
            var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
            if (product == null)
                throw new NotFoundException($"Product with ID {productId} not found");

            return await _productVariantRepository.GetActiveVariantsAsync(productId, cancellationToken);
        }

        public async Task<ProductVariant> CreateAsync(int productId, string sku, string name, Money price, Dimensions dimensions, Weight weight, string? imageUrl = null, Money? originalPrice = null, CancellationToken cancellationToken = default)
        {
            // Validate inputs
            if (string.IsNullOrWhiteSpace(sku))
                throw new ArgumentException("SKU is required", nameof(sku));

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name is required", nameof(name));

            if (price == null || price.Amount <= 0)
                throw new ArgumentException("Price must be greater than zero", nameof(price));

            // Check if product exists
            var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
            if (product == null)
                throw new NotFoundException($"Product with ID {productId} not found");

            // Check if SKU is unique
            if (await _productVariantRepository.ExistsBySkuAsync(sku, cancellationToken))
                throw new DuplicateEntityException($"Product variant with SKU {sku} already exists");

            // Create variant
            var variant = new ProductVariant(productId, sku, name, price, dimensions, weight, imageUrl, originalPrice);

            // Add to repository
            variant = await _productVariantRepository.AddAsync(variant, cancellationToken);
            await _productVariantRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Created new product variant with ID {VariantId} for product with ID {ProductId}", variant.Id, productId);
            return variant;
        }

        public async Task UpdateAsync(int id, string name, Money price, Money? originalPrice, Dimensions dimensions, Weight weight, string? imageUrl = null, CancellationToken cancellationToken = default)
        {
            var variant = await GetByIdAsync(id, cancellationToken);

            // Validate inputs
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name is required", nameof(name));

            if (price == null || price.Amount <= 0)
                throw new ArgumentException("Price must be greater than zero", nameof(price));

            variant.UpdateDetails(name, price, originalPrice, dimensions, weight, imageUrl);
            _productVariantRepository.Update(variant);
            await _productVariantRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Updated product variant with ID {VariantId}", id);
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var variant = await GetByIdAsync(id, cancellationToken);

            // Remove related entities
            await _imageRepository.RemoveAllProductVariantImagesAsync(id, cancellationToken);
            await _productAttributeRepository.RemoveAllVariantAttributesAsync(id, cancellationToken);

            _productVariantRepository.Remove(variant);
            await _productVariantRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Deleted product variant with ID {VariantId}", id);
        }

        public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _productVariantRepository.ExistsAsync(id, cancellationToken);
        }

        public async Task<(IEnumerable<ProductVariant> Variants, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize, int? productId = null, bool onlyActive = true, string sortBy = null, bool sortAscending = true, CancellationToken cancellationToken = default)
        {
            // Check if product exists (if specified)
            if (productId.HasValue && !await _productRepository.ExistsAsync(productId.Value, cancellationToken))
                throw new NotFoundException($"Product with ID {productId} not found");

            return await _productVariantRepository.GetPagedAsync(pageNumber, pageSize, productId, onlyActive, sortBy, sortAscending, cancellationToken);
        }

        public async Task<IEnumerable<ProductVariant>> GetOutOfStockVariantsAsync(int productId, CancellationToken cancellationToken = default)
        {
            // Check if product exists
            var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
            if (product == null)
                throw new NotFoundException($"Product with ID {productId} not found");

            return await _productVariantRepository.GetOutOfStockVariantsAsync(productId, cancellationToken);
        }

        public async Task<IEnumerable<ProductVariant>> GetLowStockVariantsAsync(int productId, int threshold, CancellationToken cancellationToken = default)
        {
            // Check if product exists
            var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
            if (product == null)
                throw new NotFoundException($"Product with ID {productId} not found");

            return await _productVariantRepository.GetLowStockVariantsAsync(productId, threshold, cancellationToken);
        }

        public async Task UpdateStockQuantityAsync(int variantId, int quantity, CancellationToken cancellationToken = default)
        {
            if (quantity < 0)
                throw new ArgumentException("Stock quantity cannot be negative", nameof(quantity));

            await _productVariantRepository.UpdateStockQuantityAsync(variantId, quantity, cancellationToken);
            _logger.LogInformation("Updated stock quantity for product variant with ID {VariantId} to {Quantity}", variantId, quantity);
        }

        public async Task UpdateStockStatusAsync(int variantId, StockStatus status, CancellationToken cancellationToken = default)
        {
            await _productVariantRepository.UpdateStockStatusAsync(variantId, status, cancellationToken);
            _logger.LogInformation("Updated stock status for product variant with ID {VariantId} to {Status}", variantId, status);
        }

        public async Task<IEnumerable<ProductVariant>> GetVariantsInPriceRangeAsync(int productId, decimal minPrice, decimal maxPrice, CancellationToken cancellationToken = default)
        {
            // Check if product exists
            var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
            if (product == null)
                throw new NotFoundException($"Product with ID {productId} not found");

            if (minPrice < 0 || maxPrice < 0 || minPrice > maxPrice)
                throw new ArgumentException("Invalid price range");

            return await _productVariantRepository.GetVariantsInPriceRangeAsync(productId, minPrice, maxPrice, cancellationToken);
        }

        public async Task<ProductVariant> GetCheapestVariantAsync(int productId, CancellationToken cancellationToken = default)
        {
            // Check if product exists
            var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
            if (product == null)
                throw new NotFoundException($"Product with ID {productId} not found");

            return await _productVariantRepository.GetCheapestVariantAsync(productId, cancellationToken);
        }

        public async Task<ProductVariant> GetMostExpensiveVariantAsync(int productId, CancellationToken cancellationToken = default)
        {
            // Check if product exists
            var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
            if (product == null)
                throw new NotFoundException($"Product with ID {productId} not found");

            return await _productVariantRepository.GetMostExpensiveVariantAsync(productId, cancellationToken);
        }

        public async Task<decimal> GetMinPriceAsync(int productId, CancellationToken cancellationToken = default)
        {
            // Check if product exists
            var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
            if (product == null)
                throw new NotFoundException($"Product with ID {productId} not found");

            return await _productVariantRepository.GetMinPriceAsync(productId, cancellationToken);
        }

        public async Task<decimal> GetMaxPriceAsync(int productId, CancellationToken cancellationToken = default)
        {
            // Check if product exists
            var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
            if (product == null)
                throw new NotFoundException($"Product with ID {productId} not found");

            return await _productVariantRepository.GetMaxPriceAsync(productId, cancellationToken);
        }

        public async Task ActivateAsync(int variantId, CancellationToken cancellationToken = default)
        {
            await _productVariantRepository.ActivateAsync(variantId, cancellationToken);
            _logger.LogInformation("Activated product variant with ID {VariantId}", variantId);
        }

        public async Task DeactivateAsync(int variantId, CancellationToken cancellationToken = default)
        {
            await _productVariantRepository.DeactivateAsync(variantId, cancellationToken);
            _logger.LogInformation("Deactivated product variant with ID {VariantId}", variantId);
        }

        public async Task ActivateAllByProductIdAsync(int productId, CancellationToken cancellationToken = default)
        {
            // Check if product exists
            var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
            if (product == null)
                throw new NotFoundException($"Product with ID {productId} not found");

            await _productVariantRepository.ActivateAllByProductIdAsync(productId, cancellationToken);
            _logger.LogInformation("Activated all variants for product with ID {ProductId}", productId);
        }

        public async Task DeactivateAllByProductIdAsync(int productId, CancellationToken cancellationToken = default)
        {
            // Check if product exists
            var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
            if (product == null)
                throw new NotFoundException($"Product with ID {productId} not found");

            await _productVariantRepository.DeactivateAllByProductIdAsync(productId, cancellationToken);
            _logger.LogInformation("Deactivated all variants for product with ID {ProductId}", productId);
        }

        public async Task<ProductAttribute> AddAttributeAsync(int variantId, string name, string value, CancellationToken cancellationToken = default)
        {
            var variant = await GetByIdAsync(variantId, cancellationToken);

            var attribute = new ProductAttribute(variant.ProductId, name, value, variantId, true);
            attribute = await _productAttributeRepository.AddAsync(attribute, cancellationToken);
            await _productAttributeRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Added attribute with ID {AttributeId} to product variant with ID {VariantId}", attribute.Id, variantId);
            return attribute;
        }

        public async Task UpdateAttributeAsync(int attributeId, string name, string value, CancellationToken cancellationToken = default)
        {
            var attribute = await _productAttributeRepository.GetByIdAsync(attributeId, cancellationToken);
            if (attribute == null)
                throw new NotFoundException($"Product attribute with ID {attributeId} not found");

            attribute.UpdateName(name);
            attribute.UpdateValue(value);
            _productAttributeRepository.Update(attribute);
            await _productAttributeRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Updated product attribute with ID {AttributeId}", attributeId);
        }

        public async Task DeleteAttributeAsync(int attributeId, CancellationToken cancellationToken = default)
        {
            var attribute = await _productAttributeRepository.GetByIdAsync(attributeId, cancellationToken);
            if (attribute == null)
                throw new NotFoundException($"Product attribute with ID {attributeId} not found");

            _productAttributeRepository.Remove(attribute);
            await _productAttributeRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Deleted product attribute with ID {AttributeId}", attributeId);
        }

        public async Task<IEnumerable<ProductAttribute>> GetAttributesAsync(int variantId, CancellationToken cancellationToken = default)
        {
            var variant = await GetByIdAsync(variantId, cancellationToken);
            return await _productAttributeRepository.GetAttributesAsync(variantId, cancellationToken);
        }

        public async Task<ImageResource> AddImageAsync(int variantId, string originalFileName, string fileExtension, string storagePath, string publicUrl, long fileSize, int width, int height, string createdByUserId, string? altText = null, bool isPrimary = false, CancellationToken cancellationToken = default)
        {
            var variant = await GetByIdAsync(variantId, cancellationToken);

            if (string.IsNullOrWhiteSpace(createdByUserId))
                throw new ArgumentException("CreatedByUserId is required", nameof(createdByUserId));

            var image = new ImageResource(originalFileName, fileExtension, storagePath, publicUrl, fileSize, width, height, ImageType.Variant, createdByUserId, altText, isPrimary);

            // Set shadow property for product variant
            // This will be handled by the repository

            image = await _imageRepository.AddAsync(image, cancellationToken);
            await _imageRepository.SaveChangesAsync(cancellationToken);

            // If this is set as primary, update other images
            if (isPrimary)
            {
                await _imageRepository.UpdateAllProductVariantImagesPrimaryStatusAsync(variantId, image.Id, cancellationToken);
            }

            _logger.LogInformation("Added image with ID {ImageId} to product variant with ID {VariantId}", image.Id, variantId);
            return image;
        }

        public async Task UpdateImageAsync(int imageId, string? altText = null, bool? isPrimary = null, CancellationToken cancellationToken = default)
        {
            var image = await _imageRepository.GetByIdAsync(imageId, cancellationToken);
            if (image == null)
                throw new NotFoundException($"Image with ID {imageId} not found");

            image.UpdateDetails(altText, isPrimary ?? image.IsPrimary);
            _imageRepository.Update(image);
            await _imageRepository.SaveChangesAsync(cancellationToken);

            // If set as primary, update other images
            if (isPrimary == true)
            {
                // Get the product variant ID from shadow property
                var variantId = 0; // This should be retrieved from the image's shadow property
                await _imageRepository.UpdateAllProductVariantImagesPrimaryStatusAsync(variantId, imageId, cancellationToken);
            }

            _logger.LogInformation("Updated image with ID {ImageId}", imageId);
        }

        public async Task DeleteImageAsync(int imageId, CancellationToken cancellationToken = default)
        {
            var image = await _imageRepository.GetByIdAsync(imageId, cancellationToken);
            if (image == null)
                throw new NotFoundException($"Image with ID {imageId} not found");

            _imageRepository.Remove(image);
            await _imageRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Deleted image with ID {ImageId}", imageId);
        }

        public async Task<IEnumerable<ImageResource>> GetImagesAsync(int variantId, CancellationToken cancellationToken = default)
        {
            var variant = await GetByIdAsync(variantId, cancellationToken);
            return await _imageRepository.GetProductVariantImagesAsync(variantId, cancellationToken);
        }

        public async Task<int> GetTotalStockQuantityAsync(int productId, CancellationToken cancellationToken = default)
        {
            // Check if product exists
            var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
            if (product == null)
                throw new NotFoundException($"Product with ID {productId} not found");

            return await _productVariantRepository.GetTotalStockQuantityAsync(productId, cancellationToken);
        }

        public async Task<int> GetTotalSoldQuantityAsync(int productId, CancellationToken cancellationToken = default)
        {
            // Check if product exists
            var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
            if (product == null)
                throw new NotFoundException($"Product with ID {productId} not found");

            return await _productVariantRepository.GetTotalSoldQuantityAsync(productId, cancellationToken);
        }

        public async Task<decimal> GetAveragePriceAsync(int productId, CancellationToken cancellationToken = default)
        {
            // Check if product exists
            var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
            if (product == null)
                throw new NotFoundException($"Product with ID {productId} not found");

            return await _productVariantRepository.GetAveragePriceAsync(productId, cancellationToken);
        }
    }
}