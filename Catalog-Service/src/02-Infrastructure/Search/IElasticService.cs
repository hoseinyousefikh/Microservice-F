namespace Catalog_Service.src._02_Infrastructure.Search
{
    public interface IElasticService
    {
        Task IndexProductAsync(ProductDocument product, CancellationToken cancellationToken = default);
        Task IndexProductsAsync(IEnumerable<ProductDocument> products, CancellationToken cancellationToken = default);
        Task UpdateProductAsync(ProductDocument product, CancellationToken cancellationToken = default);
        Task DeleteProductAsync(string productId, CancellationToken cancellationToken = default);
        Task<ProductSearchResult> SearchProductsAsync(ProductSearchRequest request, CancellationToken cancellationToken = default);
        Task<bool> IndexExistsAsync(string indexName, CancellationToken cancellationToken = default);
        Task CreateIndexAsync(string indexName, CancellationToken cancellationToken = default);
        Task DeleteIndexAsync(string indexName, CancellationToken cancellationToken = default);
    }
}
