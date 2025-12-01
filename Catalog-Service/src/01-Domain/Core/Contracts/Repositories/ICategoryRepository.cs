using Catalog_Service.src._01_Domain.Core.Entities;
using Catalog_Service.src._01_Domain.Core.Primitives;

namespace Catalog_Service.src._01_Domain.Core.Contracts.Repositories
{
    public interface ICategoryRepository
    {
        // متدهای اصلی CRUD
        Task<Category> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<Category> GetBySlugAsync(Slug slug, CancellationToken cancellationToken = default);
        Task<IEnumerable<Category>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<Category>> GetActiveCategoriesAsync(CancellationToken cancellationToken = default);
        Task<Category> AddAsync(Category category, CancellationToken cancellationToken = default);
        void Update(Category category);
        void Remove(Category category);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        // متدهای سلسله مراتبی
        Task<IEnumerable<Category>> GetRootCategoriesAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<Category>> GetSubCategoriesAsync(int parentCategoryId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Category>> GetAllDescendantsAsync(int categoryId, CancellationToken cancellationToken = default);
        Task<Category> GetParentCategoryAsync(int categoryId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Category>> GetCategoryPathAsync(int categoryId, CancellationToken cancellationToken = default);
        Task<bool> HasSubCategoriesAsync(int categoryId, CancellationToken cancellationToken = default);
        Task<bool> HasProductsAsync(int categoryId, CancellationToken cancellationToken = default);

        // متدهای صفحه‌بندی و جستجو
        Task<(IEnumerable<Category> Categories, int TotalCount)> GetPagedAsync(
            int pageNumber,
            int pageSize,
            string searchTerm = null,
            bool onlyActive = true,
            int? parentCategoryId = null,
            string sortBy = null,
            bool sortAscending = true,
            CancellationToken cancellationToken = default);

        // متدهای ترتیب‌بندی
        Task<IEnumerable<Category>> GetOrderedByDisplayOrderAsync(CancellationToken cancellationToken = default);
        Task<int> GetMaxDisplayOrderAsync(int? parentCategoryId = null, CancellationToken cancellationToken = default);
        Task UpdateDisplayOrderAsync(int categoryId, int displayOrder, CancellationToken cancellationToken = default);

        // متدهای ویژه
        Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
        Task<bool> ExistsBySlugAsync(Slug slug, CancellationToken cancellationToken = default);
        Task<bool> IsUniqueSlugAsync(Slug slug, int? excludeCategoryId = null, CancellationToken cancellationToken = default);
        Task<int> CountAsync(CancellationToken cancellationToken = default);
        Task<int> CountActiveAsync(CancellationToken cancellationToken = default);
        Task<int> CountSubCategoriesAsync(int categoryId, CancellationToken cancellationToken = default);
        Task<int> CountProductsAsync(int categoryId, CancellationToken cancellationToken = default);

        // متدهای برای مدیریت ساختار درختی
        Task<bool> IsAncestorOfAsync(int ancestorId, int descendantId, CancellationToken cancellationToken = default);
        Task<bool> IsDescendantOfAsync(int descendantId, int ancestorId, CancellationToken cancellationToken = default);
        Task<bool> WouldCreateCircularReferenceAsync(int parentId, int childId, CancellationToken cancellationToken = default);
        Task MoveCategoryAsync(int categoryId, int? newParentId, CancellationToken cancellationToken = default);

        // متدهای برای فعال/غیرفعال کردن
        Task ActivateAsync(int categoryId, CancellationToken cancellationToken = default);
        Task DeactivateAsync(int categoryId, CancellationToken cancellationToken = default);
        Task ActivateWithSubCategoriesAsync(int categoryId, CancellationToken cancellationToken = default);
        Task DeactivateWithSubCategoriesAsync(int categoryId, CancellationToken cancellationToken = default);

        // متدهای برای آمار و گزارش‌گیری
        Task<int> GetTotalProductsCountAsync(int categoryId, CancellationToken cancellationToken = default);
        Task<int> GetActiveProductsCountAsync(int categoryId, CancellationToken cancellationToken = default);
        Task<int> GetTotalSubCategoriesCountAsync(int categoryId, CancellationToken cancellationToken = default);
    }
}
