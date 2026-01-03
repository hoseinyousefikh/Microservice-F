using System.ComponentModel.DataAnnotations;

namespace Catalog_Service.src._03_Endpoints.DTOs.Requests.Admin
{
    public class CreateBrandRequest
    {
        [Required(ErrorMessage = "Brand name is required")]
        [StringLength(100, ErrorMessage = "Brand name cannot exceed 100 characters")]
        public string Name { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; }
        public string? WebsiteUrl { get; set; }
        [Url(ErrorMessage = "Invalid logo URL format")]
        public string LogoUrl { get; set; }

        [StringLength(60, ErrorMessage = "Meta title cannot exceed 60 characters")]
        public string MetaTitle { get; set; }

        [StringLength(160, ErrorMessage = "Meta description cannot exceed 160 characters")]
        public string MetaDescription { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
