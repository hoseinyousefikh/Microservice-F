using Catalog_Service.src._01_Domain.Core.Enums;
using static Catalog_Service.src._03_Endpoints.DTOs.Responses.Public.ProductVariantResponse;

namespace Catalog_Service.src._03_Endpoints.DTOs.Responses.Public
{
    public class ProductResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public decimal? OriginalPrice { get; set; }
        public string Sku { get; set; }
        public string Slug { get; set; }
        public int BrandId { get; set; }
        public string BrandName { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int StockQuantity { get; set; }
        public StockStatus StockStatus { get; set; }
        public bool IsFeatured { get; set; }
        public string ImageUrl { get; set; }
        public DimensionsResponse Dimensions { get; set; }
        public WeightResponse Weight { get; set; }
        public double AverageRating { get; set; }
        public int TotalReviews { get; set; }
        public List<ProductVariantResponse> Variants { get; set; } = new();
        public List<ProductImageResponse> Images { get; set; } = new();
        public List<ProductAttributeResponse> Attributes { get; set; } = new();
    }

    public class ProductVariantResponse
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Sku { get; set; }
        public decimal Price { get; set; }
        public decimal? OriginalPrice { get; set; }
        public int StockQuantity { get; set; }
        public StockStatus StockStatus { get; set; }
        public bool IsActive { get; set; }
        public string ImageUrl { get; set; }
        public DimensionsResponse Dimensions { get; set; }
        public WeightResponse Weight { get; set; }

        public class ProductImageResponse
        {
            public int Id { get; set; }
            public string PublicUrl { get; set; }
            public string AltText { get; set; }
            public bool IsPrimary { get; set; }
            public int Width { get; set; }
            public int Height { get; set; }
        }

        public class ProductAttributeResponse
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Value { get; set; }
        }

        public class DimensionsResponse
        {
            public decimal Length { get; set; }
            public decimal Width { get; set; }
            public decimal Height { get; set; }
        }

        public class WeightResponse
        {
            public decimal Value { get; set; }
            public string Unit { get; set; }
        }
    }
}
