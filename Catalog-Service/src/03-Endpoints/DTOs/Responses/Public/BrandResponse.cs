namespace Catalog_Service.src._03_Endpoints.DTOs.Responses.Public
{
    public class BrandResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Slug { get; set; }
        public string LogoUrl { get; set; }
        public int ProductCount { get; set; }
    }
}
