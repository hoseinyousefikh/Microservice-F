//using Catalog_Service.src._01_Domain.Core.Entities;
//using Catalog_Service.src._01_Domain.Core.Enums;
//using Catalog_Service.src._01_Domain.Core.Primitives;
//using Nest;

//namespace Catalog_Service.src._02_Infrastructure.Services.OpenSearch
//{

//    public interface IOpenSearchService
//    {
//        Task IndexProductAsync(Product product, CancellationToken cancellationToken = default);
//        Task UpdateProductAsync(Product product, CancellationToken cancellationToken = default);
//        Task DeleteProductAsync(int productId, CancellationToken cancellationToken = default);
//        Task<(IEnumerable<Product> Products, int TotalCount)> SearchProductsAsync(
//            string searchTerm = null,
//            int? categoryId = null,
//            int? brandId = null,
//            decimal? minPrice = null,
//            decimal? maxPrice = null,
//            ProductStatus? status = null,
//            string sortBy = null,
//            bool sortAscending = true,
//            int pageNumber = 1,
//            int pageSize = 20,
//            CancellationToken cancellationToken = default);
//    }

//    public class OpenSearchService : IOpenSearchService
//    {
//        private readonly IElasticClient _elasticClient;
//        private readonly ILogger<OpenSearchService> _logger;
//        private const string IndexName = "products";

//        public OpenSearchService(IConfiguration configuration, ILogger<OpenSearchService> logger)
//        {
//            _logger = logger;
//            var uri = new Uri(configuration["Elasticsearch:Url"]);
//            var settings = new ConnectionSettings(uri)
//                .DefaultIndex(IndexName)
//                .DisableDirectStreaming()
//                .PrettyJson();

//            _elasticClient = new ElasticClient(settings);
//        }

//        public async Task IndexProductAsync(Product product, CancellationToken cancellationToken = default)
//        {
//            try
//            {
//                var document = ProductDocument.FromEntity(product);
//                var response = await _elasticClient.IndexAsync(document, idx => idx
//                    .Index(IndexName)
//                    .Id(product.Id)
//                    .Refresh(Refresh.WaitFor), cancellationToken);

//                if (!response.IsValid)
//                {
//                    _logger.LogError("Failed to index product {ProductId}. Reason: {Reason}", product.Id, response.ServerError?.Error?.Reason);
//                }
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "An error occurred while indexing product {ProductId}", product.Id);
//                // We don't throw here to prevent breaking the main application flow if OpenSearch is down.
//            }
//        }

//        public async Task UpdateProductAsync(Product product, CancellationToken cancellationToken = default)
//        {
//            try
//            {
//                var document = ProductDocument.FromEntity(product);
//                var response = await _elasticClient.UpdateAsync<ProductDocument>(product.Id, u => u
//                    .Index(IndexName)
//                    .Doc(document)
//                    .Refresh(Refresh.WaitFor), cancellationToken);

//                if (!response.IsValid)
//                {
//                    _logger.LogError("Failed to update product {ProductId} in OpenSearch. Reason: {Reason}", product.Id, response.ServerError?.Error?.Reason);
//                }
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "An error occurred while updating product {ProductId} in OpenSearch", product.Id);
//            }
//        }

//        public async Task DeleteProductAsync(int productId, CancellationToken cancellationToken = default)
//        {
//            try
//            {
//                var response = await _elasticClient.DeleteAsync<ProductDocument>(productId, d => d
//                    .Index(IndexName)
//                    .Refresh(Refresh.WaitFor), cancellationToken);

//                if (!response.IsValid)
//                {
//                    _logger.LogError("Failed to delete product {ProductId} from OpenSearch. Reason: {Reason}", productId, response.ServerError?.Error?.Reason);
//                }
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "An error occurred while deleting product {ProductId} from OpenSearch", productId);
//            }
//        }

