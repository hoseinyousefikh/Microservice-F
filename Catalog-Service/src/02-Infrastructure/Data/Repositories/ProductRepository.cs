using Catalog_Service.src._01_Domain.Core.Contracts.Repositories;
using Catalog_Service.src._01_Domain.Core.Entities;
using Catalog_Service.src._01_Domain.Core.Enums;
using Catalog_Service.src._01_Domain.Core.Primitives;
using Catalog_Service.src._02_Infrastructure.Data.Db;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Catalog_Service.src._02_Infrastructure.Data.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly IImageRepository _imageRepository;

        public ProductRepository(AppDbContext dbContext, IImageRepository imageRepository)
        {
            _dbContext = dbContext;
            _imageRepository = imageRepository;
        }

        public async Task<Product> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            var product = await _dbContext.Products
                .IgnoreQueryFilters()
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Include(p => p.Variants)
                .Include(p => p.Attributes)
                .Include(p => p.Reviews)
                .Include(p => p.Tags)
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

            if (product != null)
            {
                var primaryImage = await _imageRepository.GetPrimaryImageAsync(product.Id, cancellationToken);
                if (primaryImage != null && !product.Images.Any(img => img.Id == primaryImage.Id))
                {
                    product.AddImage(primaryImage);
                }
            }

            return product;
        }

        public async Task<Product?> GetBySkuAsync(string sku, CancellationToken cancellationToken = default)
        {
            var product = await _dbContext.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Sku == sku, cancellationToken);

            if (product != null)
            {
                var primaryImage = await _imageRepository.GetPrimaryImageAsync(product.Id, cancellationToken);
                if (primaryImage != null && !product.Images.Any(img => img.Id == primaryImage.Id))
                {
                    product.AddImage(primaryImage);
                }
            }

            return product;
        }

        public async Task<Product> GetBySlugAsync(Slug slug, CancellationToken cancellationToken = default)
        {
            var product = await _dbContext.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Include(p => p.Variants)
                .Include(p => p.Attributes)
                .Include(p => p.Reviews)
                .Include(p => p.Tags)
                .FirstOrDefaultAsync(p => p.Slug == slug, cancellationToken);

            if (product != null)
            {
                var primaryImage = await _imageRepository.GetPrimaryImageAsync(product.Id, cancellationToken);
                if (primaryImage != null && !product.Images.Any(img => img.Id == primaryImage.Id))
                {
                    product.AddImage(primaryImage);
                }
            }

            return product;
        }

        public async Task<IEnumerable<Product>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var products = await _dbContext.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .ToListAsync(cancellationToken);

            foreach (var product in products)
            {
                var primaryImage = await _imageRepository.GetPrimaryImageAsync(product.Id, cancellationToken);
                if (primaryImage != null && !product.Images.Any(img => img.Id == primaryImage.Id))
                {
                    product.AddImage(primaryImage);
                }
            }

            return products;
        }

        public async Task<IEnumerable<Product>> GetActiveProductsAsync(CancellationToken cancellationToken = default)
        {
            var products = await _dbContext.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Where(p => p.Status == ProductStatus.Published)
                .ToListAsync(cancellationToken);

            foreach (var product in products)
            {
                var primaryImage = await _imageRepository.GetPrimaryImageAsync(product.Id, cancellationToken);
                if (primaryImage != null && !product.Images.Any(img => img.Id == primaryImage.Id))
                {
                    product.AddImage(primaryImage);
                }
            }

            return products;
        }

        public async Task<Product> AddAsync(Product product, CancellationToken cancellationToken = default)
        {
            await _dbContext.Products.AddAsync(product, cancellationToken);
            return product;
        }

        public void Update(Product product)
        {
            _dbContext.Products.Update(product);
        }

        public void Remove(Product product)
        {
            _dbContext.Products.Remove(product);
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<IEnumerable<Product>> GetByCategoryAsync(int categoryId, CancellationToken cancellationToken = default)
        {
            var products = await _dbContext.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Where(p => p.CategoryId == categoryId && p.Status == ProductStatus.Published)
                .ToListAsync(cancellationToken);

            foreach (var product in products)
            {
                var primaryImage = await _imageRepository.GetPrimaryImageAsync(product.Id, cancellationToken);
                if (primaryImage != null && !product.Images.Any(img => img.Id == primaryImage.Id))
                {
                    product.AddImage(primaryImage);
                }
            }

            return products;
        }

        public async Task<IEnumerable<Product>> GetByBrandAsync(int brandId, CancellationToken cancellationToken = default)
        {
            var products = await _dbContext.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Where(p => p.BrandId == brandId && p.Status == ProductStatus.Published)
                .ToListAsync(cancellationToken);

            foreach (var product in products)
            {
                var primaryImage = await _imageRepository.GetPrimaryImageAsync(product.Id, cancellationToken);
                if (primaryImage != null && !product.Images.Any(img => img.Id == primaryImage.Id))
                {
                    product.AddImage(primaryImage);
                }
            }

            return products;
        }

        public async Task<IEnumerable<Product>> GetByPriceRangeAsync(Money minPrice, Money maxPrice, CancellationToken cancellationToken = default)
        {
            var products = await _dbContext.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Where(p => p.Price.Amount >= minPrice.Amount && p.Price.Amount <= maxPrice.Amount && p.Status == ProductStatus.Published)
                .ToListAsync(cancellationToken);

            foreach (var product in products)
            {
                var primaryImage = await _imageRepository.GetPrimaryImageAsync(product.Id, cancellationToken);
                if (primaryImage != null && !product.Images.Any(img => img.Id == primaryImage.Id))
                {
                    product.AddImage(primaryImage);
                }
            }

            return products;
        }

        public async Task<IEnumerable<Product>> GetFeaturedProductsAsync(int count, CancellationToken cancellationToken = default)
        {
            var products = await _dbContext.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Where(p => p.IsFeatured && p.Status == ProductStatus.Published)
                .OrderByDescending(p => p.CreatedAt)
                .Take(count)
                .ToListAsync(cancellationToken);

            foreach (var product in products)
            {
                var primaryImage = await _imageRepository.GetPrimaryImageAsync(product.Id, cancellationToken);
                if (primaryImage != null && !product.Images.Any(img => img.Id == primaryImage.Id))
                {
                    product.AddImage(primaryImage);
                }
            }

            return products;
        }

        public async Task<IEnumerable<Product>> GetNewestProductsAsync(int count, CancellationToken cancellationToken = default)
        {
            var products = await _dbContext.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Where(p => p.Status == ProductStatus.Published)
                .OrderByDescending(p => p.CreatedAt)
                .Take(count)
                .ToListAsync(cancellationToken);

            foreach (var product in products)
            {
                var primaryImage = await _imageRepository.GetPrimaryImageAsync(product.Id, cancellationToken);
                if (primaryImage != null && !product.Images.Any(img => img.Id == primaryImage.Id))
                {
                    product.AddImage(primaryImage);
                }
            }

            return products;
        }

        public async Task<IEnumerable<Product>> GetBestSellingProductsAsync(int count, CancellationToken cancellationToken = default)
        {
            var products = await _dbContext.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Where(p => p.Status == ProductStatus.Published)
                .OrderByDescending(p => p.ViewCount)
                .Take(count)
                .ToListAsync(cancellationToken);

            foreach (var product in products)
            {
                var primaryImage = await _imageRepository.GetPrimaryImageAsync(product.Id, cancellationToken);
                if (primaryImage != null && !product.Images.Any(img => img.Id == primaryImage.Id))
                {
                    product.AddImage(primaryImage);
                }
            }

            return products;
        }

        public async Task<(IEnumerable<Product> Products, int TotalCount)> GetPagedAsync(
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
            CancellationToken cancellationToken = default)
        {
            var dbQuery = _dbContext.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Include(p => p.Reviews)
                .Include(p => p.Images)   
                .AsQueryable();

            if (status.HasValue)
            {
                dbQuery = dbQuery.Where(p => p.Status == status.Value);
            }
            else
            {
                dbQuery = dbQuery.Where(p => p.Status == ProductStatus.Published);
            }

            if (categoryId.HasValue)
            {
                dbQuery = dbQuery.Where(p => p.CategoryId == categoryId.Value);
            }

            if (brandId.HasValue)
            {
                dbQuery = dbQuery.Where(p => p.BrandId == brandId.Value);
            }

            var productsFromDb = await dbQuery.ToListAsync(cancellationToken);

            IEnumerable<Product> filteredProducts = productsFromDb;

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.Trim().ToLower();

                filteredProducts = filteredProducts
                    .Where(p =>
                        (p.Name != null && p.Name.ToLower().Contains(searchTerm)) ||
                        (p.Slug != null && p.Slug.Value.ToLower().Contains(searchTerm))
                    )
                    .ToList();
            }

            if (minPrice.HasValue)
            {
                filteredProducts = filteredProducts.Where(p => p.Price.Amount >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                filteredProducts = filteredProducts.Where(p => p.Price.Amount <= maxPrice.Value);
            }

            var sortedProducts = sortBy?.ToLower() switch
            {
                "name" => sortAscending
                    ? filteredProducts.OrderBy(p => p.Name)
                    : filteredProducts.OrderByDescending(p => p.Name),
                "price" => sortAscending
                    ? filteredProducts.OrderBy(p => p.Price.Amount)
                    : filteredProducts.OrderByDescending(p => p.Price.Amount),
                "date" => sortAscending
                    ? filteredProducts.OrderBy(p => p.CreatedAt)
                    : filteredProducts.OrderByDescending(p => p.CreatedAt),
                "rating" => sortAscending
                    ? filteredProducts.OrderBy(p => p.Reviews.Any() ? p.Reviews.Average(r => r.Rating) : 0)
                    : filteredProducts.OrderByDescending(p => p.Reviews.Any() ? p.Reviews.Average(r => r.Rating) : 0),
                _ => filteredProducts.OrderByDescending(p => p.CreatedAt)
            };

            var totalCount = sortedProducts.Count();

            var pagedProducts = sortedProducts
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            foreach (var product in pagedProducts)
            {
                var primaryImage = await _imageRepository.GetPrimaryImageAsync(product.Id, cancellationToken);
                if (primaryImage != null && !product.Images.Any(img => img.Id == primaryImage.Id))
                {
                    product.AddImage(primaryImage);
                }
            }

            return (pagedProducts, totalCount);
        }

        public async Task<IEnumerable<Product>> GetByStatusAsync(ProductStatus status, CancellationToken cancellationToken = default)
        {
            var products = await _dbContext.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Where(p => p.Status == status)
                .ToListAsync(cancellationToken);

            foreach (var product in products)
            {
                var primaryImage = await _imageRepository.GetPrimaryImageAsync(product.Id, cancellationToken);
                if (primaryImage != null && !product.Images.Any(img => img.Id == primaryImage.Id))
                {
                    product.AddImage(primaryImage);
                }
            }

            return products;
        }

        public async Task<IEnumerable<Product>> GetOutOfStockProductsAsync(CancellationToken cancellationToken = default)
        {
            var products = await _dbContext.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Where(p => p.StockStatus == StockStatus.OutOfStock)
                .ToListAsync(cancellationToken);

            foreach (var product in products)
            {
                var primaryImage = await _imageRepository.GetPrimaryImageAsync(product.Id, cancellationToken);
                if (primaryImage != null && !product.Images.Any(img => img.Id == primaryImage.Id))
                {
                    product.AddImage(primaryImage);
                }
            }

            return products;
        }

        public async Task<IEnumerable<Product>> GetLowStockProductsAsync(int threshold, CancellationToken cancellationToken = default)
        {
            var products = await _dbContext.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Where(p => p.StockQuantity <= threshold && p.StockQuantity > 0)
                .ToListAsync(cancellationToken);

            foreach (var product in products)
            {
                var primaryImage = await _imageRepository.GetPrimaryImageAsync(product.Id, cancellationToken);
                if (primaryImage != null && !product.Images.Any(img => img.Id == primaryImage.Id))
                {
                    product.AddImage(primaryImage);
                }
            }

            return products;
        }

        public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Products.AnyAsync(p => p.Id == id, cancellationToken);
        }

        public async Task<bool> ExistsBySkuAsync(string sku, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Products.AnyAsync(p => p.Sku == sku, cancellationToken);
        }

        public async Task<bool> ExistsBySlugAsync(Slug slug, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Products.AnyAsync(p => p.Slug == slug, cancellationToken);
        }

        public async Task<int> CountAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.Products.CountAsync(cancellationToken);
        }

        public async Task<int> CountByCategoryAsync(int categoryId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Products.CountAsync(p => p.CategoryId == categoryId, cancellationToken);
        }

        public async Task<int> CountByBrandAsync(int brandId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Products.CountAsync(p => p.BrandId == brandId, cancellationToken);
        }

        public async Task UpdateStockQuantityAsync(int productId, int quantity, CancellationToken cancellationToken = default)
        {
            var product = await _dbContext.Products.FindAsync(new object[] { productId }, cancellationToken);
            if (product != null)
            {
                product.UpdateStock(quantity);
                _dbContext.Products.Update(product);
            }
        }

        public async Task UpdateStockStatusAsync(int productId, StockStatus status, CancellationToken cancellationToken = default)
        {
            var product = await _dbContext.Products.FindAsync(new object[] { productId }, cancellationToken);
            if (product != null)
            {
                product.SetStockStatus(status);
                _dbContext.Products.Update(product);
            }
        }

        public async Task AddVariantAsync(ProductVariant variant, CancellationToken cancellationToken = default)
        {
            await _dbContext.ProductVariants.AddAsync(variant, cancellationToken);
        }

        public async Task RemoveVariantAsync(ProductVariant variant, CancellationToken cancellationToken = default)
        {
            _dbContext.ProductVariants.Remove(variant);
            await Task.CompletedTask;
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

        public async Task AddAttributeAsync(ProductAttribute attribute, CancellationToken cancellationToken = default)
        {
            await _dbContext.ProductAttributes.AddAsync(attribute, cancellationToken);
        }

        public async Task RemoveAttributeAsync(ProductAttribute attribute, CancellationToken cancellationToken = default)
        {
            _dbContext.ProductAttributes.Remove(attribute);
            await Task.CompletedTask;
        }

        public async Task AddTagAsync(ProductTag tag, CancellationToken cancellationToken = default)
        {
            await _dbContext.ProductTags.AddAsync(tag, cancellationToken);
        }

        public async Task RemoveTagAsync(ProductTag tag, CancellationToken cancellationToken = default)
        {
            _dbContext.ProductTags.Remove(tag);
            await Task.CompletedTask;
        }

        public async Task AddReviewAsync(ProductReview review, CancellationToken cancellationToken = default)
        {
            await _dbContext.ProductReviews.AddAsync(review, cancellationToken);
        }

        public async Task RemoveReviewAsync(ProductReview review, CancellationToken cancellationToken = default)
        {
            _dbContext.ProductReviews.Remove(review);
            await Task.CompletedTask;
        }

        public async Task<decimal> GetAveragePriceByCategoryAsync(int categoryId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Products
                .Where(p => p.CategoryId == categoryId && p.Status == ProductStatus.Published)
                .AverageAsync(p => p.Price.Amount, cancellationToken);
        }

        public async Task<decimal> GetAveragePriceByBrandAsync(int brandId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Products
                .Where(p => p.BrandId == brandId && p.Status == ProductStatus.Published)
                .AverageAsync(p => p.Price.Amount, cancellationToken);
        }

        public async Task<int> GetViewCountAsync(int productId, CancellationToken cancellationToken = default)
        {
            var product = await _dbContext.Products.FindAsync(new object[] { productId }, cancellationToken);
            return product?.ViewCount ?? 0;
        }

        public async Task IncrementViewCountAsync(int productId, CancellationToken cancellationToken = default)
        {
            var product = await _dbContext.Products.FindAsync(new object[] { productId }, cancellationToken);
            if (product != null)
            {
                product.IncrementViewCount();
                _dbContext.Products.Update(product);
            }
        }

        public async Task<double> GetAverageRatingAsync(int productId, CancellationToken cancellationToken = default)
        {
            var approvedReviews = await _dbContext.ProductReviews
                .Where(r => r.ProductId == productId && r.Status == ReviewStatus.Approved)
                .Select(r => r.Rating)
                .ToListAsync(cancellationToken);

            if (!approvedReviews.Any())
                return 0.0;

            return approvedReviews.Average();
        }

        public async Task<int> GetTotalReviewsCountAsync(int productId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.ProductReviews
                .Where(r => r.ProductId == productId && r.Status == ReviewStatus.Approved)
                .CountAsync(cancellationToken);
        }

        public async Task<IDictionary<int, int>> GetRatingDistributionAsync(int productId, CancellationToken cancellationToken = default)
        {
            var distribution = await _dbContext.ProductReviews
                .Where(r => r.ProductId == productId && r.Status == ReviewStatus.Approved)
                .GroupBy(r => r.Rating)
                .Select(g => new { Rating = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Rating, x => x.Count, cancellationToken);

            for (int i = 1; i <= 5; i++)
            {
                if (!distribution.ContainsKey(i))
                {
                    distribution[i] = 0;
                }
            }

            return distribution;
        }

        public async Task<IEnumerable<Product>> GetProductsWithPrimaryImagesAsync(
            IEnumerable<int> productIds,
            CancellationToken cancellationToken = default)
        {
            var products = await _dbContext.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Where(p => productIds.Contains(p.Id))
                .ToListAsync(cancellationToken);

            foreach (var product in products)
            {
                var primaryImage = await _imageRepository.GetPrimaryImageAsync(product.Id, cancellationToken);
                if (primaryImage != null && !product.Images.Any(img => img.Id == primaryImage.Id))
                {
                    product.AddImage(primaryImage);
                }
            }

            return products;
        }

        public async Task<IEnumerable<Product>> GetProductsByTagAsync(
            string tagText,
            CancellationToken cancellationToken = default)
        {
            var productIds = await _dbContext.ProductTags
                .Where(t => t.TagText.Equals(tagText, StringComparison.OrdinalIgnoreCase))
                .Select(t => t.ProductId)
                .Distinct()
                .ToListAsync(cancellationToken);

            if (!productIds.Any())
                return Enumerable.Empty<Product>();

            return await GetProductsWithPrimaryImagesAsync(productIds, cancellationToken);
        }
    }
}