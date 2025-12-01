using Catalog_Service.src._01_Domain.Core.Contracts.Repositories;
using Catalog_Service.src._01_Domain.Core.Entities;
using Catalog_Service.src._01_Domain.Core.Enums;
using Catalog_Service.src._02_Infrastructure.Data.Db;
using Microsoft.EntityFrameworkCore;

namespace Catalog_Service.src._02_Infrastructure.Data.Repositories
{
    public class ProductVariantRepository : IProductVariantRepository
    {
        private readonly AppDbContext _dbContext;

        public ProductVariantRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ProductVariant> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.ProductVariants
                .Include(pv => pv.Product)
                .Include(pv => pv.Attributes)
                .Include(pv => pv.ImageUrl)
                .FirstOrDefaultAsync(pv => pv.Id == id, cancellationToken);
        }

        public async Task<ProductVariant> GetBySkuAsync(string sku, CancellationToken cancellationToken = default)
        {
            return await _dbContext.ProductVariants
                .Include(pv => pv.Product)
                .FirstOrDefaultAsync(pv => pv.Sku == sku, cancellationToken);
        }

        public async Task<IEnumerable<ProductVariant>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.ProductVariants
                .Include(pv => pv.Product)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<ProductVariant>> GetByProductIdAsync(int productId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.ProductVariants
                .Include(pv => pv.Product)
                .Include(pv => pv.Attributes)
                .Include(pv => pv.ImageUrl)
                .Where(pv => pv.ProductId == productId)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<ProductVariant>> GetActiveVariantsAsync(int productId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.ProductVariants
                .Include(pv => pv.Product)
                .Include(pv => pv.Attributes)
                .Include(pv => pv.ImageUrl)
                .Where(pv => pv.ProductId == productId && pv.IsActive)
                .ToListAsync(cancellationToken);
        }

        public async Task<ProductVariant> AddAsync(ProductVariant variant, CancellationToken cancellationToken = default)
        {
            await _dbContext.ProductVariants.AddAsync(variant, cancellationToken);
            return variant;
        }

        public void Update(ProductVariant variant)
        {
            _dbContext.ProductVariants.Update(variant);
        }

        public void Remove(ProductVariant variant)
        {
            _dbContext.ProductVariants.Remove(variant);
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<(IEnumerable<ProductVariant> Variants, int TotalCount)> GetPagedAsync(
            int pageNumber,
            int pageSize,
            int? productId = null,
            bool onlyActive = true,
            string sortBy = null,
            bool sortAscending = true,
            CancellationToken cancellationToken = default)
        {
            var query = _dbContext.ProductVariants.AsQueryable();

            if (productId.HasValue)
            {
                query = query.Where(pv => pv.ProductId == productId.Value);
            }

            if (onlyActive)
            {
                query = query.Where(pv => pv.IsActive);
            }

            var totalCount = await query.CountAsync(cancellationToken);

            query = sortBy switch
            {
                "name" => sortAscending ? query.OrderBy(pv => pv.Name) : query.OrderByDescending(pv => pv.Name),
                "price" => sortAscending ? query.OrderBy(pv => pv.Price.Amount) : query.OrderByDescending(pv => pv.Price.Amount),
                "stock" => sortAscending ? query.OrderBy(pv => pv.StockQuantity) : query.OrderByDescending(pv => pv.StockQuantity),
                "date" => sortAscending ? query.OrderBy(pv => pv.CreatedAt) : query.OrderByDescending(pv => pv.CreatedAt),
                _ => query.OrderBy(pv => pv.Name)
            };

            var variants = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Include(pv => pv.Product)
                .ToListAsync(cancellationToken);

            return (variants, totalCount);
        }

        public async Task<IEnumerable<ProductVariant>> GetOutOfStockVariantsAsync(int productId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.ProductVariants
                .Where(pv => pv.ProductId == productId && pv.StockStatus == StockStatus.OutOfStock)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<ProductVariant>> GetLowStockVariantsAsync(int productId, int threshold, CancellationToken cancellationToken = default)
        {
            return await _dbContext.ProductVariants
                .Where(pv => pv.ProductId == productId && pv.StockQuantity <= threshold && pv.StockQuantity > 0)
                .ToListAsync(cancellationToken);
        }

        public async Task UpdateStockQuantityAsync(int variantId, int quantity, CancellationToken cancellationToken = default)
        {
            var variant = await _dbContext.ProductVariants.FindAsync(new object[] { variantId }, cancellationToken);
            if (variant != null)
            {
                variant.UpdateStock(quantity);
                _dbContext.ProductVariants.Update(variant);
            }
        }

        public async Task UpdateStockStatusAsync(int variantId, StockStatus status, CancellationToken cancellationToken = default)
        {
            var variant = await _dbContext.ProductVariants.FindAsync(new object[] { variantId }, cancellationToken);
            if (variant != null)
            {
                variant.SetStockStatus(status);
                _dbContext.ProductVariants.Update(variant);
            }
        }

        public async Task<IEnumerable<ProductVariant>> GetVariantsInPriceRangeAsync(
            int productId,
            decimal minPrice,
            decimal maxPrice,
            CancellationToken cancellationToken = default)
        {
            return await _dbContext.ProductVariants
                .Where(pv => pv.ProductId == productId &&
                           pv.Price.Amount >= minPrice &&
                           pv.Price.Amount <= maxPrice)
                .ToListAsync(cancellationToken);
        }

        public async Task<ProductVariant> GetCheapestVariantAsync(int productId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.ProductVariants
                .Where(pv => pv.ProductId == productId && pv.IsActive)
                .OrderBy(pv => pv.Price.Amount)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<ProductVariant> GetMostExpensiveVariantAsync(int productId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.ProductVariants
                .Where(pv => pv.ProductId == productId && pv.IsActive)
                .OrderByDescending(pv => pv.Price.Amount)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<decimal> GetMinPriceAsync(int productId, CancellationToken cancellationToken = default)
        {
            var minPrice = await _dbContext.ProductVariants
                .Where(pv => pv.ProductId == productId && pv.IsActive)
                .MinAsync(pv => pv.Price.Amount, cancellationToken);

            return minPrice;
        }

        public async Task<decimal> GetMaxPriceAsync(int productId, CancellationToken cancellationToken = default)
        {
            var maxPrice = await _dbContext.ProductVariants
                .Where(pv => pv.ProductId == productId && pv.IsActive)
                .MaxAsync(pv => pv.Price.Amount, cancellationToken);

            return maxPrice;
        }

        public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.ProductVariants.AnyAsync(pv => pv.Id == id, cancellationToken);
        }

        public async Task<bool> ExistsBySkuAsync(string sku, CancellationToken cancellationToken = default)
        {
            return await _dbContext.ProductVariants.AnyAsync(pv => pv.Sku == sku, cancellationToken);
        }

        public async Task<bool> IsUniqueSkuAsync(string sku, int? excludeVariantId = null, CancellationToken cancellationToken = default)
        {
            var query = _dbContext.ProductVariants.Where(pv => pv.Sku == sku);

            if (excludeVariantId.HasValue)
            {
                query = query.Where(pv => pv.Id != excludeVariantId.Value);
            }

            return !await query.AnyAsync(cancellationToken);
        }

        public async Task<int> CountAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.ProductVariants.CountAsync(cancellationToken);
        }

        public async Task<int> CountByProductIdAsync(int productId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.ProductVariants.CountAsync(pv => pv.ProductId == productId, cancellationToken);
        }

        public async Task<int> CountActiveByProductIdAsync(int productId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.ProductVariants.CountAsync(pv => pv.ProductId == productId && pv.IsActive, cancellationToken);
        }

        public async Task ActivateAsync(int variantId, CancellationToken cancellationToken = default)
        {
            var variant = await _dbContext.ProductVariants.FindAsync(new object[] { variantId }, cancellationToken);
            if (variant != null)
            {
                variant.Activate();
                _dbContext.ProductVariants.Update(variant);
            }
        }

        public async Task DeactivateAsync(int variantId, CancellationToken cancellationToken = default)
        {
            var variant = await _dbContext.ProductVariants.FindAsync(new object[] { variantId }, cancellationToken);
            if (variant != null)
            {
                variant.Deactivate();
                _dbContext.ProductVariants.Update(variant);
            }
        }

        public async Task ActivateAllByProductIdAsync(int productId, CancellationToken cancellationToken = default)
        {
            await _dbContext.ProductVariants
                .Where(pv => pv.ProductId == productId)
                .ForEachAsync(pv => pv.Activate(), cancellationToken);
        }

        public async Task DeactivateAllByProductIdAsync(int productId, CancellationToken cancellationToken = default)
        {
            await _dbContext.ProductVariants
                .Where(pv => pv.ProductId == productId)
                .ForEachAsync(pv => pv.Deactivate(), cancellationToken);
        }

        public async Task AddAttributeAsync(ProductAttribute attribute, CancellationToken cancellationToken = default)
        {
            await _dbContext.ProductAttributes.AddAsync(attribute, cancellationToken);
        }

        public async Task RemoveAttributeAsync(ProductAttribute attribute, CancellationToken cancellationToken = default)
        {
            _dbContext.ProductAttributes.Remove(attribute);
            await Task.CompletedTask;
        }

        public async Task<IEnumerable<ProductAttribute>> GetAttributesAsync(int variantId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.ProductAttributes
                .Where(pa => pa.ProductVariantId == variantId)
                .ToListAsync(cancellationToken);
        }

        public async Task AddImageAsync(ImageResource image, CancellationToken cancellationToken = default)
        {
            await _dbContext.ImageResources.AddAsync(image, cancellationToken);
        }

        public async Task RemoveImageAsync(ImageResource image, CancellationToken cancellationToken = default)
        {
            _dbContext.ImageResources.Remove(image);
            await Task.CompletedTask;
        }

        public async Task<IEnumerable<ImageResource>> GetImagesAsync(int variantId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.ImageResources
                .Where(i => EF.Property<int?>(i, "ProductVariantId") == variantId)
                .OrderBy(i => i.IsPrimary ? 0 : 1)
                .ToListAsync(cancellationToken);
        }

        public async Task<int> GetTotalStockQuantityAsync(int productId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.ProductVariants
                .Where(pv => pv.ProductId == productId)
                .SumAsync(pv => pv.StockQuantity, cancellationToken);
        }

        public async Task<int> GetTotalSoldQuantityAsync(int productId, CancellationToken cancellationToken = default)
        {
            // در یک پیاده‌سازی واقعی، این متد باید با داده‌های فروش کار کند
            return 0;
        }

        public async Task<decimal> GetAveragePriceAsync(int productId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.ProductVariants
                .Where(pv => pv.ProductId == productId && pv.IsActive)
                .AverageAsync(pv => pv.Price.Amount, cancellationToken);
        }
    }
}
