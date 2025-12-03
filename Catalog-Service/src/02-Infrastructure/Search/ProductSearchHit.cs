namespace Catalog_Service.src._02_Infrastructure.Search
{
    public class ProductSearchHit
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string BrandName { get; set; }
        public string CategoryName { get; set; }
        public string ImageUrl { get; set; }
        public List<string> Highlights { get; set; } = new List<string>();
    }
}