//        public async Task<(IEnumerable<Product> Products, int TotalCount)> SearchProductsAsync(
//            string searchTerm = null,
//            int? categoryId = null,
//            int? brandId = null,
//            decimal? minPrice = null,
//            decimal? maxPrice = null,
//            ProductStatus? status = null,
//            string sortBy = null,
//            bool sortAscending = true,
//            int pageNumber = 1,
//            int pageSize = 20,
//            CancellationToken cancellationToken = default)
//        {
//            var query = new QueryContainerDescriptor<ProductDocument>();

//            // Always filter by published status unless explicitly set otherwise
//            if (!status.HasValue)
//            {
//                query &= q => q.Term(t => t.Field(f => f.Status).Value("Published"));
//            }
//            else
//            {
//                query &= q => q.Term(t => t.Field(f => f.Status).Value(status.Value.ToString()));
//            }

//            if (categoryId.HasValue)
//                query &= q => q.Term(t => t.Field(f => f.CategoryId).Value(categoryId.Value));

//            if (brandId.HasValue)
//                query &= q => q.Term(t => t.Field(f => f.BrandId).Value(brandId.Value));

//            if (!string.IsNullOrWhiteSpace(searchTerm))
//            {
//                query &= q => q
//                    .Bool(b => b
//                        .Should(
//                            s => s.Match(m => m.Field(f => f.Name).Query(searchTerm)),
//                            s => s.Match(m => m.Field(f => f.Description).Query(searchTerm)),
//                            s => s.Match(m => m.Field(f => f.Sku).Query(searchTerm))
//                        )
//                    );
//            }

//            if (minPrice.HasValue || maxPrice.HasValue)
//            {
//                query &= q => q
//                    .Range(r => r
//                        .Field(f => f.Price)
//                        .GreaterThanOrEquals(minPrice)
//                        .LessThanOrEquals(maxPrice)
//                    );
//            }

//            var sortDescriptor = new SortDescriptor<ProductDocument>();
//            if (!string.IsNullOrWhiteSpace(sortBy))
//            {
//                switch (sortBy.ToLowerInvariant())
//                {
//                    case "name":
//                        sortDescriptor.Field(f => f.Name, sortAscending ? SortOrder.Ascending : SortOrder.Descending);
//                        break;
//                    case "price":
//                        sortDescriptor.Field(f => f.Price, sortAscending ? SortOrder.Ascending : SortOrder.Descending);
//                        break;
//                    case "date":
//                        sortDescriptor.Field(f => f.CreatedAt, sortAscending ? SortOrder.Ascending : SortOrder.Descending);
//                        break;
//                    case "rating":
//                        sortDescriptor.Field(f => f.AverageRating, sortAscending ? SortOrder.Ascending : SortOrder.Descending);
//                        break;
//                    default:
//                        sortDescriptor.Field(f => f.CreatedAt, SortOrder.Descending);
//                        break;
//                }
//            }
//            else
//            {
//                sortDescriptor.Field(f => f.CreatedAt, SortOrder.Descending);
//            }

//            var searchResponse = await _elasticClient.SearchAsync<ProductDocument>(s => s
//                .Index(IndexName)
//                .Query(q => query)
//                .Sort(sort => sortDescriptor)
//                .From((pageNumber - 1) * pageSize)
//                .Size(pageSize)
//                .TrackTotalHits(true), cancellationToken);

//            if (!searchResponse.IsValid)
//            {
//                _logger.LogError("OpenSearch search failed. Reason: {Reason}", searchResponse.ServerError?.Error?.Reason);
//                return (Enumerable.Empty<Product>(), 0);
//            }

//            var products = searchResponse.Documents.Select(doc => doc.ToEntity()).ToList();

//            return (products, (int)searchResponse.TotalHits);
//        }
//    }

//    // DTO for OpenSearch document
//    public class ProductDocument
//    {
//        public int Id { get; set; }
//        public string Name { get; set; }
//        public string Description { get; set; }
//        public decimal Price { get; set; }
//        public decimal? OriginalPrice { get; set; }
//        public string Sku { get; set; }
//        public string Slug { get; set; }
//        public string Status { get; set; }
//        public int BrandId { get; set; }
//        public string BrandName { get; set; }
//        public int CategoryId { get; set; }
//        public string CategoryName { get; set; }
//        public int StockQuantity { get; set; }
//        public string StockStatus { get; set; }
//        public bool IsFeatured { get; set; }
//        public int ViewCount { get; set; }
//        public string ImageUrl { get; set; }
//        public DimensionsDocument Dimensions { get; set; }
//        public WeightDocument Weight { get; set; }
//        public double AverageRating { get; set; }
//        public int TotalReviews { get; set; }
//        public DateTime CreatedAt { get; set; }
//        public DateTime? UpdatedAt { get; set; }
//        public DateTime? PublishedAt { get; set; }

