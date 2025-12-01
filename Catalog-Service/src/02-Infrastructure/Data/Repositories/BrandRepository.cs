using Catalog_Service.src._01_Domain.Core.Contracts.Repositories;
using Catalog_Service.src._01_Domain.Core.Entities;
using Catalog_Service.src._01_Domain.Core.Enums;
using Catalog_Service.src._01_Domain.Core.Primitives;
using Catalog_Service.src._02_Infrastructure.Data.Db;
using Microsoft.EntityFrameworkCore;

namespace Catalog_Service.src._02_Infrastructure.Data.Repositories
{
    public class BrandRepository : IBrandRepository
    {
        private readonly AppDbContext _dbContext;

        public BrandRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Brand> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Brands
                .Include(b => b.Products)
                .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
        }

        public async Task<Brand> GetBySlugAsync(Slug slug, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Brands
                .Include(b => b.Products)
                .FirstOrDefaultAsync(b => b.Slug == slug, cancellationToken);
        }

        public async Task<IEnumerable<Brand>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.Brands
                .Include(b => b.Products)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Brand>> GetActiveBrandsAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.Brands
                .Where(b => b.IsActive)
                .Include(b => b.Products)
                .ToListAsync(cancellationToken);
        }

        public async Task<Brand> AddAsync(Brand brand, CancellationToken cancellationToken = default)
        {
            await _dbContext.Brands.AddAsync(brand, cancellationToken);
            return brand;
        }

        public void Update(Brand brand)
        {
            _dbContext.Brands.Update(brand);
        }

        public void Remove(Brand brand)
        {
            _dbContext.Brands.Remove(brand);
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<(IEnumerable<Brand> Brands, int TotalCount)> GetPagedAsync(
            int pageNumber,
            int pageSize,
            string searchTerm = null,
            bool onlyActive = true,
            string sortBy = null,
            bool sortAscending = true,
            CancellationToken cancellationToken = default)
        {
            var query = _dbContext.Brands.AsQueryable();

            if (onlyActive)
            {
                query = query.Where(b => b.IsActive);
            }

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(b => b.Name.Contains(searchTerm) || b.Description.Contains(searchTerm));
            }

            var totalCount = await query.CountAsync(cancellationToken);

            query = sortBy switch
            {
                "name" => sortAscending ? query.OrderBy(b => b.Name) : query.OrderByDescending(b => b.Name),
                "date" => sortAscending ? query.OrderBy(b => b.CreatedAt) : query.OrderByDescending(b => b.CreatedAt),
                _ => query.OrderBy(b => b.Name)
            };

            var brands = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Include(b => b.Products)
                .ToListAsync(cancellationToken);

            return (brands, totalCount);
        }

        public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Brands.AnyAsync(b => b.Id == id, cancellationToken);
        }

        public async Task<bool> ExistsBySlugAsync(Slug slug, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Brands.AnyAsync(b => b.Slug == slug, cancellationToken);
        }

        public async Task<bool> IsUniqueSlugAsync(Slug slug, int? excludeBrandId = null, CancellationToken cancellationToken = default)
        {
            var query = _dbContext.Brands.Where(b => b.Slug == slug);

            if (excludeBrandId.HasValue)
            {
                query = query.Where(b => b.Id != excludeBrandId.Value);
            }

            return !await query.AnyAsync(cancellationToken);
        }

        public async Task<int> CountAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.Brands.CountAsync(cancellationToken);
        }

        public async Task<int> CountActiveAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.Brands.CountAsync(b => b.IsActive, cancellationToken);
        }

        public async Task<int> CountProductsAsync(int brandId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Products.CountAsync(p => p.BrandId == brandId, cancellationToken);
        }

        public async Task<IEnumerable<Product>> GetProductsAsync(int brandId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Products
                .Where(p => p.BrandId == brandId)
                .Include(p => p.Category)
                .ToListAsync(cancellationToken);
        }

        public async Task<int> GetProductsCountAsync(int brandId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Products.CountAsync(p => p.BrandId == brandId, cancellationToken);
        }

        public async Task<bool> HasProductsAsync(int brandId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Products.AnyAsync(p => p.BrandId == brandId, cancellationToken);
        }

        public async Task ActivateAsync(int brandId, CancellationToken cancellationToken = default)
        {
            var brand = await _dbContext.Brands.FindAsync(new object[] { brandId }, cancellationToken);
            if (brand != null)
            {
                brand.Activate();
                _dbContext.Brands.Update(brand);
            }
        }

        public async Task DeactivateAsync(int brandId, CancellationToken cancellationToken = default)
        {
            var brand = await _dbContext.Brands.FindAsync(new object[] { brandId }, cancellationToken);
            if (brand != null)
            {
                brand.Deactivate();
                _dbContext.Brands.Update(brand);
            }
        }

        public async Task<decimal> GetAveragePriceAsync(int brandId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Products
                .Where(p => p.BrandId == brandId && p.Status == ProductStatus.Published)
                .AverageAsync(p => p.Price.Amount, cancellationToken);
        }

        public async Task<int> GetTotalViewCountAsync(int brandId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Products
                .Where(p => p.BrandId == brandId)
                .SumAsync(p => p.ViewCount, cancellationToken);
        }

        public async Task<int> GetTotalReviewsCountAsync(int brandId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Products
                .Where(p => p.BrandId == brandId)
                .SelectMany(p => p.Reviews)
                .CountAsync(cancellationToken);
        }

        public async Task<double> GetAverageRatingAsync(int brandId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Products
                .Where(p => p.BrandId == brandId)
                .SelectMany(p => p.Reviews)
                .AverageAsync(r => r.Rating, cancellationToken);
        }

        public async Task<IEnumerable<Brand>> GetTopBrandsByProductsCountAsync(int count, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Brands
                .Where(b => b.IsActive)
                .Select(b => new { Brand = b, ProductCount = b.Products.Count })
                .OrderByDescending(x => x.ProductCount)
                .Take(count)
                .Select(x => x.Brand)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Brand>> GetTopBrandsByRatingAsync(int count, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Brands
                .Where(b => b.IsActive)
                .Select(b => new
                {
                    Brand = b,
                    AvgRating = b.Products.SelectMany(p => p.Reviews).Average(r => (double?)r.Rating) ?? 0
                })
                .OrderByDescending(x => x.AvgRating)
                .Take(count)
                .Select(x => x.Brand)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Brand>> GetTopBrandsBySalesAsync(int count, CancellationToken cancellationToken = default)
        {
            // در یک پیاده‌سازی واقعی، این متد باید با داده‌های فروش کار کند
            return await _dbContext.Brands
                .Where(b => b.IsActive)
                .Select(b => new { Brand = b, TotalViews = b.Products.Sum(p => p.ViewCount) })
                .OrderByDescending(x => x.TotalViews)
                .Take(count)
                .Select(x => x.Brand)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Brand>> GetBrandsWithMostProductsAsync(int count, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Brands
                .Where(b => b.IsActive)
                .Select(b => new { Brand = b, ProductCount = b.Products.Count })
                .OrderByDescending(x => x.ProductCount)
                .Take(count)
                .Select(x => x.Brand)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Brand>> GetBrandsWithHighestAveragePriceAsync(int count, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Brands
                .Where(b => b.IsActive)
                .Select(b => new
                {
                    Brand = b,
                    AvgPrice = b.Products.Where(p => p.Status == ProductStatus.Published).Average(p => p.Price.Amount)
                })
                .OrderByDescending(x => x.AvgPrice)
                .Take(count)
                .Select(x => x.Brand)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Brand>> GetBrandsWithLowestAveragePriceAsync(int count, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Brands
                .Where(b => b.IsActive)
                .Select(b => new
                {
                    Brand = b,
                    AvgPrice = b.Products.Where(p => p.Status == ProductStatus.Published).Average(p => p.Price.Amount)
                })
                .OrderBy(x => x.AvgPrice)
                .Take(count)
                .Select(x => x.Brand)
                .ToListAsync(cancellationToken);
        }
    }
}
