using Catalog_Service.src._01_Domain.Core.Entities;

namespace Catalog_Service.src._01_Domain.Core.Contracts.Services
{
    public interface IProductAttributeService
    {
        // متدهای اصلی CRUD
        Task<ProductAttribute> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<IEnumerable<ProductAttribute>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<ProductAttribute>> GetByProductIdAsync(int productId, CancellationToken cancellationToken = default);
        Task<IEnumerable<ProductAttribute>> GetByProductVariantIdAsync(int productVariantId, CancellationToken cancellationToken = default);
        Task<ProductAttribute> CreateAsync(int productId, string name, string value, int? productVariantId = null, bool isVariantSpecific = false, CancellationToken cancellationToken = default);
        Task UpdateAsync(int id, string name, string value, CancellationToken cancellationToken = default);
        Task DeleteAsync(int id, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);

        // متدهای جستجو و فیلتر
        Task<(IEnumerable<ProductAttribute> Attributes, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize, int? productId = null, int? productVariantId = null, string name = null, bool onlyVariantSpecific = false, CancellationToken cancellationToken = default);

        // متدهای برای مدیریت ویژگی‌های محصول
        Task<IEnumerable<ProductAttribute>> GetProductAttributesAsync(int productId, CancellationToken cancellationToken = default);
        Task<IEnumerable<ProductAttribute>> GetVariantAttributesAsync(int productVariantId, CancellationToken cancellationToken = default);
        Task<IEnumerable<string>> GetUniqueAttributeNamesAsync(int productId, CancellationToken cancellationToken = default);
        Task<IEnumerable<string>> GetUniqueAttributeValuesAsync(int productId, string attributeName, CancellationToken cancellationToken = default);

        // متدهای برای مدیریت ویژگی‌های مشترک
        Task<IEnumerable<ProductAttribute>> GetCommonAttributesAsync(IEnumerable<int> productIds, CancellationToken cancellationToken = default);
        Task<IEnumerable<ProductAttribute>> GetDistinctAttributesAsync(int productId, CancellationToken cancellationToken = default);

        // متدهای برای حذف گروهی
        Task RemoveAllProductAttributesAsync(int productId, CancellationToken cancellationToken = default);
        Task RemoveAllVariantAttributesAsync(int productVariantId, CancellationToken cancellationToken = default);
        Task RemoveAllAttributesByNameAsync(int productId, string attributeName, CancellationToken cancellationToken = default);

        // متدهای برای به‌روزرسانی گروهی
        Task UpdateAllAttributesValueAsync(int productId, string attributeName, string newValue, CancellationToken cancellationToken = default);
        Task UpdateAllVariantAttributesValueAsync(int productVariantId, string attributeName, string newValue, CancellationToken cancellationToken = default);

        // متدهای برای کپی ویژگی‌ها
        Task CopyProductAttributesToVariantAsync(int productId, int productVariantId, CancellationToken cancellationToken = default);
        Task CopyVariantAttributesToProductAsync(int productVariantId, int productId, CancellationToken cancellationToken = default);
        Task CopyAttributesBetweenProductsAsync(int sourceProductId, int targetProductId, CancellationToken cancellationToken = default);

        // متدهای برای جستجوی پیشرفته
        Task<IEnumerable<ProductAttribute>> GetAttributesContainingValueAsync(int productId, string searchValue, CancellationToken cancellationToken = default);
        Task<IEnumerable<ProductAttribute>> GetAttributesByNameAsync(int productId, string attributeName, CancellationToken cancellationToken = default);
        Task<IEnumerable<ProductAttribute>> GetAttributesByValueAsync(int productId, string attributeValue, CancellationToken cancellationToken = default);
    }
}
