using Catalog_Service.src._01_Domain.Core.Contracts.Repositories;
using Catalog_Service.src._01_Domain.Core.Entities;
using Catalog_Service.src._01_Domain.Core.Enums;
using Catalog_Service.src._02_Infrastructure.Data.Db;
using Microsoft.EntityFrameworkCore;

namespace Catalog_Service.src._02_Infrastructure.Data.Repositories
{
    public class ProductTagRepository : IProductTagRepository
    {
        private readonly AppDbContext _dbContext;

        public ProductTagRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ProductTag> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.ProductTags
                .Include(pt => pt.Product)
                .FirstOrDefaultAsync(pt => pt.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<ProductTag>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.ProductTags
                .Include(pt => pt.Product)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<ProductTag>> GetByProductIdAsync(int productId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.ProductTags
                .Include(pt => pt.Product)
                .Where(pt => pt.ProductId == productId)
                .OrderBy(pt => pt.TagText)
                .ToListAsync(cancellationToken);
        }

        public async Task<ProductTag> AddAsync(ProductTag tag, CancellationToken cancellationToken = default)
        {
            await _dbContext.ProductTags.AddAsync(tag, cancellationToken);
            return tag;
        }

        public void Remove(ProductTag tag)
        {
            _dbContext.ProductTags.Remove(tag);
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<(IEnumerable<ProductTag> Tags, int TotalCount)> GetPagedAsync(
            int pageNumber,
            int pageSize,
            int? productId = null,
            string searchTerm = null,
            CancellationToken cancellationToken = default)
        {
            var query = _dbContext.ProductTags.AsQueryable();

            if (productId.HasValue)
            {
                query = query.Where(pt => pt.ProductId == productId.Value);
            }

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(pt => pt.TagText.Contains(searchTerm));
            }

            var totalCount = await query.CountAsync(cancellationToken);
            var tags = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Include(pt => pt.Product)
                .OrderBy(pt => pt.TagText)
                .ToListAsync(cancellationToken);

            return (tags, totalCount);
        }

        public async Task<IEnumerable<string>> GetTagsByProductIdAsync(int productId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.ProductTags
                .Where(pt => pt.ProductId == productId)
                .Select(pt => pt.TagText)
                .Distinct()
                .OrderBy(tag => tag)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<int>> GetProductIdsByTagAsync(string tagText, CancellationToken cancellationToken = default)
        {
            return await _dbContext.ProductTags
                .Where(pt => pt.TagText == tagText)
                .Select(pt => pt.ProductId)
                .Distinct()
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Product>> GetProductsByTagAsync(string tagText, CancellationToken cancellationToken = default)
        {
            var productIds = await GetProductIdsByTagAsync(tagText, cancellationToken);
            return await _dbContext.Products
                .Where(p => productIds.Contains(p.Id) && p.Status == ProductStatus.Published)
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> ProductHasTagAsync(int productId, string tagText, CancellationToken cancellationToken = default)
        {
            return await _dbContext.ProductTags.AnyAsync(pt => pt.ProductId == productId && pt.TagText == tagText, cancellationToken);
        }

        public async Task<IEnumerable<string>> GetPopularTagsAsync(int count, CancellationToken cancellationToken = default)
        {
            return await _dbContext.ProductTags
                .GroupBy(pt => pt.TagText)
                .Select(g => new { TagText = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(count)
                .Select(x => x.TagText)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<string>> GetMostUsedTagsAsync(int count, CancellationToken cancellationToken = default)
        {
            return await GetPopularTagsAsync(count, cancellationToken);
        }

        public async Task<IDictionary<string, int>> GetTagUsageCountAsync(IEnumerable<string> tags, CancellationToken cancellationToken = default)
        {
            return await _dbContext.ProductTags
                .Where(pt => tags.Contains(pt.TagText))
                .GroupBy(pt => pt.TagText)
                .Select(g => new { TagText = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.TagText, x => x.Count, cancellationToken);
        }

        public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.ProductTags.AnyAsync(pt => pt.Id == id, cancellationToken);
        }

        public async Task<bool> ExistsByProductAndTagAsync(int productId, string tagText, CancellationToken cancellationToken = default)
        {
            return await _dbContext.ProductTags.AnyAsync(pt => pt.ProductId == productId && pt.TagText == tagText, cancellationToken);
        }

        public async Task<int> CountAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.ProductTags.CountAsync(cancellationToken);
        }

        public async Task<int> CountByProductIdAsync(int productId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.ProductTags.CountAsync(pt => pt.ProductId == productId, cancellationToken);
        }

        public async Task<int> CountByTagTextAsync(string tagText, CancellationToken cancellationToken = default)
        {
            return await _dbContext.ProductTags.CountAsync(pt => pt.TagText == tagText, cancellationToken);
        }

        public async Task RemoveAllProductTagsAsync(int productId, CancellationToken cancellationToken = default)
        {
            var tags = await _dbContext.ProductTags
                .Where(pt => pt.ProductId == productId)
                .ToListAsync(cancellationToken);

            _dbContext.ProductTags.RemoveRange(tags);
        }

        public async Task RemoveAllTagsByTextAsync(string tagText, CancellationToken cancellationToken = default)
        {
            var tags = await _dbContext.ProductTags
                .Where(pt => pt.TagText == tagText)
                .ToListAsync(cancellationToken);

            _dbContext.ProductTags.RemoveRange(tags);
        }

        public async Task UpdateTagTextAsync(int productId, string oldTagText, string newTagText, CancellationToken cancellationToken = default)
        {
            var tags = await _dbContext.ProductTags
                .Where(pt => pt.ProductId == productId && pt.TagText == oldTagText)
                .ToListAsync(cancellationToken);

            foreach (var tag in tags)
            {
                tag.UpdateTagText(newTagText);
                _dbContext.ProductTags.Update(tag);
            }
        }

        public async Task CopyTagsBetweenProductsAsync(int sourceProductId, int targetProductId, CancellationToken cancellationToken = default)
        {
            var sourceTags = await _dbContext.ProductTags
                .Where(pt => pt.ProductId == sourceProductId)
                .ToListAsync(cancellationToken);

            foreach (var tag in sourceTags)
            {
                if (!await ProductHasTagAsync(targetProductId, tag.TagText, cancellationToken))
                {
                    var newTag = new ProductTag(targetProductId, tag.TagText);
                    await _dbContext.ProductTags.AddAsync(newTag, cancellationToken);
                }
            }
        }

        public async Task<IEnumerable<string>> GetTagsContainingAsync(string searchText, CancellationToken cancellationToken = default)
        {
            return await _dbContext.ProductTags
                .Where(pt => pt.TagText.Contains(searchText))
                .Select(pt => pt.TagText)
                .Distinct()
                .OrderBy(tag => tag)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<string>> GetTagsStartingWithAsync(string prefix, CancellationToken cancellationToken = default)
        {
            return await _dbContext.ProductTags
                .Where(pt => pt.TagText.StartsWith(prefix))
                .Select(pt => pt.TagText)
                .Distinct()
                .OrderBy(tag => tag)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<string>> GetTagsEndingWithAsync(string suffix, CancellationToken cancellationToken = default)
        {
            return await _dbContext.ProductTags
                .Where(pt => pt.TagText.EndsWith(suffix))
                .Select(pt => pt.TagText)
                .Distinct()
                .OrderBy(tag => tag)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<string>> GetSimilarTagsAsync(string tagText, int count, CancellationToken cancellationToken = default)
        {
            return await _dbContext.ProductTags
                .Where(pt => pt.TagText.Contains(tagText) || pt.TagText.StartsWith(tagText))
                .Select(pt => pt.TagText)
                .Distinct()
                .OrderBy(tag => tag)
                .Take(count)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<string>> GetRecommendedTagsForProductAsync(int productId, int count, CancellationToken cancellationToken = default)
        {
            var currentTags = await GetTagsByProductIdAsync(productId, cancellationToken);
            var similarProducts = await _dbContext.Products
                .Where(p => p.Id != productId && p.Status == ProductStatus.Published)
                .Where(p => p.Tags.Any(t => currentTags.Contains(t.TagText)))
                .Take(10)
                .SelectMany(p => p.Tags)
                .Select(t => t.TagText)
                .Distinct()
                .ToListAsync(cancellationToken);

            var recommendedTags = similarProducts
                .Except(currentTags)
                .Take(count)
                .ToList();

            return recommendedTags;
        }

        public async Task<IDictionary<string, int>> GetRelatedTagsAsync(string tagText, int count, CancellationToken cancellationToken = default)
        {
            var productIdsWithTag = await GetProductIdsByTagAsync(tagText, cancellationToken);
            var relatedTags = await _dbContext.ProductTags
                .Where(pt => productIdsWithTag.Contains(pt.ProductId) && pt.TagText != tagText)
                .GroupBy(pt => pt.TagText)
                .Select(g => new { TagText = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(count)
                .ToDictionaryAsync(x => x.TagText, x => x.Count, cancellationToken);

            return relatedTags;
        }

        public async Task<IEnumerable<string>> GetTrendingTagsAsync(DateTime sinceDate, int count, CancellationToken cancellationToken = default)
        {
            return await _dbContext.ProductTags
                .Where(pt => pt.CreatedAt >= sinceDate)
                .GroupBy(pt => pt.TagText)
                .Select(g => new { TagText = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(count)
                .Select(x => x.TagText)
                .ToListAsync(cancellationToken);
        }
    }
}
