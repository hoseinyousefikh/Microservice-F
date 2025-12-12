using Catalog_Service.src._01_Domain.Core.Entities;
using Catalog_Service.src._01_Domain.Core.Enums;
using Catalog_Service.src._01_Domain.Core.Primitives;

namespace Catalog_Service.src._03_Endpoints.DTOs.Responses.Admin
{
    public class AdminProductResponse
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
        public DimensionsResponse Dimensions { get; set; }
        public WeightResponse Weight { get; set; }
        public double AverageRating { get; set; }
        public int TotalReviews { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? PublishedAt { get; set; }
        public List<ProductVariantResponse> Variants { get; set; } = new();
        public List<ProductImageResponse> Images { get; set; } = new();
        public List<ProductAttributeResponse> Attributes { get; set; } = new();
    }

    public class ProductVariantResponse
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
        public DimensionsResponse Dimensions { get; set; }
        public WeightResponse Weight { get; set; }
    }

    public class ProductImageResponse
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

    public class ProductAttributeResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public bool IsVariantSpecific { get; set; }
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
    public class AdminProductReviewResponse
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string Title { get; set; }
        public string Comment { get; set; }
        public int Rating { get; set; }
        public ReviewStatus Status { get; set; }
        public bool IsVerifiedPurchase { get; set; }
        public bool IsHelpful { get; set; }
        public int HelpfulCount { get; set; }
        public int UnhelpfulCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public string? AdminNotes { get; set; }
        public bool HasImages { get; set; }
        public int ReplyCount { get; set; }
    }

    public class AdminProductReviewSummaryResponse
    {
        public int TotalReviews { get; set; }
        public int PendingReviews { get; set; }
        public int ApprovedReviews { get; set; }
        int RejectedReviews { get; set; }
        public double AverageRating { get; set; }
        public Dictionary<int, int> RatingDistribution { get; set; } // Key: Rating (1-5), Value: Count
    }
}
