//using Elasticsearch.Net;
//using HealthChecks.Elasticsearch;
//using Microsoft.Extensions.Options;
//using Nest;

//namespace Catalog_Service.src._02_Infrastructure.Search
//{
//    public class ElasticService : IElasticService
//    {
//        private readonly IElasticClient _elasticClient;
//        private readonly ILogger<ElasticService> _logger;
//        private readonly ElasticSearchOptions _options;

//        public ElasticService(IElasticClient elasticClient, IOptions<ElasticSearchOptions> options, ILogger<ElasticService> logger)
//        {
//            _elasticClient = elasticClient;
//            _logger = logger;
//            _options = options.Value;
//        }

//        public async Task IndexProductAsync(ProductDocument product, CancellationToken cancellationToken = default)
//        {
//            try
//            {
//                var response = await _elasticClient.IndexAsync(product, idx => idx
//                    .Index(_options.DefaultIndex)
//                    .Id(product.Id)
//                    .Refresh(Refresh.WaitFor), cancellationToken);

//                if (!response.IsValid)
//                {
//                    _logger.LogError("Failed to index product: {ProductId}. Error: {Error}", product.Id, response.DebugInformation);
//                    throw new Exception($"Failed to index product: {product.Id}");
//                }

//                _logger.LogInformation("Successfully indexed product: {ProductId}", product.Id);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error indexing product: {ProductId}", product.Id);
//                throw;
//            }
//        }

//        public async Task IndexProductsAsync(IEnumerable<ProductDocument> products, CancellationToken cancellationToken = default)
//        {
//            try
//            {
//                var bulkResponse = await _elasticClient.BulkAsync(b => b
//                    .Index(_options.DefaultIndex)
//                    .IndexMany(products)
//                    .Refresh(Refresh.WaitFor), cancellationToken);

//                if (!bulkResponse.IsValid)
//                {
//                    _logger.LogError("Failed to bulk index products. Errors: {Errors}", bulkResponse.DebugInformation);
//                    throw new Exception($"Failed to bulk index products: {bulkResponse.DebugInformation}");
//                }

//                _logger.LogInformation("Successfully indexed {Count} products", products.Count());
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error bulk indexing products");
//                throw;
//            }
//        }

//        public async Task UpdateProductAsync(ProductDocument product, CancellationToken cancellationToken = default)
//        {
//            try
//            {
//                var response = await _elasticClient.UpdateAsync<ProductDocument>(product.Id, u => u
//                    .Index(_options.DefaultIndex)
//                    .Doc(product)
//                    .Refresh(Refresh.WaitFor), cancellationToken);

//                if (!response.IsValid)
//                {
//                    _logger.LogError("Failed to update product: {ProductId}. Error: {Error}", product.Id, response.DebugInformation);
//                    throw new Exception($"Failed to update product: {product.Id}");
//                }

//                _logger.LogInformation("Successfully updated product: {ProductId}", product.Id);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error updating product: {ProductId}", product.Id);
//                throw;
//            }
//        }

//        public async Task DeleteProductAsync(string productId, CancellationToken cancellationToken = default)
//        {
//            try
//            {
//                var response = await _elasticClient.DeleteAsync<ProductDocument>(productId, d => d
//                    .Index(_options.DefaultIndex)
//                    .Refresh(Refresh.WaitFor), cancellationToken);

//                if (!response.IsValid && response.Result != Result.NotFound)
//                {
//                    _logger.LogError("Failed to delete product: {ProductId}. Error: {Error}", productId, response.DebugInformation);
//                    throw new Exception($"Failed to delete product: {productId}");
//                }

//                _logger.LogInformation("Successfully deleted product: {ProductId}", productId);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error deleting product: {ProductId}", productId);
//                throw;
//            }
//        }

