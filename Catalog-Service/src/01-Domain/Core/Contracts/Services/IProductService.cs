using Catalog_Service.src._01_Domain.Core.Entities;
using Catalog_Service.src._01_Domain.Core.Enums;
using Catalog_Service.src._01_Domain.Core.Primitives;

namespace Catalog_Service.src._01_Domain.Core.Contracts.Services
{
    public interface IProductService
    {
        // متدهای اصلی CRUD
        Task<Product> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<Product> GetBySkuAsync(string sku, CancellationToken cancellationToken = default);
        Task<Product> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);
        Task<IEnumerable<Product>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<Product> CreateAsync(string name, string description, Money price, int brandId, int categoryId, string sku, Dimensions dimensions, Weight weight, string? metaTitle = null, string? metaDescription = null, CancellationToken cancellationToken = default);
        Task UpdateAsync(int id, string name, string description, Money price, Money? originalPrice, Dimensions dimensions, Weight weight, string? metaTitle = null, string? metaDescription = null, CancellationToken cancellationToken = default);
        Task DeleteAsync(int id, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);

        // متدهای جستجو و فیلتر
        Task<(IEnumerable<Product> Products, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize, string searchTerm = null, int? categoryId = null, int? brandId = null, ProductStatus? status = null, decimal? minPrice = null, decimal? maxPrice = null, string sortBy = null, bool sortAscending = true, CancellationToken cancellationToken = default);
        Task<IEnumerable<Product>> GetByCategoryAsync(int categoryId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Product>> GetByBrandAsync(int brandId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Product>> GetByPriceRangeAsync(Money minPrice, Money maxPrice, CancellationToken cancellationToken = default);
        Task<IEnumerable<Product>> GetFeaturedProductsAsync(int count, CancellationToken cancellationToken = default);
        Task<IEnumerable<Product>> GetNewestProductsAsync(int count, CancellationToken cancellationToken = default);
        Task<IEnumerable<Product>> GetBestSellingProductsAsync(int count, CancellationToken cancellationToken = default);

        // متدهای مدیریت وضعیت محصول
        Task PublishAsync(int id, CancellationToken cancellationToken = default);
        Task UnpublishAsync(int id, CancellationToken cancellationToken = default);
        Task ArchiveAsync(int id, CancellationToken cancellationToken = default);
        Task SetAsFeaturedAsync(int id, CancellationToken cancellationToken = default);
        Task RemoveFromFeaturedAsync(int id, CancellationToken cancellationToken = default);
        Task IncrementViewCountAsync(int id, CancellationToken cancellationToken = default);

        // متدهای مدیریت موجودی
        Task UpdateStockQuantityAsync(int id, int quantity, CancellationToken cancellationToken = default);
        Task UpdateStockStatusAsync(int id, StockStatus status, CancellationToken cancellationToken = default);

        // متدهای مدیریت متغیرهای محصول
        Task<ProductVariant> AddVariantAsync(int productId, string sku, string name, Money price, Dimensions dimensions, Weight weight, string? imageUrl = null, Money? originalPrice = null, CancellationToken cancellationToken = default);
        Task UpdateVariantAsync(int variantId, string name, Money price, Money? originalPrice, Dimensions dimensions, Weight weight, string? imageUrl = null, CancellationToken cancellationToken = default);
        Task DeleteVariantAsync(int variantId, CancellationToken cancellationToken = default);
        Task ActivateVariantAsync(int variantId, CancellationToken cancellationToken = default);
        Task DeactivateVariantAsync(int variantId, CancellationToken cancellationToken = default);

        // متدهای مدیریت تصاویر
        Task<ImageResource> AddImageAsync(int productId, string originalFileName, string fileExtension, string storagePath, string publicUrl, long fileSize, int width, int height, ImageType imageType, string? altText = null, bool isPrimary = false, CancellationToken cancellationToken = default);
        Task UpdateImageAsync(int imageId, string? altText = null, bool? isPrimary = null, CancellationToken cancellationToken = default);
        Task DeleteImageAsync(int imageId, CancellationToken cancellationToken = default);
        Task SetPrimaryImageAsync(int imageId, CancellationToken cancellationToken = default);

        // متدهای مدیریت ویژگی‌ها
        Task<ProductAttribute> AddAttributeAsync(int productId, string name, string value, CancellationToken cancellationToken = default);
        Task UpdateAttributeAsync(int attributeId, string name, string value, CancellationToken cancellationToken = default);
        Task DeleteAttributeAsync(int attributeId, CancellationToken cancellationToken = default);

        // متدهای مدیریت تگ‌ها
        Task<ProductTag> AddTagAsync(int productId, string tagText, CancellationToken cancellationToken = default);
        Task RemoveTagAsync(int tagId, CancellationToken cancellationToken = default);
        Task<IEnumerable<string>> GetTagsByProductIdAsync(int productId, CancellationToken cancellationToken = default);

        // متدهای مدیریت بازبینی‌ها
        Task<ProductReview> AddReviewAsync(int productId, string userId, string title, string comment, int rating, bool isVerifiedPurchase = false, CancellationToken cancellationToken = default);
        Task UpdateReviewAsync(int reviewId, string title, string comment, int rating, CancellationToken cancellationToken = default);
        Task DeleteReviewAsync(int reviewId, CancellationToken cancellationToken = default);
        Task ApproveReviewAsync(int reviewId, CancellationToken cancellationToken = default);
        Task RejectReviewAsync(int reviewId, CancellationToken cancellationToken = default);
        Task MarkReviewAsVerifiedAsync(int reviewId, CancellationToken cancellationToken = default);
        Task IncrementHelpfulVotesAsync(int reviewId, CancellationToken cancellationToken = default);

        // متدهای مدیریت اسلاگ
        Task SetSlugAsync(int id, string title, CancellationToken cancellationToken = default);

        // متدهای آمار و گزارش‌گیری
        Task<decimal> GetAveragePriceByCategoryAsync(int categoryId, CancellationToken cancellationToken = default);
        Task<decimal> GetAveragePriceByBrandAsync(int brandId, CancellationToken cancellationToken = default);
        Task<int> GetViewCountAsync(int id, CancellationToken cancellationToken = default);
        Task<double> GetAverageRatingAsync(int id, CancellationToken cancellationToken = default);
        Task<int> GetTotalReviewsCountAsync(int id, CancellationToken cancellationToken = default);
        Task<IDictionary<int, int>> GetRatingDistributionAsync(int id, CancellationToken cancellationToken = default);
    }
}
