using Catalog_Service.src._01_Domain.Core.Contracts.Repositories;
using Catalog_Service.src._01_Domain.Core.Contracts.Services;
using Catalog_Service.src._01_Domain.Core.Entities;
using Catalog_Service.src._01_Domain.Core.Enums;
using Catalog_Service.src.CrossCutting.Exceptions;

namespace Catalog_Service.src._01_Domain.Services
{
    public class ImageService : IImageService
    {
        private readonly IImageRepository _imageRepository;
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IBrandRepository _brandRepository;
        private readonly IProductVariantRepository _productVariantRepository;
        private readonly ILogger<ImageService> _logger;

        public ImageService(
            IImageRepository imageRepository,
            IProductRepository productRepository,
            ICategoryRepository categoryRepository,
            IBrandRepository brandRepository,
            IProductVariantRepository productVariantRepository,
            ILogger<ImageService> logger)
        {
            _imageRepository = imageRepository;
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _brandRepository = brandRepository;
            _productVariantRepository = productVariantRepository;
            _logger = logger;
        }

        public async Task<ImageResource> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var image = await _imageRepository.GetByIdAsync(id, cancellationToken);
            if (image == null)
            {
                _logger.LogWarning("Image with ID {ImageId} not found", id);
                throw new NotFoundException($"Image with ID {id} not found");
            }
            return image;
        }

