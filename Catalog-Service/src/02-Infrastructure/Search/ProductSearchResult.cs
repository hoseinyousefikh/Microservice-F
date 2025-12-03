namespace Catalog_Service.src._02_Infrastructure.Search
{
    public class ProductSearchResult
    {
        public long Total { get; set; }
        public List<ProductSearchHit> Products { get; set; } = new List<ProductSearchHit>();
    }
}
