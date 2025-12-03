using Catalog_Service.src._01_Domain.Core.Enums;

namespace Catalog_Service.src._03_Endpoints.DTOs.Responses.Vendor
{
    public class VendorProductResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public decimal? OriginalPrice { get; set; }
        public string Sku { get; set; }
        public string Slug { get; set; }
        public ProductStatus Status { get; set; }
        public int BrandId { get; set; }
        public string BrandName { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int StockQuantity { get; set; }
        public StockStatus StockStatus { get; set; }
        public bool IsFeatured { get; set; }
        public int ViewCount { get; set; }
        public string ImageUrl { get; set; }
        public VendorDimensionsResponse Dimensions { get; set; }
        public VendorWeightResponse Weight { get; set; }
        public double AverageRating { get; set; }
        public int TotalReviews { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? PublishedAt { get; set; }
        public List<VendorProductVariantResponse> Variants { get; set; } = new();
        public List<VendorProductImageResponse> Images { get; set; } = new();
        public List<VendorProductAttributeResponse> Attributes { get; set; } = new();
    }

    public class VendorProductVariantResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Sku { get; set; }
        public decimal Price { get; set; }
        public decimal? OriginalPrice { get; set; }
        public int StockQuantity { get; set; }
        public StockStatus StockStatus { get; set; }
        public bool IsActive { get; set; }
        public string ImageUrl { get; set; }
        public VendorDimensionsResponse Dimensions { get; set; }
        public VendorWeightResponse Weight { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class VendorProductImageResponse
    {
        public int Id { get; set; }
        public string OriginalFileName { get; set; }
        public string FileExtension { get; set; }
        public string PublicUrl { get; set; }
        public long FileSize { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public ImageType ImageType { get; set; }
        public string AltText { get; set; }
        public bool IsPrimary { get; set; }
    }

    public class VendorProductAttributeResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public bool IsVariantSpecific { get; set; }
    }

    public class VendorDimensionsResponse
    {
        public decimal Length { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
    }

    public class VendorWeightResponse
    {
        public decimal Value { get; set; }
        public string Unit { get; set; }
    }
}
