using Catalog_Service.src._01_Domain.Core.Entities;
using Catalog_Service.src._01_Domain.Core.Enums;

namespace Catalog_Service.src._01_Domain.Core.Contracts.Repositories
{
    public interface IProductVariantRepository
    {
        // متدهای اصلی CRUD
        Task<ProductVariant> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<ProductVariant> GetBySkuAsync(string sku, CancellationToken cancellationToken = default);
        Task<IEnumerable<ProductVariant>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<ProductVariant>> GetByProductIdAsync(int productId, CancellationToken cancellationToken = default);
        Task<IEnumerable<ProductVariant>> GetActiveVariantsAsync(int productId, CancellationToken cancellationToken = default);
        Task<ProductVariant> AddAsync(ProductVariant variant, CancellationToken cancellationToken = default);
        void Update(ProductVariant variant);
        void Remove(ProductVariant variant);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        // متدهای جستجو و فیلتر
        Task<(IEnumerable<ProductVariant> Variants, int TotalCount)> GetPagedAsync(
            int pageNumber,
            int pageSize,
            int? productId = null,
            bool onlyActive = true,
            string sortBy = null,
            bool sortAscending = true,
            CancellationToken cancellationToken = default);

        // متدهای برای مدیریت موجودی
        Task<IEnumerable<ProductVariant>> GetOutOfStockVariantsAsync(int productId, CancellationToken cancellationToken = default);
        Task<IEnumerable<ProductVariant>> GetLowStockVariantsAsync(int productId, int threshold, CancellationToken cancellationToken = default);
        Task UpdateStockQuantityAsync(int variantId, int quantity, CancellationToken cancellationToken = default);
        Task UpdateStockStatusAsync(int variantId, StockStatus status, CancellationToken cancellationToken = default);

        // متدهای برای مدیریت قیمت
        Task<IEnumerable<ProductVariant>> GetVariantsInPriceRangeAsync(
            int productId,
            decimal minPrice,
            decimal maxPrice,
            CancellationToken cancellationToken = default);

        Task<ProductVariant> GetCheapestVariantAsync(int productId, CancellationToken cancellationToken = default);
        Task<ProductVariant> GetMostExpensiveVariantAsync(int productId, CancellationToken cancellationToken = default);
        Task<decimal> GetMinPriceAsync(int productId, CancellationToken cancellationToken = default);
        Task<decimal> GetMaxPriceAsync(int productId, CancellationToken cancellationToken = default);

        // متدهای ویژه
        Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
        Task<bool> ExistsBySkuAsync(string sku, CancellationToken cancellationToken = default);
        Task<bool> IsUniqueSkuAsync(string sku, int? excludeVariantId = null, CancellationToken cancellationToken = default);
        Task<int> CountAsync(CancellationToken cancellationToken = default);
        Task<int> CountByProductIdAsync(int productId, CancellationToken cancellationToken = default);
        Task<int> CountActiveByProductIdAsync(int productId, CancellationToken cancellationToken = default);

        // متدهای برای فعال/غیرفعال کردن
        Task ActivateAsync(int variantId, CancellationToken cancellationToken = default);
        Task DeactivateAsync(int variantId, CancellationToken cancellationToken = default);
        Task ActivateAllByProductIdAsync(int productId, CancellationToken cancellationToken = default);
        Task DeactivateAllByProductIdAsync(int productId, CancellationToken cancellationToken = default);

        // متدهای برای مدیریت ویژگی‌ها
        Task AddAttributeAsync(ProductAttribute attribute, CancellationToken cancellationToken = default);
        Task RemoveAttributeAsync(ProductAttribute attribute, CancellationToken cancellationToken = default);
        Task<IEnumerable<ProductAttribute>> GetAttributesAsync(int variantId, CancellationToken cancellationToken = default);

        // متدهای برای مدیریت تصاویر
        Task AddImageAsync(ImageResource image, CancellationToken cancellationToken = default);
        Task RemoveImageAsync(ImageResource image, CancellationToken cancellationToken = default);
        Task<IEnumerable<ImageResource>> GetImagesAsync(int variantId, CancellationToken cancellationToken = default);

        // متدهای برای آمار و گزارش‌گیری
        Task<int> GetTotalStockQuantityAsync(int productId, CancellationToken cancellationToken = default);
        Task<int> GetTotalSoldQuantityAsync(int productId, CancellationToken cancellationToken = default);
        Task<decimal> GetAveragePriceAsync(int productId, CancellationToken cancellationToken = default);
    }
}
