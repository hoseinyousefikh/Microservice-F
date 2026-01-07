using Catalog_Service.src._01_Domain.Core.Entities;

namespace Catalog_Service.src._01_Domain.Core.Contracts.Services
{
    public interface IBrandService
    {
        // متدهای اصلی CRUD
        Task<Brand> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<Brand> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);
        Task<IEnumerable<Brand>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<Brand>> GetActiveBrandsAsync(CancellationToken cancellationToken = default);
        Task<Brand> CreateAsync(string name, string description, string createdByUserId, string? logoUrl = null, string? websiteUrl = null, string? metaTitle = null, string? metaDescription = null, CancellationToken cancellationToken = default);
        Task UpdateAsync(int id, string name, string description, string? logoUrl = null, string? websiteUrl = null, string? metaTitle = null, string? metaDescription = null, CancellationToken cancellationToken = default);
        Task DeleteAsync(int id, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);

        // متدهای جستجو و فیلتر
        Task<(IEnumerable<Brand> Brands, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize, string searchTerm = null, bool onlyActive = true, string sortBy = null, bool sortAscending = true, CancellationToken cancellationToken = default);

        // متدهای مدیریت محصولات
        Task<IEnumerable<Product>> GetProductsAsync(int brandId, CancellationToken cancellationToken = default);
        Task<int> GetProductsCountAsync(int brandId, CancellationToken cancellationToken = default);
        Task<bool> HasProductsAsync(int brandId, CancellationToken cancellationToken = default);

        // متدهای برای فعال/غیرفعال کردن
        Task ActivateAsync(int id, CancellationToken cancellationToken = default);
        Task DeactivateAsync(int id, CancellationToken cancellationToken = default);

        // متدهای مدیریت اسلاگ
        Task SetSlugAsync(int id, string title, CancellationToken cancellationToken = default);

        // متدهای آمار و گزارش‌گیری
        Task<decimal> GetAveragePriceAsync(int brandId, CancellationToken cancellationToken = default);
        Task<int> GetTotalViewCountAsync(int brandId, CancellationToken cancellationToken = default);
        Task<int> GetTotalReviewsCountAsync(int brandId, CancellationToken cancellationToken = default);
        Task<double> GetAverageRatingAsync(int brandId, CancellationToken cancellationToken = default);

        // متدهای برای برندهای محبوب
        Task<IEnumerable<Brand>> GetTopBrandsByProductsCountAsync(int count, CancellationToken cancellationToken = default);
        Task<IEnumerable<Brand>> GetTopBrandsByRatingAsync(int count, CancellationToken cancellationToken = default);
        Task<IEnumerable<Brand>> GetTopBrandsBySalesAsync(int count, CancellationToken cancellationToken = default);
    }
}