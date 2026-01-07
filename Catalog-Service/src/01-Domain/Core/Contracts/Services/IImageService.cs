using Catalog_Service.src._01_Domain.Core.Entities;
using Catalog_Service.src._01_Domain.Core.Enums;

namespace Catalog_Service.src._01_Domain.Core.Contracts.Services
{
    public interface IImageService
    {
        // متدهای اصلی CRUD
        Task<ImageResource> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<IEnumerable<ImageResource>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<ImageResource> CreateAsync(string originalFileName, string fileExtension, string storagePath, string publicUrl, long fileSize, int width, int height, ImageType imageType, string createdByUserId, string? altText = null, bool isPrimary = false, CancellationToken cancellationToken = default);
        Task UpdateAsync(int id, string? altText = null, bool? isPrimary = null, CancellationToken cancellationToken = default);
        Task DeleteAsync(int id, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);

        // متدهای جستجو و فیلتر بر اساس نوع
        Task<IEnumerable<ImageResource>> GetByTypeAsync(ImageType imageType, CancellationToken cancellationToken = default);
        Task<IEnumerable<ImageResource>> GetProductImagesAsync(int productId, CancellationToken cancellationToken = default);
        Task<IEnumerable<ImageResource>> GetCategoryImagesAsync(int categoryId, CancellationToken cancellationToken = default);
        Task<IEnumerable<ImageResource>> GetBrandImagesAsync(int brandId, CancellationToken cancellationToken = default);
        Task<IEnumerable<ImageResource>> GetProductVariantImagesAsync(int productVariantId, CancellationToken cancellationToken = default);

        // متدهای برای مدیریت تصویر اصلی
        Task<ImageResource> GetPrimaryImageAsync(int productId, CancellationToken cancellationToken = default);
        Task<ImageResource> GetPrimaryImageByTypeAsync(ImageType imageType, int entityId, CancellationToken cancellationToken = default);
        Task<IEnumerable<ImageResource>> GetNonPrimaryImagesAsync(int productId, CancellationToken cancellationToken = default);
        Task SetAsPrimaryAsync(int imageId, CancellationToken cancellationToken = default);
        Task RemoveAsPrimaryAsync(int imageId, CancellationToken cancellationToken = default);
        Task<bool> IsPrimaryAsync(int imageId, CancellationToken cancellationToken = default);

        // متدهای صفحه‌بندی
        Task<(IEnumerable<ImageResource> Images, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize, ImageType? imageType = null, int? entityId = null, bool onlyPrimary = false, CancellationToken cancellationToken = default);

        // متدهای آپلود و پردازش تصویر
        Task<ImageResource> UploadProductImageAsync(int productId, Stream imageStream, string originalFileName, string createdByUserId, string? altText = null, bool isPrimary = false, CancellationToken cancellationToken = default);
        Task<ImageResource> UploadCategoryImageAsync(int categoryId, Stream imageStream, string originalFileName, string createdByUserId, string? altText = null, bool isPrimary = false, CancellationToken cancellationToken = default);
        Task<ImageResource> UploadBrandImageAsync(int brandId, Stream imageStream, string originalFileName, string createdByUserId, string? altText = null, bool isPrimary = false, CancellationToken cancellationToken = default);
        Task<ImageResource> UploadProductVariantImageAsync(int productVariantId, Stream imageStream, string originalFileName, string createdByUserId, string? altText = null, bool isPrimary = false, CancellationToken cancellationToken = default);

        // متدهای برای حذف گروهی
        Task RemoveAllProductImagesAsync(int productId, CancellationToken cancellationToken = default);
        Task RemoveAllCategoryImagesAsync(int categoryId, CancellationToken cancellationToken = default);
        Task RemoveAllBrandImagesAsync(int brandId, CancellationToken cancellationToken = default);
        Task RemoveAllProductVariantImagesAsync(int productVariantId, CancellationToken cancellationToken = default);

        // متدهای برای به‌روزرسانی گروهی
        Task UpdateAllProductImagesPrimaryStatusAsync(int productId, int primaryImageId, CancellationToken cancellationToken = default);
        Task UpdateAllCategoryImagesPrimaryStatusAsync(int categoryId, int primaryImageId, CancellationToken cancellationToken = default);
        Task UpdateAllBrandImagesPrimaryStatusAsync(int brandId, int primaryImageId, CancellationToken cancellationToken = default);
        Task UpdateAllProductVariantImagesPrimaryStatusAsync(int productVariantId, int primaryImageId, CancellationToken cancellationToken = default);

        // متدهای برای جستجوی پیشرفته
        Task<IEnumerable<ImageResource>> GetImagesBySizeRangeAsync(int minWidth, int maxWidth, int minHeight, int maxHeight, CancellationToken cancellationToken = default);
        Task<IEnumerable<ImageResource>> GetImagesByFileSizeRangeAsync(long minSize, long maxSize, CancellationToken cancellationToken = default);
        Task<IEnumerable<ImageResource>> GetImagesByExtensionAsync(string extension, CancellationToken cancellationToken = default);
    }
}