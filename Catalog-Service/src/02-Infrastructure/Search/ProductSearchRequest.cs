using System.ComponentModel.DataAnnotations;

namespace Catalog_Service.src._02_Infrastructure.Search
{
    public class ProductSearchRequest
    {
        [Required]
        public string Query { get; set; }

        public int From { get; set; } = 0;
        public int Size { get; set; } = 10;
        public string SortBy { get; set; }
        public bool SortAscending { get; set; }

        public List<int> CategoryIds { get; set; }
        public List<int> BrandIds { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public bool InStockOnly { get; set; }
    }
}
