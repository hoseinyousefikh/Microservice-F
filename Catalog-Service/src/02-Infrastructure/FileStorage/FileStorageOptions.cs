namespace Catalog_Service.src._02_Infrastructure.FileStorage
{
    public class FileStorageOptions
    {
        public string LocalPath { get; set; }
        public string BaseUrl { get; set; }
        public bool UseS3 { get; set; }
        public bool UseAzureBlob { get; set; }
        public string S3 { get; set; }
        public string Azure { get; set; }
    }
}
