using Nest;

namespace Catalog_Service.src._02_Infrastructure.Search
{
    public class ProductDocument
    {
        [Keyword(Name = "id")]
        public string Id { get; set; }

        [Text(Name = "name")]
        public string Name { get; set; }

        [Text(Name = "description")]
        public string Description { get; set; }

        [Number(NumberType.Double, Name = "price")]
        public decimal Price { get; set; }

        [Number(NumberType.Double, Name = "average_rating")]
        public double AverageRating { get; set; }

        [Keyword(Name = "brand_id")]
        public int BrandId { get; set; }

        [Keyword(Name = "brand_name")]
        public string BrandName { get; set; }

        [Keyword(Name = "category_id")]
        public int CategoryId { get; set; }

        [Keyword(Name = "category_name")]
        public string CategoryName { get; set; }

        [Keyword(Name = "image_url")]
        public string ImageUrl { get; set; }

        [Boolean(Name = "in_stock")]
        public bool InStock { get; set; }

        [Boolean(Name = "is_active")]
        public bool IsActive { get; set; }

      
        [Date(Name = "created_at")]
        public System.DateTime CreatedAt { get; set; }
    }
}
