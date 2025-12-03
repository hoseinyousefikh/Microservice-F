using System.ComponentModel.DataAnnotations;

namespace Catalog_Service.src._03_Endpoints.DTOs.Requests.Admin
{
    public class UpdateCategoryRequest
    {
        [Required(ErrorMessage = "Category ID is required")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Category name is required")]
        [StringLength(100, ErrorMessage = "Category name cannot exceed 100 characters")]
        public string Name { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Display order is required")]
        [Range(0, int.MaxValue, ErrorMessage = "Display order must be a positive number")]
        public int DisplayOrder { get; set; }

        public int? ParentCategoryId { get; set; }

        [Url(ErrorMessage = "Invalid image URL format")]
        public string ImageUrl { get; set; }

        [StringLength(60, ErrorMessage = "Meta title cannot exceed 60 characters")]
        public string MetaTitle { get; set; }

        [StringLength(160, ErrorMessage = "Meta description cannot exceed 160 characters")]
        public string MetaDescription { get; set; }

        public bool IsActive { get; set; }
    }
}
