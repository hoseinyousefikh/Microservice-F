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
        Task<Product> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<Product?> GetBySkuAsync(string sku, CancellationToken cancellationToken = default);
        Task<Product> GetBySlugAsync(Slug slug, CancellationToken cancellationToken = default);
        Task<IEnumerable<Product>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<Product>> GetActiveProductsAsync(CancellationToken cancellationToken = default);
        Task<Product> AddAsync(Product product, CancellationToken cancellationToken = default);
        void Update(Product product);
        void Remove(Product product);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<Product>> GetByCategoryAsync(int categoryId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Product>> GetByBrandAsync(int brandId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Product>> GetByPriceRangeAsync(Money minPrice, Money maxPrice, CancellationToken cancellationToken = default);
        Task<IEnumerable<Product>> GetFeaturedProductsAsync(int count, CancellationToken cancellationToken = default);
        Task<IEnumerable<Product>> GetNewestProductsAsync(int count, CancellationToken cancellationToken = default);
        Task<IEnumerable<Product>> GetBestSellingProductsAsync(int count, CancellationToken cancellationToken = default);
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
        Task<IEnumerable<Product>> GetByStatusAsync(ProductStatus status, CancellationToken cancellationToken = default);
        Task<IEnumerable<Product>> GetOutOfStockProductsAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<Product>> GetLowStockProductsAsync(int threshold, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
        Task<bool> ExistsBySkuAsync(string sku, CancellationToken cancellationToken = default);
        Task<bool> ExistsBySlugAsync(Slug slug, CancellationToken cancellationToken = default);
        Task<int> CountAsync(CancellationToken cancellationToken = default);
        Task<int> CountByCategoryAsync(int categoryId, CancellationToken cancellationToken = default);
        Task<int> CountByBrandAsync(int brandId, CancellationToken cancellationToken = default);
        Task UpdateStockQuantityAsync(int productId, int quantity, CancellationToken cancellationToken = default);
        Task UpdateStockStatusAsync(int productId, StockStatus status, CancellationToken cancellationToken = default);
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
        Task<decimal> GetAveragePriceByCategoryAsync(int categoryId, CancellationToken cancellationToken = default);
        Task<decimal> GetAveragePriceByBrandAsync(int brandId, CancellationToken cancellationToken = default);
        Task<int> GetViewCountAsync(int productId, CancellationToken cancellationToken = default);
        Task IncrementViewCountAsync(int productId, CancellationToken cancellationToken = default);
        Task<double> GetAverageRatingAsync(int productId, CancellationToken cancellationToken = default);
        Task<int> GetTotalReviewsCountAsync(int productId, CancellationToken cancellationToken = default);
        Task<IDictionary<int, int>> GetRatingDistributionAsync(int productId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Product>> GetProductsWithPrimaryImagesAsync(IEnumerable<int> productIds, CancellationToken cancellationToken = default);
        Task<IEnumerable<Product>> GetProductsByTagAsync(string tagText, CancellationToken cancellationToken = default);
    }
}