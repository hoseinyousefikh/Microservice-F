using System.ComponentModel.DataAnnotations;

namespace Catalog_Service.src._03_Endpoints.DTOs.Requests.Admin
{
    public class AdminProductSearchRequest
    {
        [StringLength(100, ErrorMessage = "Search term cannot exceed 100 characters")]
        public string SearchTerm { get; set; }

        public int? CategoryId { get; set; }

        public int? BrandId { get; set; }

        public bool? IsActive { get; set; }

        public decimal? MinPrice { get; set; }

        public decimal? MaxPrice { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Page number must be at least 1")]
        public int PageNumber { get; set; } = 1;

        [Range(1, 100, ErrorMessage = "Page size must be between 1 and 100")]
        public int PageSize { get; set; } = 20;

        public string SortBy { get; set; } = "name";

        public bool SortAscending { get; set; } = true;
    }
}
