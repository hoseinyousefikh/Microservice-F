namespace Catalog_Service.src.CrossCutting.Extensions
{
    public class ApiKeyValidationResult
    {
        public bool IsValid { get; set; }
        public string ServiceName { get; set; } = string.Empty;
    }
}