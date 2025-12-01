using Catalog_Service.src._01_Domain.Core.Primitives;

namespace Catalog_Service.src._01_Domain.Core.Contracts.Services
{
    public interface ISlugService
    {
        // متدهای اصلی
        Task<Slug> CreateSlugAsync(string title, CancellationToken cancellationToken = default);
        Task<Slug> CreateUniqueSlugAsync(string title, Func<string, Task<bool>> uniquenessChecker, CancellationToken cancellationToken = default);
        Task<Slug> CreateUniqueSlugForProductAsync(string title, int? excludeProductId = null, CancellationToken cancellationToken = default);
        Task<Slug> CreateUniqueSlugForCategoryAsync(string title, int? excludeCategoryId = null, CancellationToken cancellationToken = default);
        Task<Slug> CreateUniqueSlugForBrandAsync(string title, int? excludeBrandId = null, CancellationToken cancellationToken = default);

        // متدهای برای بررسی یکتایی
        Task<bool> IsUniqueProductSlugAsync(string slug, int? excludeProductId = null, CancellationToken cancellationToken = default);
        Task<bool> IsUniqueCategorySlugAsync(string slug, int? excludeCategoryId = null, CancellationToken cancellationToken = default);
        Task<bool> IsUniqueBrandSlugAsync(string slug, int? excludeBrandId = null, CancellationToken cancellationToken = default);

        // متدهای برای به‌روزرسانی اسلاگ
        Task<Slug> UpdateProductSlugAsync(int productId, string title, CancellationToken cancellationToken = default);
        Task<Slug> UpdateCategorySlugAsync(int categoryId, string title, CancellationToken cancellationToken = default);
        Task<Slug> UpdateBrandSlugAsync(int brandId, string title, CancellationToken cancellationToken = default);

        // متدهای کمکی
        string GenerateSlugFromTitle(string title);
        bool IsValidSlug(string slug);
        Task<string> EnsureUniquenessAsync(string baseSlug, Func<string, Task<bool>> uniquenessChecker, CancellationToken cancellationToken = default);
    }
}
