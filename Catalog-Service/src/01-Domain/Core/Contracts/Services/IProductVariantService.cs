using Catalog_Service.src._01_Domain.Core.Entities;
using Catalog_Service.src._01_Domain.Core.Enums;
using Catalog_Service.src._01_Domain.Core.Primitives;

namespace Catalog_Service.src._01_Domain.Core.Contracts.Services
{
    public interface IProductVariantService
    {
        // متدهای اصلی CRUD
        Task<ProductVariant> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<ProductVariant> GetBySkuAsync(string sku, CancellationToken cancellationToken = default);
        Task<IEnumerable<ProductVariant>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<ProductVariant>> GetByProductIdAsync(int productId, CancellationToken cancellationToken = default);
        Task<IEnumerable<ProductVariant>> GetActiveVariantsAsync(int productId, CancellationToken cancellationToken = default);
        Task<ProductVariant> CreateAsync(int productId, string sku, string name, Money price, Dimensions dimensions, Weight weight, string? imageUrl = null, Money? originalPrice = null, CancellationToken cancellationToken = default);
        Task UpdateAsync(int id, string name, Money price, Money? originalPrice, Dimensions dimensions, Weight weight, string? imageUrl = null, CancellationToken cancellationToken = default);
        Task DeleteAsync(int id, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);

        // متدهای جستجو و فیلتر
        Task<(IEnumerable<ProductVariant> Variants, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize, int? productId = null, bool onlyActive = true, string sortBy = null, bool sortAscending = true, CancellationToken cancellationToken = default);

        // متدهای برای مدیریت موجودی
        Task<IEnumerable<ProductVariant>> GetOutOfStockVariantsAsync(int productId, CancellationToken cancellationToken = default);
        Task<IEnumerable<ProductVariant>> GetLowStockVariantsAsync(int productId, int threshold, CancellationToken cancellationToken = default);
        Task UpdateStockQuantityAsync(int variantId, int quantity, CancellationToken cancellationToken = default);
        Task UpdateStockStatusAsync(int variantId, StockStatus status, CancellationToken cancellationToken = default);

        // متدهای برای مدیریت قیمت
        Task<IEnumerable<ProductVariant>> GetVariantsInPriceRangeAsync(int productId, decimal minPrice, decimal maxPrice, CancellationToken cancellationToken = default);
        Task<ProductVariant> GetCheapestVariantAsync(int productId, CancellationToken cancellationToken = default);
        Task<ProductVariant> GetMostExpensiveVariantAsync(int productId, CancellationToken cancellationToken = default);
        Task<decimal> GetMinPriceAsync(int productId, CancellationToken cancellationToken = default);
        Task<decimal> GetMaxPriceAsync(int productId, CancellationToken cancellationToken = default);

        // متدهای برای فعال/غیرفعال کردن
        Task ActivateAsync(int variantId, CancellationToken cancellationToken = default);
        Task DeactivateAsync(int variantId, CancellationToken cancellationToken = default);
        Task ActivateAllByProductIdAsync(int productId, CancellationToken cancellationToken = default);
        Task DeactivateAllByProductIdAsync(int productId, CancellationToken cancellationToken = default);

        // متدهای برای مدیریت ویژگی‌ها
        Task<ProductAttribute> AddAttributeAsync(int variantId, string name, string value, CancellationToken cancellationToken = default);
        Task UpdateAttributeAsync(int attributeId, string name, string value, CancellationToken cancellationToken = default);
        Task DeleteAttributeAsync(int attributeId, CancellationToken cancellationToken = default);
        Task<IEnumerable<ProductAttribute>> GetAttributesAsync(int variantId, CancellationToken cancellationToken = default);

        // متدهای برای مدیریت تصاویر
        Task<ImageResource> AddImageAsync(int variantId, string originalFileName, string fileExtension, string storagePath, string publicUrl, long fileSize, int width, int height, string createdByUserId, string? altText = null, bool isPrimary = false, CancellationToken cancellationToken = default);
        Task UpdateImageAsync(int imageId, string? altText = null, bool? isPrimary = null, CancellationToken cancellationToken = default);
        Task DeleteImageAsync(int imageId, CancellationToken cancellationToken = default);
        Task<IEnumerable<ImageResource>> GetImagesAsync(int variantId, CancellationToken cancellationToken = default);

        // متدهای آمار و گزارش‌گیری
        Task<int> GetTotalStockQuantityAsync(int productId, CancellationToken cancellationToken = default);
        Task<int> GetTotalSoldQuantityAsync(int productId, CancellationToken cancellationToken = default);
        Task<decimal> GetAveragePriceAsync(int productId, CancellationToken cancellationToken = default);
    }
}