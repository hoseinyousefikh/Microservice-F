using Catalog_Service.src._01_Domain.Core.Enums;
using Catalog_Service.src._01_Domain.Core.Primitives;

namespace Catalog_Service.src._01_Domain.Core.Entities
{
    public class ImageResource : AggregateRoot
    {
        public string OriginalFileName { get; private set; }
        public string FileExtension { get; private set; }
        public string StoragePath { get; private set; }
        public string PublicUrl { get; private set; }
        public long FileSize { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public ImageType ImageType { get; private set; }
        public string? AltText { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }
        public bool IsPrimary { get; private set; }

        // For EF Core
        protected ImageResource() { }

        public ImageResource(
            string originalFileName,
            string fileExtension,
            string storagePath,
            string publicUrl,
            long fileSize,
            int width,
            int height,
            ImageType imageType,
            string? altText = null,
            bool isPrimary = false)
        {
            OriginalFileName = originalFileName;
            FileExtension = fileExtension;
            StoragePath = storagePath;
            PublicUrl = publicUrl;
            FileSize = fileSize;
            Width = width;
            Height = height;
            ImageType = imageType;
            AltText = altText;
            CreatedAt = DateTime.UtcNow;
            IsPrimary = isPrimary;
        }

        public void UpdateDetails(
            string? altText = null,
            bool isPrimary = false)
        {
            AltText = altText;
            IsPrimary = isPrimary;
            UpdatedAt = DateTime.UtcNow;
        }

        public void SetAsPrimary()
        {
            IsPrimary = true;
            UpdatedAt = DateTime.UtcNow;
        }

        public void RemoveAsPrimary()
        {
            IsPrimary = false;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
