using Catalog_Service.src._01_Domain.Core.Entities;
using Catalog_Service.src._01_Domain.Core.Enums;
using Catalog_Service.src._01_Domain.Core.Primitives;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Catalog_Service.src._01_Domain.Core.Contracts.Repositories
{
    public interface IProductRepository
    {
        // متدهای اصلی CRUD
        Task<Product> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<Product> GetBySkuAsync(string sku, CancellationToken cancellationToken = default);
        Task<Product> GetBySlugAsync(Slug slug, CancellationToken cancellationToken = default);
        Task<IEnumerable<Product>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<Product>> GetActiveProductsAsync(CancellationToken cancellationToken = default);
        Task<Product> AddAsync(Product product, CancellationToken cancellationToken = default);
        void Update(Product product);
        void Remove(Product product);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        // متدهای جستجو و فیلتر
        Task<IEnumerable<Product>> GetByCategoryAsync(int categoryId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Product>> GetByBrandAsync(int brandId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Product>> GetByPriceRangeAsync(Money minPrice, Money maxPrice, CancellationToken cancellationToken = default);
        Task<IEnumerable<Product>> GetFeaturedProductsAsync(int count, CancellationToken cancellationToken = default);
        Task<IEnumerable<Product>> GetNewestProductsAsync(int count, CancellationToken cancellationToken = default);
        Task<IEnumerable<Product>> GetBestSellingProductsAsync(int count, CancellationToken cancellationToken = default);

        // متدهای صفحه‌بندی
        Task<(IEnumerable<Product> Products, int TotalCount)> GetPagedAsync(
            int pageNumber,
            int pageSize,
            string searchTerm = null,
            int? categoryId = null,
            int? brandId = null,
            ProductStatus? status = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            string sortBy = null,
            bool sortAscending = true,
            CancellationToken cancellationToken = default);

        // متدهای ویژه وضعیت محصول
        Task<IEnumerable<Product>> GetByStatusAsync(ProductStatus status, CancellationToken cancellationToken = default);
        Task<IEnumerable<Product>> GetOutOfStockProductsAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<Product>> GetLowStockProductsAsync(int threshold, CancellationToken cancellationToken = default);

        // متدهای پیشرفته
        Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
        Task<bool> ExistsBySkuAsync(string sku, CancellationToken cancellationToken = default);
        Task<bool> ExistsBySlugAsync(Slug slug, CancellationToken cancellationToken = default);
        Task<int> CountAsync(CancellationToken cancellationToken = default);
        Task<int> CountByCategoryAsync(int categoryId, CancellationToken cancellationToken = default);
        Task<int> CountByBrandAsync(int brandId, CancellationToken cancellationToken = default);

        // متدهای برای مدیریت موجودی
        Task UpdateStockQuantityAsync(int productId, int quantity, CancellationToken cancellationToken = default);
        Task UpdateStockStatusAsync(int productId, StockStatus status, CancellationToken cancellationToken = default);

        // متدهای برای مدیریت ویژگی‌های محصول
        Task AddVariantAsync(ProductVariant variant, CancellationToken cancellationToken = default);
        Task RemoveVariantAsync(ProductVariant variant, CancellationToken cancellationToken = default);
        Task AddImageAsync(ImageResource image, CancellationToken cancellationToken = default);
        Task RemoveImageAsync(ImageResource image, CancellationToken cancellationToken = default);
        Task AddAttributeAsync(ProductAttribute attribute, CancellationToken cancellationToken = default);
        Task RemoveAttributeAsync(ProductAttribute attribute, CancellationToken cancellationToken = default);
        Task AddTagAsync(ProductTag tag, CancellationToken cancellationToken = default);
        Task RemoveTagAsync(ProductTag tag, CancellationToken cancellationToken = default);
        Task AddReviewAsync(ProductReview review, CancellationToken cancellationToken = default);
        Task RemoveReviewAsync(ProductReview review, CancellationToken cancellationToken = default);

        // متدهای برای آمار و گزارش‌گیری
        Task<decimal> GetAveragePriceByCategoryAsync(int categoryId, CancellationToken cancellationToken = default);
        Task<decimal> GetAveragePriceByBrandAsync(int brandId, CancellationToken cancellationToken = default);
        Task<int> GetViewCountAsync(int productId, CancellationToken cancellationToken = default);
        Task IncrementViewCountAsync(int productId, CancellationToken cancellationToken = default);

        // متدهای جدید برای بررسی‌ها
        Task<double> GetAverageRatingAsync(int productId, CancellationToken cancellationToken = default);
        Task<int> GetTotalReviewsCountAsync(int productId, CancellationToken cancellationToken = default);
        Task<IDictionary<int, int>> GetRatingDistributionAsync(int productId, CancellationToken cancellationToken = default);
    }
}