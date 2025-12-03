namespace Catalog_Service.src._03_Endpoints.DTOs.Responses.Admin
{
    public class AdminBrandResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Slug { get; set; }
        public string LogoUrl { get; set; }
        public bool IsActive { get; set; }
        public int ProductCount { get; set; }
        public string MetaTitle { get; set; }
        public string MetaDescription { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
