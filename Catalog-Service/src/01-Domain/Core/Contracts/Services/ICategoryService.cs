using Catalog_Service.src._01_Domain.Core.Entities;

namespace Catalog_Service.src._01_Domain.Core.Contracts.Services
{
    public interface ICategoryService
    {
        // متدهای اصلی CRUD
        Task<Category> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<Category> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);
        Task<IEnumerable<Category>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<Category>> GetActiveCategoriesAsync(CancellationToken cancellationToken = default);
        Task<Category> CreateAsync(string name, string description, int displayOrder, string createdByUserId, int? parentCategoryId = null, string? imageUrl = null, string? metaTitle = null, string? metaDescription = null, CancellationToken cancellationToken = default);
        Task UpdateAsync(int id, string name, string description, int displayOrder, string? imageUrl = null, string? metaTitle = null, string? metaDescription = null, CancellationToken cancellationToken = default);
        Task DeleteAsync(int id, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);

        // متدهای سلسله مراتبی
        Task<IEnumerable<Category>> GetRootCategoriesAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<Category>> GetSubCategoriesAsync(int parentCategoryId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Category>> GetAllDescendantsAsync(int categoryId, CancellationToken cancellationToken = default);
        Task<Category> GetParentCategoryAsync(int categoryId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Category>> GetCategoryPathAsync(int categoryId, CancellationToken cancellationToken = default);
        Task<bool> HasSubCategoriesAsync(int categoryId, CancellationToken cancellationToken = default);
        Task<bool> HasProductsAsync(int categoryId, CancellationToken cancellationToken = default);

        // متدهای صفحه‌بندی و جستجو
        Task<(IEnumerable<Category> Categories, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize, string searchTerm = null, bool onlyActive = true, int? parentCategoryId = null, string sortBy = null, bool sortAscending = true, CancellationToken cancellationToken = default);

        // متدهای ترتیب‌بندی
        Task<IEnumerable<Category>> GetOrderedByDisplayOrderAsync(CancellationToken cancellationToken = default);
        Task<int> GetMaxDisplayOrderAsync(int? parentCategoryId = null, CancellationToken cancellationToken = default);
        Task UpdateDisplayOrderAsync(int id, int displayOrder, CancellationToken cancellationToken = default);

        // متدهای مدیریت ساختار درختی
        Task MoveCategoryAsync(int categoryId, int? newParentId, CancellationToken cancellationToken = default);
        Task<bool> WouldCreateCircularReferenceAsync(int parentId, int childId, CancellationToken cancellationToken = default);

        // متدهای برای فعال/غیرفعال کردن
        Task ActivateAsync(int id, CancellationToken cancellationToken = default);
        Task DeactivateAsync(int id, CancellationToken cancellationToken = default);
        Task ActivateWithSubCategoriesAsync(int id, CancellationToken cancellationToken = default);
        Task DeactivateWithSubCategoriesAsync(int id, CancellationToken cancellationToken = default);

        // متدهای مدیریت اسلاگ
        Task SetSlugAsync(int id, string title, CancellationToken cancellationToken = default);

        // متدهای آمار و گزارش‌گیری
        Task<int> GetTotalProductsCountAsync(int categoryId, CancellationToken cancellationToken = default);
        Task<int> GetActiveProductsCountAsync(int categoryId, CancellationToken cancellationToken = default);
        Task<int> GetTotalSubCategoriesCountAsync(int categoryId, CancellationToken cancellationToken = default);
        // در فایل ICategoryService.cs
        Task<IEnumerable<Category>> GetCategoryTreeAsync(CancellationToken cancellationToken = default);
    }
}