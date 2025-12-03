using System.ComponentModel.DataAnnotations;

namespace Catalog_Service.src._03_Endpoints.DTOs.Requests.Vendor
{
    public class CreateProductVariantRequest
    {
        [Required(ErrorMessage = "Product ID is required")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Variant name is required")]
        [StringLength(100, ErrorMessage = "Variant name cannot exceed 100 characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "SKU is required")]
        [StringLength(50, ErrorMessage = "SKU cannot exceed 50 characters")]
        public string Sku { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Weight is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Weight must be greater than 0")]
        public decimal Weight { get; set; }

        [Required(ErrorMessage = "Dimensions are required")]
        public DimensionsRequest Dimensions { get; set; }

        public string? ImageUrl { get; set; }
        public decimal? OriginalPrice { get; set; }
    }
}
