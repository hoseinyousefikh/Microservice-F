using Catalog_Service.src._01_Domain.Core.Entities;
using Catalog_Service.src._01_Domain.Core.Primitives;

namespace Catalog_Service.src._01_Domain.Core.Contracts.Repositories
{
    public interface IBrandRepository
    {
        // متدهای اصلی CRUD
        Task<Brand> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<Brand> GetBySlugAsync(Slug slug, CancellationToken cancellationToken = default);
        Task<IEnumerable<Brand>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<Brand>> GetActiveBrandsAsync(CancellationToken cancellationToken = default);
        Task<Brand> AddAsync(Brand brand, CancellationToken cancellationToken = default);
        void Update(Brand brand);
        void Remove(Brand brand);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        // متدهای جستجو و فیلتر
        Task<(IEnumerable<Brand> Brands, int TotalCount)> GetPagedAsync(
            int pageNumber,
            int pageSize,
            string searchTerm = null,
            bool onlyActive = true,
            string sortBy = null,
            bool sortAscending = true,
            CancellationToken cancellationToken = default);

        // متدهای ویژه
        Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
        Task<bool> ExistsBySlugAsync(Slug slug, CancellationToken cancellationToken = default);
        Task<bool> IsUniqueSlugAsync(Slug slug, int? excludeBrandId = null, CancellationToken cancellationToken = default);
        Task<int> CountAsync(CancellationToken cancellationToken = default);
        Task<int> CountActiveAsync(CancellationToken cancellationToken = default);
        Task<int> CountProductsAsync(int brandId, CancellationToken cancellationToken = default);

        // متدهای برای مدیریت محصولات
        Task<IEnumerable<Product>> GetProductsAsync(int brandId, CancellationToken cancellationToken = default);
        Task<int> GetProductsCountAsync(int brandId, CancellationToken cancellationToken = default);
        Task<bool> HasProductsAsync(int brandId, CancellationToken cancellationToken = default);

        // متدهای برای فعال/غیرفعال کردن
        Task ActivateAsync(int brandId, CancellationToken cancellationToken = default);
        Task DeactivateAsync(int brandId, CancellationToken cancellationToken = default);

        // متدهای برای آمار و گزارش‌گیری
        Task<decimal> GetAveragePriceAsync(int brandId, CancellationToken cancellationToken = default);
        Task<int> GetTotalViewCountAsync(int brandId, CancellationToken cancellationToken = default);
        Task<int> GetTotalReviewsCountAsync(int brandId, CancellationToken cancellationToken = default);
        Task<double> GetAverageRatingAsync(int brandId, CancellationToken cancellationToken = default);

        // متدهای برای برندهای محبوب
        Task<IEnumerable<Brand>> GetTopBrandsByProductsCountAsync(int count, CancellationToken cancellationToken = default);
        Task<IEnumerable<Brand>> GetTopBrandsByRatingAsync(int count, CancellationToken cancellationToken = default);
        Task<IEnumerable<Brand>> GetTopBrandsBySalesAsync(int count, CancellationToken cancellationToken = default);

        // متدهای برای جستجوی پیشرفته
        Task<IEnumerable<Brand>> GetBrandsWithMostProductsAsync(int count, CancellationToken cancellationToken = default);
        Task<IEnumerable<Brand>> GetBrandsWithHighestAveragePriceAsync(int count, CancellationToken cancellationToken = default);
        Task<IEnumerable<Brand>> GetBrandsWithLowestAveragePriceAsync(int count, CancellationToken cancellationToken = default);
    }
}
