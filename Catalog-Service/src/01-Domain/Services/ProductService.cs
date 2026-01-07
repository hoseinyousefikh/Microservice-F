using Catalog_Service.src._01_Domain.Core.Contracts.Repositories;
using Catalog_Service.src._01_Domain.Core.Contracts.Services;
using Catalog_Service.src._01_Domain.Core.Entities;
using Catalog_Service.src._01_Domain.Core.Enums;
using Catalog_Service.src._01_Domain.Core.Primitives;
using Catalog_Service.src.CrossCutting.Exceptions;

namespace Catalog_Service.src._01_Domain.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IBrandRepository _brandRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IProductVariantRepository _productVariantRepository;
        private readonly IImageRepository _imageRepository;
        private readonly IProductAttributeRepository _productAttributeRepository;
        private readonly IProductReviewRepository _productReviewRepository;
        private readonly IProductTagRepository _productTagRepository;
        private readonly ISlugService _slugService;
        private readonly ILogger<ProductService> _logger;

        public ProductService(
            IProductRepository productRepository,
            IBrandRepository brandRepository,
            ICategoryRepository categoryRepository,
            IProductVariantRepository productVariantRepository,
            IImageRepository imageRepository,
            IProductAttributeRepository productAttributeRepository,
            IProductReviewRepository productReviewRepository,
            IProductTagRepository productTagRepository,
            ISlugService slugService,
            ILogger<ProductService> logger)
        {
            _productRepository = productRepository;
            _brandRepository = brandRepository;
            _categoryRepository = categoryRepository;
            _productVariantRepository = productVariantRepository;
            _imageRepository = imageRepository;
            _productAttributeRepository = productAttributeRepository;
            _productReviewRepository = productReviewRepository;
            _productTagRepository = productTagRepository;
            _slugService = slugService;
            _logger = logger;
        }

        public async Task<Product> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var product = await _productRepository.GetByIdAsync(id, cancellationToken);
            if (product == null)
            {
                _logger.LogWarning("Product with ID {ProductId} not found", id);
                throw new NotFoundException($"Product with ID {id} not found");
            }
            return product;
        }

        public async Task<Product?> GetBySkuAsync(string sku, CancellationToken cancellationToken = default)
        {
            return await _productRepository.GetBySkuAsync(sku, cancellationToken);
        }

        public async Task<Product> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
        {
            var product = await _productRepository.GetBySlugAsync(Slug.FromString(slug), cancellationToken);
            if (product == null)
            {
                _logger.LogWarning("Product with slug {ProductSlug} not found", slug);
                throw new NotFoundException($"Product with slug {slug} not found");
            }
            return product;
        }

        public async Task<IEnumerable<Product>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _productRepository.GetAllAsync(cancellationToken);
        }

        public async Task<Product> CreateAsync(string name, string description, Money price, int brandId, int categoryId, string sku, Dimensions dimensions, Weight weight, string createdByUserId, string? metaTitle = null, string? metaDescription = null, CancellationToken cancellationToken = default)
        {
            // Validate inputs
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Product name is required", nameof(name));

            if (price == null || price.Amount <= 0)
                throw new ArgumentException("Product price must be greater than zero", nameof(price));

            if (string.IsNullOrWhiteSpace(sku))
                throw new ArgumentException("Product SKU is required", nameof(sku));

            if (string.IsNullOrWhiteSpace(createdByUserId))
                throw new ArgumentException("CreatedByUserId is required", nameof(createdByUserId));

            // Check if brand exists
            var brand = await _brandRepository.GetByIdAsync(brandId, cancellationToken);
            if (brand == null)
                throw new NotFoundException($"Brand with ID {brandId} not found");

            // Check if category exists
            var category = await _categoryRepository.GetByIdAsync(categoryId, cancellationToken);
            if (category == null)
                throw new NotFoundException($"Category with ID {categoryId} not found");

            // Check if SKU is unique
            if (await _productRepository.ExistsBySkuAsync(sku, cancellationToken))
                throw new DuplicateEntityException($"Product with SKU {sku} already exists");

            // Create product
            var product = new Product(name, description, price, brandId, categoryId, sku, dimensions, weight, createdByUserId, metaTitle, metaDescription);

            // Generate and set slug
            var slug = await _slugService.CreateUniqueSlugForProductAsync(
                title: name,
                cancellationToken: cancellationToken
            ); product.SetSlug(slug);

            // Add to repository
            product = await _productRepository.AddAsync(product, cancellationToken);
            await _productRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Created new product with ID {ProductId} and SKU {ProductSku}", product.Id, product.Sku);
            return product;
        }

        public async Task UpdateAsync(int id, string name, string description, Money price, Money? originalPrice, Dimensions dimensions, Weight weight, string? metaTitle = null, string? metaDescription = null, CancellationToken cancellationToken = default)
        {
            var product = await GetByIdAsync(id, cancellationToken);

            // Validate inputs
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Product name is required", nameof(name));

            if (price == null || price.Amount <= 0)
                throw new ArgumentException("Product price must be greater than zero", nameof(price));

            // Update product details
            product.UpdateDetails(name, description, price, originalPrice, dimensions, weight, metaTitle, metaDescription);

            // Update slug if name changed
            if (product.Name != name)
            {
                var slug = await _slugService.CreateUniqueSlugForProductAsync(name, id, cancellationToken);
                product.SetSlug(slug);
            }

            _productRepository.Update(product);
            await _productRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Updated product with ID {ProductId}", id);
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var product = await GetByIdAsync(id, cancellationToken);

            // Check if product has variants
            var variants = await _productVariantRepository.GetByProductIdAsync(id, cancellationToken);
            if (variants.Any())
            {
                throw new BusinessRuleException("Cannot delete product that has variants");
            }

            // Remove related entities
            await _imageRepository.RemoveAllProductImagesAsync(id, cancellationToken);
            await _productAttributeRepository.RemoveAllProductAttributesAsync(id, cancellationToken);
            await _productTagRepository.RemoveAllProductTagsAsync(id, cancellationToken);
            await _productReviewRepository.RemoveByProductIdAsync(id, cancellationToken);

            _productRepository.Remove(product);
            await _productRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Deleted product with ID {ProductId}", id);
        }

        public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _productRepository.ExistsAsync(id, cancellationToken);
        }

        public async Task<bool> ExistsBySkuAsync(string sku, CancellationToken cancellationToken = default)
        {
            return await _productRepository.ExistsBySkuAsync(sku, cancellationToken);
        }

        public async Task<(IEnumerable<Product> Products, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize, string searchTerm = null, int? categoryId = null, int? brandId = null, ProductStatus? status = null, decimal? minPrice = null, decimal? maxPrice = null, string sortBy = null, bool sortAscending = true, CancellationToken cancellationToken = default)
        {
            return await _productRepository.GetPagedAsync(pageNumber, pageSize, searchTerm, categoryId, brandId, status, minPrice, maxPrice, sortBy, sortAscending, cancellationToken);
        }

        public async Task<IEnumerable<Product>> GetByCategoryAsync(int categoryId, CancellationToken cancellationToken = default)
        {
            // Check if category exists
            var category = await _categoryRepository.GetByIdAsync(categoryId, cancellationToken);
            if (category == null)
                throw new NotFoundException($"Category with ID {categoryId} not found");

            return await _productRepository.GetByCategoryAsync(categoryId, cancellationToken);
        }

        public async Task<IEnumerable<Product>> GetByBrandAsync(int brandId, CancellationToken cancellationToken = default)
        {
            // Check if brand exists
            var brand = await _brandRepository.GetByIdAsync(brandId, cancellationToken);
            if (brand == null)
                throw new Catalog_Service.src.CrossCutting.Exceptions.NotFoundException($"Brand with ID {brandId} not found");

            return await _productRepository.GetByBrandAsync(brandId, cancellationToken);
        }
        public async Task<IEnumerable<Product>> GetByPriceRangeAsync(Money minPrice, Money maxPrice, CancellationToken cancellationToken = default)
        {
            if (minPrice == null || maxPrice == null || minPrice.Amount > maxPrice.Amount)
                throw new ArgumentException("Invalid price range");

            return await _productRepository.GetByPriceRangeAsync(minPrice, maxPrice, cancellationToken);
        }

        public async Task<IEnumerable<Product>> GetFeaturedProductsAsync(int count, CancellationToken cancellationToken = default)
        {
            return await _productRepository.GetFeaturedProductsAsync(count, cancellationToken);
        }

        public async Task<IEnumerable<Product>> GetNewestProductsAsync(int count, CancellationToken cancellationToken = default)
        {
            return await _productRepository.GetNewestProductsAsync(count, cancellationToken);
        }

        public async Task<IEnumerable<Product>> GetBestSellingProductsAsync(int count, CancellationToken cancellationToken = default)
        {
            // در یک پیاده‌سازی واقعی، این متد باید با داده‌های فروش کار کند
            return await _productRepository.GetBestSellingProductsAsync(count, cancellationToken);
        }

        public async Task PublishAsync(int id, CancellationToken cancellationToken = default)
        {
            var product = await GetByIdAsync(id, cancellationToken);
            product.Publish();
            _productRepository.Update(product);
            await _productRepository.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Published product with ID {ProductId}", id);
        }

        public async Task UnpublishAsync(int id, CancellationToken cancellationToken = default)
        {
            var product = await GetByIdAsync(id, cancellationToken);
            product.Unpublish();
            _productRepository.Update(product);
            await _productRepository.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Unpublished product with ID {ProductId}", id);
        }

        public async Task ArchiveAsync(int id, CancellationToken cancellationToken = default)
        {
            var product = await GetByIdAsync(id, cancellationToken);
            product.Archive();
            _productRepository.Update(product);
            await _productRepository.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Archived product with ID {ProductId}", id);
        }

        public async Task SetAsFeaturedAsync(int id, CancellationToken cancellationToken = default)
        {
            var product = await GetByIdAsync(id, cancellationToken);
            product.SetAsFeatured();
            _productRepository.Update(product);
            await _productRepository.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Set product with ID {ProductId} as featured", id);
        }

        public async Task RemoveFromFeaturedAsync(int id, CancellationToken cancellationToken = default)
        {
            var product = await GetByIdAsync(id, cancellationToken);
            product.RemoveFromFeatured();
            _productRepository.Update(product);
            await _productRepository.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Removed product with ID {ProductId} from featured", id);
        }

        public async Task IncrementViewCountAsync(int id, CancellationToken cancellationToken = default)
        {
            await _productRepository.IncrementViewCountAsync(id, cancellationToken);
            _logger.LogDebug("Incremented view count for product with ID {ProductId}", id);
        }

        public async Task UpdateStockQuantityAsync(int id, int quantity, CancellationToken cancellationToken = default)
        {
            if (quantity < 0)
                throw new ArgumentException("Stock quantity cannot be negative", nameof(quantity));

            await _productRepository.UpdateStockQuantityAsync(id, quantity, cancellationToken);
            _logger.LogInformation("Updated stock quantity for product with ID {ProductId} to {Quantity}", id, quantity);
        }

        public async Task UpdateStockStatusAsync(int id, StockStatus status, CancellationToken cancellationToken = default)
        {
            await _productRepository.UpdateStockStatusAsync(id, status, cancellationToken);
            _logger.LogInformation("Updated stock status for product with ID {ProductId} to {Status}", id, status);
        }

        public async Task<ProductVariant> AddVariantAsync(int productId, string sku, string name, Money price, Dimensions dimensions, Weight weight, string? imageUrl = null, Money? originalPrice = null, CancellationToken cancellationToken = default)
        {
            var product = await GetByIdAsync(productId, cancellationToken);

            // Check if SKU is unique
            if (await _productVariantRepository.ExistsBySkuAsync(sku, cancellationToken))
                throw new DuplicateEntityException($"Product variant with SKU {sku} already exists");

            var variant = new ProductVariant(productId, sku, name, price, dimensions, weight, imageUrl, originalPrice);
            variant = await _productVariantRepository.AddAsync(variant, cancellationToken);
            await _productVariantRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Added new variant with ID {VariantId} to product with ID {ProductId}", variant.Id, productId);
            return variant;
        }

        public async Task UpdateVariantAsync(int variantId, string name, Money price, Money? originalPrice, Dimensions dimensions, Weight weight, string? imageUrl = null, CancellationToken cancellationToken = default)
        {
            var variant = await _productVariantRepository.GetByIdAsync(variantId, cancellationToken);
            if (variant == null)
                throw new NotFoundException($"Product variant with ID {variantId} not found");

            variant.UpdateDetails(name, price, originalPrice, dimensions, weight, imageUrl);
            _productVariantRepository.Update(variant);
            await _productVariantRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Updated product variant with ID {VariantId}", variantId);
        }

        public async Task DeleteVariantAsync(int variantId, CancellationToken cancellationToken = default)
        {
            var variant = await _productVariantRepository.GetByIdAsync(variantId, cancellationToken);
            if (variant == null)
                throw new NotFoundException($"Product variant with ID {variantId} not found");

            // Remove related entities
            await _imageRepository.RemoveAllProductVariantImagesAsync(variantId, cancellationToken);
            await _productAttributeRepository.RemoveAllVariantAttributesAsync(variantId, cancellationToken);

            _productVariantRepository.Remove(variant);
            await _productVariantRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Deleted product variant with ID {VariantId}", variantId);
        }

        public async Task ActivateVariantAsync(int variantId, CancellationToken cancellationToken = default)
        {
            await _productVariantRepository.ActivateAsync(variantId, cancellationToken);
            _logger.LogInformation("Activated product variant with ID {VariantId}", variantId);
        }

        public async Task DeactivateVariantAsync(int variantId, CancellationToken cancellationToken = default)
        {
            await _productVariantRepository.DeactivateAsync(variantId, cancellationToken);
            _logger.LogInformation("Deactivated product variant with ID {VariantId}", variantId);
        }

        public async Task<ImageResource> AddImageAsync(int productId, string originalFileName, string fileExtension, string storagePath, string publicUrl, long fileSize, int width, int height, ImageType imageType, string createdByUserId, string? altText = null, bool isPrimary = false, CancellationToken cancellationToken = default)
        {
            var product = await GetByIdAsync(productId, cancellationToken);

            // Validate createdByUserId
            if (string.IsNullOrWhiteSpace(createdByUserId))
                throw new ArgumentException("CreatedByUserId is required", nameof(createdByUserId));

            var image = new ImageResource(originalFileName, fileExtension, storagePath, publicUrl, fileSize, width, height, imageType, createdByUserId, altText, isPrimary);

            // Set shadow property for product
            // This will be handled by the repository

            image = await _imageRepository.AddAsync(image, cancellationToken);
            await _imageRepository.SaveChangesAsync(cancellationToken);

            // If this is set as primary, update other images
            if (isPrimary)
            {
                await _imageRepository.UpdateAllProductImagesPrimaryStatusAsync(productId, image.Id, cancellationToken);
            }

            _logger.LogInformation("Added image with ID {ImageId} to product with ID {ProductId}", image.Id, productId);
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
                // Get the entity type and ID from shadow properties
                // This is a simplified approach - in real implementation, you'd need to get the entity type
                var productId = 0; // This should be retrieved from the image's shadow property
                await _imageRepository.UpdateAllProductImagesPrimaryStatusAsync(productId, imageId, cancellationToken);
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

        public async Task SetPrimaryImageAsync(int imageId, CancellationToken cancellationToken = default)
        {
            await _imageRepository.SetAsPrimaryAsync(imageId, cancellationToken);
            _logger.LogInformation("Set image with ID {ImageId} as primary", imageId);
        }

        public async Task<ProductAttribute> AddAttributeAsync(int productId, string name, string value, CancellationToken cancellationToken = default)
        {
            var product = await GetByIdAsync(productId, cancellationToken);

            var attribute = new ProductAttribute(productId, name, value);
            attribute = await _productAttributeRepository.AddAsync(attribute, cancellationToken);
            await _productAttributeRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Added attribute with ID {AttributeId} to product with ID {ProductId}", attribute.Id, productId);
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

        // *** این متد جدید اضافه شده است ***
        public async Task DeleteAttributeAsync(int attributeId, CancellationToken cancellationToken = default)
        {
            var attribute = await _productAttributeRepository.GetByIdAsync(attributeId, cancellationToken);
            if (attribute == null)
                throw new NotFoundException($"Product attribute with ID {attributeId} not found");

            _productAttributeRepository.Remove(attribute);
            await _productAttributeRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Deleted product attribute with ID {AttributeId}", attributeId);
        }

        public async Task<ProductTag> AddTagAsync(int productId, string tagText, CancellationToken cancellationToken = default)
        {
            var product = await GetByIdAsync(productId, cancellationToken);

            // Check if tag already exists
            if (await _productTagRepository.ExistsByProductAndTagAsync(productId, tagText, cancellationToken))
                throw new DuplicateEntityException($"Product already has tag '{tagText}'");

            var tag = new ProductTag(productId, tagText);
            tag = await _productTagRepository.AddAsync(tag, cancellationToken);
            await _productTagRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Added tag with ID {TagId} to product with ID {ProductId}", tag.Id, productId);
            return tag;
        }

        public async Task RemoveTagAsync(int tagId, CancellationToken cancellationToken = default)
        {
            var tag = await _productTagRepository.GetByIdAsync(tagId, cancellationToken);
            if (tag == null)
                throw new NotFoundException($"Product tag with ID {tagId} not found");

            _productTagRepository.Remove(tag);
            await _productTagRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Removed product tag with ID {TagId}", tagId);
        }

        public async Task<IEnumerable<string>> GetTagsByProductIdAsync(int productId, CancellationToken cancellationToken = default)
        {
            return await _productTagRepository.GetTagsByProductIdAsync(productId, cancellationToken);
        }

        public async Task<ProductReview> AddReviewAsync(int productId, string userId, string title, string comment, int rating, bool isVerifiedPurchase = false, CancellationToken cancellationToken = default)
        {
            var product = await GetByIdAsync(productId, cancellationToken);

            // Validate rating
            if (rating < 1 || rating > 5)
                throw new ArgumentException("Rating must be between 1 and 5", nameof(rating));

            // Check if user already reviewed this product
            if (await _productReviewRepository.UserHasReviewedProductAsync(userId, productId, cancellationToken))
                throw new BusinessRuleException("User has already reviewed this product");

            var review = new ProductReview(productId, userId, title, comment, rating, isVerifiedPurchase);
            review = await _productReviewRepository.AddReviewAsync(review, cancellationToken);
            await _productReviewRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Added review with ID {ReviewId} for product with ID {ProductId} by user {UserId}", review.Id, productId, userId);
            return review;
        }

        public async Task UpdateReviewAsync(int reviewId, string title, string comment, int rating, CancellationToken cancellationToken = default)
        {
            var review = await _productReviewRepository.GetByIdAsync(reviewId, cancellationToken);
            if (review == null)
                throw new NotFoundException($"Product review with ID {reviewId} not found");

            // Validate rating
            if (rating < 1 || rating > 5)
                throw new ArgumentException("Rating must be between 1 and 5", nameof(rating));

            review.UpdateContent(title, comment, rating);
            _productReviewRepository.Update(review);
            await _productReviewRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Updated product review with ID {ReviewId}", reviewId);
        }

        // *** یکی از متدهای DeleteReviewAsync حذف شد ***
        public async Task DeleteReviewAsync(int reviewId, CancellationToken cancellationToken = default)
        {
            var review = await _productReviewRepository.GetByIdAsync(reviewId, cancellationToken);
            if (review == null)
                throw new NotFoundException($"Product review with ID {reviewId} not found");

            _productReviewRepository.Remove(review);
            await _productReviewRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Deleted product review with ID {ReviewId}", reviewId);
        }

        public async Task ApproveReviewAsync(int reviewId, CancellationToken cancellationToken = default)
        {
            await _productReviewRepository.ApproveAsync(reviewId, cancellationToken);
            _logger.LogInformation("Approved product review with ID {ReviewId}", reviewId);
        }

        public async Task RejectReviewAsync(int reviewId, CancellationToken cancellationToken = default)
        {
            await _productReviewRepository.RejectAsync(reviewId, cancellationToken);
            _logger.LogInformation("Rejected product review with ID {ReviewId}", reviewId);
        }

        public async Task MarkReviewAsVerifiedAsync(int reviewId, CancellationToken cancellationToken = default)
        {
            await _productReviewRepository.MarkAsVerifiedAsync(reviewId, cancellationToken);
            _logger.LogInformation("Marked product review with ID {ReviewId} as verified", reviewId);
        }

        public async Task IncrementHelpfulVotesAsync(int reviewId, CancellationToken cancellationToken = default)
        {
            await _productReviewRepository.IncrementHelpfulVotesAsync(reviewId, cancellationToken);
            _logger.LogInformation("Incremented helpful votes for product review with ID {ReviewId}", reviewId);
        }

        public async Task SetSlugAsync(int id, string title, CancellationToken cancellationToken = default)
        {
            var product = await GetByIdAsync(id, cancellationToken);
            var slug = await _slugService.CreateUniqueSlugForProductAsync(title, id, cancellationToken);
            product.SetSlug(slug);
            _productRepository.Update(product);
            await _productRepository.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Updated slug for product with ID {ProductId}", id);
        }

        public async Task<decimal> GetAveragePriceByCategoryAsync(int categoryId, CancellationToken cancellationToken = default)
        {
            return await _productRepository.GetAveragePriceByCategoryAsync(categoryId, cancellationToken);
        }

        public async Task<decimal> GetAveragePriceByBrandAsync(int brandId, CancellationToken cancellationToken = default)
        {
            return await _productRepository.GetAveragePriceByBrandAsync(brandId, cancellationToken);
        }

        public async Task<int> GetViewCountAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _productRepository.GetViewCountAsync(id, cancellationToken);
        }

        public async Task<double> GetAverageRatingAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _productRepository.GetAverageRatingAsync(id, cancellationToken);
        }

        public async Task<int> GetTotalReviewsCountAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _productRepository.GetTotalReviewsCountAsync(id, cancellationToken);
        }

        public async Task<IDictionary<int, int>> GetRatingDistributionAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _productRepository.GetRatingDistributionAsync(id, cancellationToken);
        }
    }
}