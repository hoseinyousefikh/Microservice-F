namespace Catalog_Service.src._01_Domain.Core.Entities
{
    public class ProductReviewReply : Entity
    {
        public int ProductReviewId { get; private set; }
        public string UserId { get; private set; }
        public string Comment { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }

        // Navigation properties
        public ProductReview ProductReview { get; private set; }

        // For EF Core
        protected ProductReviewReply() { }

        public ProductReviewReply(int productReviewId, string userId, string comment)
        {
            ProductReviewId = productReviewId;
            UserId = userId;
            Comment = comment;
            CreatedAt = DateTime.UtcNow;
        }

        public void UpdateComment(string comment)
        {
            Comment = comment;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
