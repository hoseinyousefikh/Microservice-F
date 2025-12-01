using Catalog_Service.src._01_Domain.Core.Entities;

namespace Catalog_Service.src._01_Domain.Core.Contracts.Services
{
    public interface IProductTagService
    {
        // متدهای اصلی CRUD
        Task<ProductTag> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<IEnumerable<ProductTag>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<ProductTag>> GetByProductIdAsync(int productId, CancellationToken cancellationToken = default);
        Task<ProductTag> CreateAsync(int productId, string tagText, CancellationToken cancellationToken = default);
        Task DeleteAsync(int id, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);

        // متدهای جستجو و فیلتر
        Task<(IEnumerable<ProductTag> Tags, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize, int? productId = null, string searchTerm = null, CancellationToken cancellationToken = default);

        // متدهای برای مدیریت تگ‌های محصول
        Task<IEnumerable<string>> GetTagsByProductIdAsync(int productId, CancellationToken cancellationToken = default);
        Task<IEnumerable<int>> GetProductIdsByTagAsync(string tagText, CancellationToken cancellationToken = default);
        Task<IEnumerable<Product>> GetProductsByTagAsync(string tagText, CancellationToken cancellationToken = default);
        Task<bool> ProductHasTagAsync(int productId, string tagText, CancellationToken cancellationToken = default);

        // متدهای برای مدیریت تگ‌های محبوب
        Task<IEnumerable<string>> GetPopularTagsAsync(int count, CancellationToken cancellationToken = default);
        Task<IEnumerable<string>> GetMostUsedTagsAsync(int count, CancellationToken cancellationToken = default);
        Task<IDictionary<string, int>> GetTagUsageCountAsync(IEnumerable<string> tags, CancellationToken cancellationToken = default);

        // متدهای ویژه
        Task<int> CountAsync(CancellationToken cancellationToken = default);
        Task<int> CountByProductIdAsync(int productId, CancellationToken cancellationToken = default);
        Task<int> CountByTagTextAsync(string tagText, CancellationToken cancellationToken = default);

        // متدهای برای حذف گروهی
        Task RemoveAllProductTagsAsync(int productId, CancellationToken cancellationToken = default);
        Task RemoveAllTagsByTextAsync(string tagText, CancellationToken cancellationToken = default);

        // متدهای برای به‌روزرسانی گروهی
        Task UpdateTagTextAsync(int productId, string oldTagText, string newTagText, CancellationToken cancellationToken = default);
        Task CopyTagsBetweenProductsAsync(int sourceProductId, int targetProductId, CancellationToken cancellationToken = default);

        // متدهای برای جستجوی پیشرفته
        Task<IEnumerable<string>> GetTagsContainingAsync(string searchText, CancellationToken cancellationToken = default);
        Task<IEnumerable<string>> GetTagsStartingWithAsync(string prefix, CancellationToken cancellationToken = default);
        Task<IEnumerable<string>> GetTagsEndingWithAsync(string suffix, CancellationToken cancellationToken = default);
        Task<IEnumerable<string>> GetSimilarTagsAsync(string tagText, int count, CancellationToken cancellationToken = default);

        // متدهای برای تحلیل تگ‌ها
        Task<IEnumerable<string>> GetRecommendedTagsForProductAsync(int productId, int count, CancellationToken cancellationToken = default);
        Task<IDictionary<string, int>> GetRelatedTagsAsync(string tagText, int count, CancellationToken cancellationToken = default);
        Task<IEnumerable<string>> GetTrendingTagsAsync(DateTime sinceDate, int count, CancellationToken cancellationToken = default);
    }
}