//        public static ProductDocument FromEntity(Product product)
//        {
//            return new ProductDocument
//            {
//                Id = product.Id,
//                Name = product.Name,
//                Description = product.Description,
//                Price = product.Price.Amount,
//                OriginalPrice = product.OriginalPrice?.Amount,
//                Sku = product.Sku,
//                Slug = product.Slug?.Value,
//                Status = product.Status.ToString(),
//                BrandId = product.BrandId,
//                BrandName = product.Brand?.Name,
//                CategoryId = product.CategoryId,
//                CategoryName = product.Category?.Name,
//                StockQuantity = product.StockQuantity,
//                StockStatus = product.StockStatus.ToString(),
//                IsFeatured = product.IsFeatured,
//                ViewCount = product.ViewCount,
//                ImageUrl = product.Images.FirstOrDefault(i => i.IsPrimary)?.PublicUrl,
//                Dimensions = new DimensionsDocument
//                {
//                    Length = product.Dimensions.Length,
//                    Width = product.Dimensions.Width,
//                    Height = product.Dimensions.Height
//                },
//                Weight = new WeightDocument
//                {
//                    Value = product.Weight.Value,
//                    Unit = product.Weight.Unit
//                },
//                AverageRating = product.Reviews.Any() ? product.Reviews.Average(r => r.Rating) : 0,
//                TotalReviews = product.Reviews.Count,
//                CreatedAt = product.CreatedAt,
//                UpdatedAt = product.UpdatedAt,
//                PublishedAt = product.PublishedAt
//            };
//        }

//        public Product ToEntity()
//        {
//            // This is a simplified conversion. For a full entity, you'd need to fetch related data from the DB.
//            var product = new Product(
//                Name, Description, Money.Create(Price, "USD"), BrandId, CategoryId, Sku,
//                Dimensions.Create(Dimensions.Length, Dimensions.Width, Dimensions.Height, "cm"),
//                Weight.Create(Weight.Value, Weight.Unit), "system");

//            // Use reflection to set private/protected properties for a more complete object
//            typeof(Product).GetProperty(nameof(Product.Id))?.SetValue(product, Id);
//            typeof(Product).GetProperty(nameof(Product.Slug))?.SetValue(product, new Slug(Slug));
//            typeof(Product).GetProperty(nameof(Product.Status))?.SetValue(product, Enum.Parse<ProductStatus>(Status));
//            typeof(Product).GetProperty(nameof(Product.StockQuantity))?.SetValue(product, StockQuantity);
//            typeof(Product).GetProperty(nameof(Product.StockStatus))?.SetValue(product, Enum.Parse<StockStatus>(StockStatus));
//            typeof(Product).GetProperty(nameof(Product.IsFeatured))?.SetValue(product, IsFeatured);
//            typeof(Product).GetProperty(nameof(Product.ViewCount))?.SetValue(product, ViewCount);
//            typeof(Product).GetProperty(nameof(Product.CreatedAt))?.SetValue(product, CreatedAt);
//            typeof(Product).GetProperty(nameof(Product.UpdatedAt))?.SetValue(product, UpdatedAt);
//            typeof(Product).GetProperty(nameof(Product.PublishedAt))?.SetValue(product, PublishedAt);

//            return product;
//        }
//    }

//    public class DimensionsDocument
//    {
//        public decimal Length { get; set; }
//        public decimal Width { get; set; }
//        public decimal Height { get; set; }
//    }

//    public class WeightDocument
//    {
//        public decimal Value { get; set; }
//        public string Unit { get; set; }
//    }
//}