        public async Task<IEnumerable<ImageResource>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _imageRepository.GetAllAsync(cancellationToken);
        }

        public async Task<ImageResource> CreateAsync(string originalFileName, string fileExtension, string storagePath, string publicUrl, long fileSize, int width, int height, ImageType imageType, string createdByUserId, string? altText = null, bool isPrimary = false, CancellationToken cancellationToken = default)
        {
            // Validate inputs
            if (string.IsNullOrWhiteSpace(originalFileName))
                throw new ArgumentException("Original file name is required", nameof(originalFileName));

            if (string.IsNullOrWhiteSpace(fileExtension))
                throw new ArgumentException("File extension is required", nameof(fileExtension));

            if (string.IsNullOrWhiteSpace(storagePath))
                throw new ArgumentException("Storage path is required", nameof(storagePath));

            if (string.IsNullOrWhiteSpace(publicUrl))
                throw new ArgumentException("Public URL is required", nameof(publicUrl));

            if (fileSize <= 0)
                throw new ArgumentException("File size must be greater than zero", nameof(fileSize));

            if (width <= 0 || height <= 0)
                throw new ArgumentException("Image dimensions must be greater than zero");

            if (string.IsNullOrWhiteSpace(createdByUserId))
                throw new ArgumentException("CreatedByUserId is required", nameof(createdByUserId));

            // Create image
            var image = new ImageResource(originalFileName, fileExtension, storagePath, publicUrl, fileSize, width, height, imageType, createdByUserId, altText, isPrimary);

            // Add to repository
            image = await _imageRepository.AddAsync(image, cancellationToken);
            await _imageRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Created new image with ID {ImageId} and type {ImageType}", image.Id, image.ImageType);
            return image;
        }

        public async Task UpdateAsync(int id, string? altText = null, bool? isPrimary = null, CancellationToken cancellationToken = default)
        {
            var image = await GetByIdAsync(id, cancellationToken);

            image.UpdateDetails(altText, isPrimary ?? image.IsPrimary);
            _imageRepository.Update(image);
            await _imageRepository.SaveChangesAsync(cancellationToken);

            // If set as primary, update other images
            if (isPrimary == true)
            {
                await SetAsPrimaryAsync(id, cancellationToken);
            }

            _logger.LogInformation("Updated image with ID {ImageId}", id);
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var image = await GetByIdAsync(id, cancellationToken);

            _imageRepository.Remove(image);
            await _imageRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Deleted image with ID {ImageId}", id);
        }

        public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _imageRepository.ExistsAsync(id, cancellationToken);
        }

        public async Task<IEnumerable<ImageResource>> GetByTypeAsync(ImageType imageType, CancellationToken cancellationToken = default)
        {
            return await _imageRepository.GetByTypeAsync(imageType, cancellationToken);
        }

        public async Task<IEnumerable<ImageResource>> GetProductImagesAsync(int productId, CancellationToken cancellationToken = default)
        {
            // Check if product exists
            var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
            if (product == null)
                throw new NotFoundException($"Product with ID {productId} not found");

            return await _imageRepository.GetProductImagesAsync(productId, cancellationToken);
        }

        public async Task<IEnumerable<ImageResource>> GetCategoryImagesAsync(int categoryId, CancellationToken cancellationToken = default)
        {
            // Check if category exists
            var category = await _categoryRepository.GetByIdAsync(categoryId, cancellationToken);
            if (category == null)
                throw new NotFoundException($"Category with ID {categoryId} not found");

            return await _imageRepository.GetCategoryImagesAsync(categoryId, cancellationToken);
        }

        public async Task<IEnumerable<ImageResource>> GetBrandImagesAsync(int brandId, CancellationToken cancellationToken = default)
        {
            // Check if brand exists
            var brand = await _brandRepository.GetByIdAsync(brandId, cancellationToken);
            if (brand == null)
                throw new NotFoundException($"Brand with ID {brandId} not found");

            return await _imageRepository.GetBrandImagesAsync(brandId, cancellationToken);
        }

        public async Task<IEnumerable<ImageResource>> GetProductVariantImagesAsync(int productVariantId, CancellationToken cancellationToken = default)
        {
            // Check if product variant exists
            var variant = await _productVariantRepository.GetByIdAsync(productVariantId, cancellationToken);
            if (variant == null)
                throw new NotFoundException($"Product variant with ID {productVariantId} not found");

            return await _imageRepository.GetProductVariantImagesAsync(productVariantId, cancellationToken);
        }

        public async Task<ImageResource> GetPrimaryImageAsync(int productId, CancellationToken cancellationToken = default)
        {
            // Check if product exists
            var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
            if (product == null)
                throw new NotFoundException($"Product with ID {productId} not found");

            return await _imageRepository.GetPrimaryImageAsync(productId, cancellationToken);
        }

        public async Task<ImageResource> GetPrimaryImageByTypeAsync(ImageType imageType, int entityId, CancellationToken cancellationToken = default)
        {
            // Validate entity exists based on type
            switch (imageType)
            {
                case ImageType.Product:
                    if (!await _productRepository.ExistsAsync(entityId, cancellationToken))
                        throw new NotFoundException($"Product with ID {entityId} not found");
                    break;
                case ImageType.Category:
                    if (!await _categoryRepository.ExistsAsync(entityId, cancellationToken))
                        throw new NotFoundException($"Category with ID {entityId} not found");
                    break;
                case ImageType.Brand:
                    if (!await _brandRepository.ExistsAsync(entityId, cancellationToken))
                        throw new NotFoundException($"Brand with ID {entityId} not found");
                    break;
                case ImageType.Variant:
                    if (!await _productVariantRepository.ExistsAsync(entityId, cancellationToken))
                        throw new NotFoundException($"Product variant with ID {entityId} not found");
                    break;
                default:
                    throw new ArgumentException("Invalid image type", nameof(imageType));
            }

            return await _imageRepository.GetPrimaryImageByTypeAsync(imageType, entityId, cancellationToken);
        }

        public async Task<IEnumerable<ImageResource>> GetNonPrimaryImagesAsync(int productId, CancellationToken cancellationToken = default)
        {
            // Check if product exists
            var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
            if (product == null)
                throw new NotFoundException($"Product with ID {productId} not found");

            return await _imageRepository.GetNonPrimaryImagesAsync(productId, cancellationToken);
        }

        public async Task SetAsPrimaryAsync(int imageId, CancellationToken cancellationToken = default)
        {
            await _imageRepository.SetAsPrimaryAsync(imageId, cancellationToken);
            _logger.LogInformation("Set image with ID {ImageId} as primary", imageId);
        }

        public async Task RemoveAsPrimaryAsync(int imageId, CancellationToken cancellationToken = default)
        {
            await _imageRepository.RemoveAsPrimaryAsync(imageId, cancellationToken);
            _logger.LogInformation("Removed image with ID {ImageId} as primary", imageId);
        }

        public async Task<bool> IsPrimaryAsync(int imageId, CancellationToken cancellationToken = default)
        {
            return await _imageRepository.IsPrimaryAsync(imageId, cancellationToken);
        }

        public async Task<(IEnumerable<ImageResource> Images, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize, ImageType? imageType = null, int? entityId = null, bool onlyPrimary = false, CancellationToken cancellationToken = default)
        {
            return await _imageRepository.GetPagedAsync(pageNumber, pageSize, imageType, entityId, onlyPrimary, cancellationToken);
        }

        public async Task<ImageResource> UploadProductImageAsync(int productId, Stream imageStream, string originalFileName, string createdByUserId, string? altText = null, bool isPrimary = false, CancellationToken cancellationToken = default)
        {
            // Check if product exists
            var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
            if (product == null)
                throw new NotFoundException($"Product with ID {productId} not found");

            // Process image (simplified - in real implementation, you'd process the image)
            var fileExtension = Path.GetExtension(originalFileName).TrimStart('.');
            var storagePath = $"/uploads/products/{productId}/{Guid.NewGuid()}{fileExtension}";
            var publicUrl = $"https://yourdomain.com{storagePath}";

            // In a real implementation, you would:
            // 1. Save the file to storage
            // 2. Get image dimensions
            // 3. Get file size

            // For now, we'll use placeholder values
            var fileSize = imageStream.Length;
            const int width = 800;
            const int height = 600;

            var image = await CreateAsync(originalFileName, fileExtension, storagePath, publicUrl, fileSize, width, height, ImageType.Product, createdByUserId, altText, isPrimary, cancellationToken);

            // Set shadow property for product
            // This will be handled by the repository

            // If this is set as primary, update other images
            if (isPrimary)
            {
                await _imageRepository.UpdateAllProductImagesPrimaryStatusAsync(productId, image.Id, cancellationToken);
            }

            _logger.LogInformation("Uploaded image for product with ID {ProductId}", productId);
            return image;
        }

        public async Task<ImageResource> UploadCategoryImageAsync(int categoryId, Stream imageStream, string originalFileName, string createdByUserId, string? altText = null, bool isPrimary = false, CancellationToken cancellationToken = default)
        {
            // Similar implementation to UploadProductImageAsync but for categories
            // Implementation omitted for brevity
            throw new NotImplementedException();
        }

        public async Task<ImageResource> UploadBrandImageAsync(int brandId, Stream imageStream, string originalFileName, string createdByUserId, string? altText = null, bool isPrimary = false, CancellationToken cancellationToken = default)
        {
            // Similar implementation to UploadProductImageAsync but for brands
            // Implementation omitted for brevity
            throw new NotImplementedException();
        }

        public async Task<ImageResource> UploadProductVariantImageAsync(int productVariantId, Stream imageStream, string originalFileName, string createdByUserId, string? altText = null, bool isPrimary = false, CancellationToken cancellationToken = default)
        {
            // Similar implementation to UploadProductImageAsync but for product variants
            // Implementation omitted for brevity
            throw new NotImplementedException();
        }

        public async Task RemoveAllProductImagesAsync(int productId, CancellationToken cancellationToken = default)
        {
            // Check if product exists
            var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
            if (product == null)
                throw new NotFoundException($"Product with ID {productId} not found");

            await _imageRepository.RemoveAllProductImagesAsync(productId, cancellationToken);
            _logger.LogInformation("Removed all images for product with ID {ProductId}", productId);
        }

        public async Task RemoveAllCategoryImagesAsync(int categoryId, CancellationToken cancellationToken = default)
        {
            // Check if category exists
            var category = await _categoryRepository.GetByIdAsync(categoryId, cancellationToken);
            if (category == null)
                throw new NotFoundException($"Category with ID {categoryId} not found");

            await _imageRepository.RemoveAllCategoryImagesAsync(categoryId, cancellationToken);
            _logger.LogInformation("Removed all images for category with ID {CategoryId}", categoryId);
        }

        public async Task RemoveAllBrandImagesAsync(int brandId, CancellationToken cancellationToken = default)
        {
            // Check if brand exists
            var brand = await _brandRepository.GetByIdAsync(brandId, cancellationToken);
            if (brand == null)
                throw new NotFoundException($"Brand with ID {brandId} not found");

            await _imageRepository.RemoveAllBrandImagesAsync(brandId, cancellationToken);
            _logger.LogInformation("Removed all images for brand with ID {BrandId}", brandId);
        }

        public async Task RemoveAllProductVariantImagesAsync(int productVariantId, CancellationToken cancellationToken = default)
        {
            // Check if product variant exists
            var variant = await _productVariantRepository.GetByIdAsync(productVariantId, cancellationToken);
            if (variant == null)
                throw new NotFoundException($"Product variant with ID {productVariantId} not found");

            await _imageRepository.RemoveAllProductVariantImagesAsync(productVariantId, cancellationToken);
            _logger.LogInformation("Removed all images for product variant with ID {ProductVariantId}", productVariantId);
        }

        public async Task UpdateAllProductImagesPrimaryStatusAsync(int productId, int primaryImageId, CancellationToken cancellationToken = default)
        {
            // Check if product exists
            var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
            if (product == null)
                throw new NotFoundException($"Product with ID {productId} not found");

            await _imageRepository.UpdateAllProductImagesPrimaryStatusAsync(productId, primaryImageId, cancellationToken);
            _logger.LogInformation("Updated primary image status for all images of product with ID {ProductId}", productId);
        }

        public async Task UpdateAllCategoryImagesPrimaryStatusAsync(int categoryId, int primaryImageId, CancellationToken cancellationToken = default)
        {
            // Check if category exists
            var category = await _categoryRepository.GetByIdAsync(categoryId, cancellationToken);
            if (category == null)
                throw new NotFoundException($"Category with ID {categoryId} not found");

            await _imageRepository.UpdateAllCategoryImagesPrimaryStatusAsync(categoryId, primaryImageId, cancellationToken);
            _logger.LogInformation("Updated primary image status for all images of category with ID {CategoryId}", categoryId);
        }

        public async Task UpdateAllBrandImagesPrimaryStatusAsync(int brandId, int primaryImageId, CancellationToken cancellationToken = default)
        {
            // Check if brand exists
            var brand = await _brandRepository.GetByIdAsync(brandId, cancellationToken);
            if (brand == null)
                throw new NotFoundException($"Brand with ID {brandId} not found");

            await _imageRepository.UpdateAllBrandImagesPrimaryStatusAsync(brandId, primaryImageId, cancellationToken);
            _logger.LogInformation("Updated primary image status for all images of brand with ID {BrandId}", brandId);
        }

        public async Task UpdateAllProductVariantImagesPrimaryStatusAsync(int productVariantId, int primaryImageId, CancellationToken cancellationToken = default)
        {
            // Check if product variant exists
            var variant = await _productVariantRepository.GetByIdAsync(productVariantId, cancellationToken);
            if (variant == null)
                throw new NotFoundException($"Product variant with ID {productVariantId} not found");

            await _imageRepository.UpdateAllProductVariantImagesPrimaryStatusAsync(productVariantId, primaryImageId, cancellationToken);
            _logger.LogInformation("Updated primary image status for all images of product variant with ID {ProductVariantId}", productVariantId);
        }

        public async Task<IEnumerable<ImageResource>> GetImagesBySizeRangeAsync(int minWidth, int maxWidth, int minHeight, int maxHeight, CancellationToken cancellationToken = default)
        {
            if (minWidth <= 0 || maxWidth <= 0 || minHeight <= 0 || maxHeight <= 0)
                throw new ArgumentException("Dimensions must be greater than zero");

            if (minWidth > maxWidth || minHeight > maxHeight)
                throw new ArgumentException("Invalid size range");

            return await _imageRepository.GetImagesBySizeRangeAsync(minWidth, maxWidth, minHeight, maxHeight, cancellationToken);
        }

        public async Task<IEnumerable<ImageResource>> GetImagesByFileSizeRangeAsync(long minSize, long maxSize, CancellationToken cancellationToken = default)
        {
            if (minSize <= 0 || maxSize <= 0)
                throw new ArgumentException("File sizes must be greater than zero");

            if (minSize > maxSize)
                throw new ArgumentException("Invalid file size range");

            return await _imageRepository.GetImagesByFileSizeRangeAsync(minSize, maxSize, cancellationToken);
        }

        public async Task<IEnumerable<ImageResource>> GetImagesByExtensionAsync(string extension, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(extension))
                throw new ArgumentException("Extension is required", nameof(extension));

            return await _imageRepository.GetImagesByExtensionAsync(extension, cancellationToken);
        }
    }
}