//        public async Task<ProductSearchResult> SearchProductsAsync(ProductSearchRequest request, CancellationToken cancellationToken = default)
//        {
//            try
//            {
//                var searchResponse = await _elasticClient.SearchAsync<ProductDocument>(s => s
//                    .Index(_options.DefaultIndex)
//                    .From(request.From)
//                    .Size(request.Size)
//                    .Query(SearchQueryBuilder.BuildQuery(request))
//                    .Sort(SearchQueryBuilder.BuildSort(request))
//                    .Highlight(h => h
//                        .Fields(f => f
//                            .Field(p => p.Name)
//                            .Field(p => p.Description)
//                            .Field(p => p.CategoryName)
//                            .Field(p => p.BrandName))
//                        .PreTags("<mark>")
//                        .PostTags("</mark>")), cancellationToken);

//                if (!searchResponse.IsValid)
//                {
//                    _logger.LogError("Failed to search products. Error: {Error}", searchResponse.DebugInformation);
//                    throw new Exception($"Failed to search products: {searchResponse.DebugInformation}");
//                }

//                return new ProductSearchResult
//                {
//                    Total = searchResponse.Total,
//                    Products = searchResponse.Hits.Select(h => new ProductSearchHit
//                    {
//                        Id = h.Source.Id,
//                        Name = h.Source.Name,
//                        Description = h.Source.Description,
//                        Price = h.Source.Price,
//                        BrandName = h.Source.BrandName,
//                        CategoryName = h.Source.CategoryName,
//                        ImageUrl = h.Source.ImageUrl,
//                        Highlights = h.Highlight?.SelectMany(hl => hl.Value.Value).ToList() ?? new List<string>()
//                    }).ToList()
//                };
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error searching products");
//                throw;
//            }
//        }

//        public async Task<bool> IndexExistsAsync(string indexName, CancellationToken cancellationToken = default)
//        {
//            var response = await _elasticClient.Indices.ExistsAsync(indexName, cancellationToken);
//            return response.Exists;
//        }

//        public async Task CreateIndexAsync(string indexName, CancellationToken cancellationToken = default)
//        {
//            try
//            {
//                var response = await _elasticClient.Indices.CreateAsync(indexName, c => c
//                    .Settings(s => s
//                        .Analysis(a => a
//                            .Analyzers(ad => ad
//                                .Standard("custom_standard", sa => sa
//                                    .Tokenizer("standard")
//                                    .Filters("lowercase", "stop", "stemmer")))))
//                    .Map<ProductDocument>(m => m
//                        .AutoMap()
//                        .Properties(p => p
//                            .Text(t => t
//                                .Name(n => n.Name)
//                                .Analyzer("custom_standard"))
//                            .Text(t => t
//                                .Name(n => n.Description)
//                                .Analyzer("custom_standard"))
//                            .Keyword(k => k
//                                .Name(n => n.CategoryName))
//                            .Keyword(k => k
//                                .Name(n => n.BrandName))
//                            .Number(n => n
//                                .Name(nn => nn.Price))
//                            .Boolean(b => b
//                                .Name(n => n.IsActive))
//                            .Date(d => d
//                                .Name(n => n.CreatedAt)))), cancellationToken);

//                if (!response.IsValid)
//                {
//                    _logger.LogError("Failed to create index: {IndexName}. Error: {Error}", indexName, response.DebugInformation);
//                    throw new Exception($"Failed to create index: {indexName}");
//                }

//                _logger.LogInformation("Successfully created index: {IndexName}", indexName);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error creating index: {IndexName}", indexName);
//                throw;
//            }
//        }

//        public async Task DeleteIndexAsync(string indexName, CancellationToken cancellationToken = default)
//        {
//            try
//            {
//                var response = await _elasticClient.Indices.DeleteAsync(indexName, cancellationToken);

//                if (!response.IsValid && response.ServerError?.Status != 404)
//                {
//                    _logger.LogError("Failed to delete index: {IndexName}. Error: {Error}", indexName, response.DebugInformation);
//                    throw new Exception($"Failed to delete index: {indexName}");
//                }

//                _logger.LogInformation("Successfully deleted index: {IndexName}", indexName);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error deleting index: {IndexName}", indexName);
//                throw;
//            }
//        }
//    }
//}
