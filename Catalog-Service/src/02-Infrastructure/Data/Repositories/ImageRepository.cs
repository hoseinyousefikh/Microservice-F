using Catalog_Service.src._01_Domain.Core.Contracts.Repositories;
using Catalog_Service.src._01_Domain.Core.Entities;
using Catalog_Service.src._01_Domain.Core.Enums;
using Catalog_Service.src._02_Infrastructure.Data.Db;
using Microsoft.EntityFrameworkCore;

namespace Catalog_Service.src._02_Infrastructure.Data.Repositories
{
    public class ImageRepository : IImageRepository
    {
        private readonly AppDbContext _dbContext;

        public ImageRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ImageResource> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.ImageResources
                .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<ImageResource>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.ImageResources.ToListAsync(cancellationToken);
        }

        public async Task<ImageResource> AddAsync(ImageResource image, CancellationToken cancellationToken = default)
        {
            await _dbContext.ImageResources.AddAsync(image, cancellationToken);
            return image;
        }

        public void Update(ImageResource image)
        {
            _dbContext.ImageResources.Update(image);
        }

        public void Remove(ImageResource image)
        {
            _dbContext.ImageResources.Remove(image);
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<IEnumerable<ImageResource>> GetByTypeAsync(ImageType imageType, CancellationToken cancellationToken = default)
        {
            return await _dbContext.ImageResources
                .Where(i => i.ImageType == imageType)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<ImageResource>> GetProductImagesAsync(int productId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.ImageResources
                .Where(i => EF.Property<int?>(i, "ProductId") == productId)
                .OrderBy(i => i.IsPrimary ? 0 : 1)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<ImageResource>> GetCategoryImagesAsync(int categoryId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.ImageResources
                .Where(i => EF.Property<int?>(i, "CategoryId") == categoryId)
                .OrderBy(i => i.IsPrimary ? 0 : 1)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<ImageResource>> GetBrandImagesAsync(int brandId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.ImageResources
                .Where(i => EF.Property<int?>(i, "BrandId") == brandId)
                .OrderBy(i => i.IsPrimary ? 0 : 1)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<ImageResource>> GetProductVariantImagesAsync(int productVariantId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.ImageResources
                .Where(i => EF.Property<int?>(i, "ProductVariantId") == productVariantId)
                .OrderBy(i => i.IsPrimary ? 0 : 1)
                .ToListAsync(cancellationToken);
        }

        public async Task<ImageResource> GetPrimaryImageAsync(int productId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.ImageResources
                .Where(i => EF.Property<int?>(i, "ProductId") == productId && i.IsPrimary)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<ImageResource> GetPrimaryImageByTypeAsync(ImageType imageType, int entityId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.ImageResources
                .Where(i => i.ImageType == imageType &&
                           ((imageType == ImageType.Product && EF.Property<int?>(i, "ProductId") == entityId) ||
                            (imageType == ImageType.Category && EF.Property<int?>(i, "CategoryId") == entityId) ||
                            (imageType == ImageType.Brand && EF.Property<int?>(i, "BrandId") == entityId) ||
                            (imageType == ImageType.Variant && EF.Property<int?>(i, "ProductVariantId") == entityId)) &&
                           i.IsPrimary)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<IEnumerable<ImageResource>> GetNonPrimaryImagesAsync(int productId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.ImageResources
                .Where(i => EF.Property<int?>(i, "ProductId") == productId && !i.IsPrimary)
                .ToListAsync(cancellationToken);
        }

        public async Task SetAsPrimaryAsync(int imageId, CancellationToken cancellationToken = default)
        {
            var image = await _dbContext.ImageResources.FindAsync(new object[] { imageId }, cancellationToken);
            if (image != null)
            {
                // ابتدا تمام تصاویر مرتبط را غیراصلی می‌کنیم
                var entityId = image.ImageType switch
                {
                    ImageType.Product => EF.Property<int?>(image, "ProductId"),
                    ImageType.Category => EF.Property<int?>(image, "CategoryId"),
                    ImageType.Brand => EF.Property<int?>(image, "BrandId"),
                    ImageType.Variant => EF.Property<int?>(image, "ProductVariantId"),
                    _ => null
                };

                if (entityId.HasValue)
                {
                    var relatedImages = await _dbContext.ImageResources
                        .Where(i => i.ImageType == image.ImageType &&
                                   ((image.ImageType == ImageType.Product && EF.Property<int?>(i, "ProductId") == entityId) ||
                                    (image.ImageType == ImageType.Category && EF.Property<int?>(i, "CategoryId") == entityId) ||
                                    (image.ImageType == ImageType.Brand && EF.Property<int?>(i, "BrandId") == entityId) ||
                                    (image.ImageType == ImageType.Variant && EF.Property<int?>(i, "ProductVariantId") == entityId)))
                        .ToListAsync(cancellationToken);

                    foreach (var img in relatedImages)
                    {
                        img.RemoveAsPrimary();
                    }
                }

                // سپس تصویر مورد نظر را اصلی می‌کنیم
                image.SetAsPrimary();
                _dbContext.ImageResources.Update(image);
            }
        }

        public async Task RemoveAsPrimaryAsync(int imageId, CancellationToken cancellationToken = default)
        {
            var image = await _dbContext.ImageResources.FindAsync(new object[] { imageId }, cancellationToken);
            if (image != null)
            {
                image.RemoveAsPrimary();
                _dbContext.ImageResources.Update(image);
            }
        }

        public async Task<bool> IsPrimaryAsync(int imageId, CancellationToken cancellationToken = default)
        {
            var image = await _dbContext.ImageResources.FindAsync(new object[] { imageId }, cancellationToken);
            return image?.IsPrimary ?? false;
        }

        public async Task<(IEnumerable<ImageResource> Images, int TotalCount)> GetPagedAsync(
            int pageNumber,
            int pageSize,
            ImageType? imageType = null,
            int? entityId = null,
            bool onlyPrimary = false,
            CancellationToken cancellationToken = default)
        {
            var query = _dbContext.ImageResources.AsQueryable();

            if (imageType.HasValue)
            {
                query = query.Where(i => i.ImageType == imageType.Value);
            }

            if (entityId.HasValue)
            {
                query = query.Where(i =>
                    EF.Property<int?>(i, "ProductId") == entityId ||
                    EF.Property<int?>(i, "CategoryId") == entityId ||
                    EF.Property<int?>(i, "BrandId") == entityId ||
                    EF.Property<int?>(i, "ProductVariantId") == entityId);
            }

            if (onlyPrimary)
            {
                query = query.Where(i => i.IsPrimary);
            }

            var totalCount = await query.CountAsync(cancellationToken);
            var images = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .OrderByDescending(i => i.IsPrimary)
                .ThenBy(i => i.CreatedAt)
                .ToListAsync(cancellationToken);

            return (images, totalCount);
        }

        public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.ImageResources.AnyAsync(i => i.Id == id, cancellationToken);
        }

        public async Task<int> CountAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.ImageResources.CountAsync(cancellationToken);
        }

        public async Task<int> CountByTypeAsync(ImageType imageType, CancellationToken cancellationToken = default)
        {
            return await _dbContext.ImageResources.CountAsync(i => i.ImageType == imageType, cancellationToken);
        }

        public async Task<int> CountByEntityAsync(ImageType imageType, int entityId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.ImageResources
                .CountAsync(i => i.ImageType == imageType &&
                                ((imageType == ImageType.Product && EF.Property<int?>(i, "ProductId") == entityId) ||
                                 (imageType == ImageType.Category && EF.Property<int?>(i, "CategoryId") == entityId) ||
                                 (imageType == ImageType.Brand && EF.Property<int?>(i, "BrandId") == entityId) ||
                                 (imageType == ImageType.Variant && EF.Property<int?>(i, "ProductVariantId") == entityId)),
                            cancellationToken);
        }

        public async Task RemoveAllProductImagesAsync(int productId, CancellationToken cancellationToken = default)
        {
            var images = await _dbContext.ImageResources
                .Where(i => EF.Property<int?>(i, "ProductId") == productId)
                .ToListAsync(cancellationToken);

            _dbContext.ImageResources.RemoveRange(images);
        }

        public async Task RemoveAllCategoryImagesAsync(int categoryId, CancellationToken cancellationToken = default)
        {
            var images = await _dbContext.ImageResources
                .Where(i => EF.Property<int?>(i, "CategoryId") == categoryId)
                .ToListAsync(cancellationToken);

            _dbContext.ImageResources.RemoveRange(images);
        }

        public async Task RemoveAllBrandImagesAsync(int brandId, CancellationToken cancellationToken = default)
        {
            var images = await _dbContext.ImageResources
                .Where(i => EF.Property<int?>(i, "BrandId") == brandId)
                .ToListAsync(cancellationToken);

            _dbContext.ImageResources.RemoveRange(images);
        }

        public async Task RemoveAllProductVariantImagesAsync(int productVariantId, CancellationToken cancellationToken = default)
        {
            var images = await _dbContext.ImageResources
                .Where(i => EF.Property<int?>(i, "ProductVariantId") == productVariantId)
                .ToListAsync(cancellationToken);

            _dbContext.ImageResources.RemoveRange(images);
        }

        public async Task UpdateAllProductImagesPrimaryStatusAsync(int productId, int primaryImageId, CancellationToken cancellationToken = default)
        {
            var images = await _dbContext.ImageResources
                .Where(i => EF.Property<int?>(i, "ProductId") == productId)
                .ToListAsync(cancellationToken);

            foreach (var image in images)
            {
                if (image.Id == primaryImageId)
                {
                    image.SetAsPrimary();
                }
                else
                {
                    image.RemoveAsPrimary();
                }
                _dbContext.ImageResources.Update(image);
            }
        }

        public async Task UpdateAllCategoryImagesPrimaryStatusAsync(int categoryId, int primaryImageId, CancellationToken cancellationToken = default)
        {
            var images = await _dbContext.ImageResources
                .Where(i => EF.Property<int?>(i, "CategoryId") == categoryId)
                .ToListAsync(cancellationToken);

            foreach (var image in images)
            {
                if (image.Id == primaryImageId)
                {
                    image.SetAsPrimary();
                }
                else
                {
                    image.RemoveAsPrimary();
                }
                _dbContext.ImageResources.Update(image);
            }
        }

        public async Task UpdateAllBrandImagesPrimaryStatusAsync(int brandId, int primaryImageId, CancellationToken cancellationToken = default)
        {
            var images = await _dbContext.ImageResources
                .Where(i => EF.Property<int?>(i, "BrandId") == brandId)
                .ToListAsync(cancellationToken);

            foreach (var image in images)
            {
                if (image.Id == primaryImageId)
                {
                    image.SetAsPrimary();
                }
                else
                {
                    image.RemoveAsPrimary();
                }
                _dbContext.ImageResources.Update(image);
            }
        }

        public async Task UpdateAllProductVariantImagesPrimaryStatusAsync(int productVariantId, int primaryImageId, CancellationToken cancellationToken = default)
        {
            var images = await _dbContext.ImageResources
                .Where(i => EF.Property<int?>(i, "ProductVariantId") == productVariantId)
                .ToListAsync(cancellationToken);

            foreach (var image in images)
            {
                if (image.Id == primaryImageId)
                {
                    image.SetAsPrimary();
                }
                else
                {
                    image.RemoveAsPrimary();
                }
                _dbContext.ImageResources.Update(image);
            }
        }

        public async Task<IEnumerable<ImageResource>> GetImagesBySizeRangeAsync(
            int minWidth,
            int maxWidth,
            int minHeight,
            int maxHeight,
            CancellationToken cancellationToken = default)
        {
            return await _dbContext.ImageResources
                .Where(i => i.Width >= minWidth && i.Width <= maxWidth &&
                           i.Height >= minHeight && i.Height <= maxHeight)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<ImageResource>> GetImagesByFileSizeRangeAsync(
            long minSize,
            long maxSize,
            CancellationToken cancellationToken = default)
        {
            return await _dbContext.ImageResources
                .Where(i => i.FileSize >= minSize && i.FileSize <= maxSize)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<ImageResource>> GetImagesByExtensionAsync(
            string extension,
            CancellationToken cancellationToken = default)
        {
            return await _dbContext.ImageResources
                .Where(i => i.FileExtension.Equals(extension, StringComparison.OrdinalIgnoreCase))
                .ToListAsync(cancellationToken);
        }
    }
}
