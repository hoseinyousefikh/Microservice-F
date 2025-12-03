namespace Catalog_Service.src._02_Infrastructure.Search
{
    public class ElasticSearchOptions
    {
        public string DefaultIndex { get; set; } = "products";
        public string Uri { get; set; }
        public string DefaultUsername { get; set; }
        public string DefaultPassword { get; set; }
    }
}
