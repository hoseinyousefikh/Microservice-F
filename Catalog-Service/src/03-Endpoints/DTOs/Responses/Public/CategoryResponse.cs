namespace Catalog_Service.src._03_Endpoints.DTOs.Responses.Public
{
    public class CategoryResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Slug { get; set; }
        public int DisplayOrder { get; set; }
        public int? ParentCategoryId { get; set; }
        public string ImageUrl { get; set; }
        public int ProductCount { get; set; }
        public List<CategoryResponse> SubCategories { get; set; } = new();
    }
}
