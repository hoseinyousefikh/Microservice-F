using Catalog_Service.src._01_Domain.Core.Enums;

namespace Catalog_Service.src._01_Domain.Core.Entities
{
    public class ProductReview : Entity
    {
        public int ProductId { get; private set; }
        public string UserId { get; private set; }
        public string Title { get; private set; }
        public string Comment { get; private set; }
        public int Rating { get; private set; }
        public ReviewStatus Status { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }
        public bool IsVerifiedPurchase { get; private set; }
        public int HelpfulVotes { get; private set; }

        // Navigation properties
        public Product Product { get; private set; }

        // For EF Core
        protected ProductReview() { }

        public ProductReview(
            int productId,
            string userId,
            string title,
            string comment,
            int rating,
            bool isVerifiedPurchase = false)
        {
            ProductId = productId;
            UserId = userId;
            Title = title;
            Comment = comment;
            Rating = rating;
            IsVerifiedPurchase = isVerifiedPurchase;
            Status = ReviewStatus.Pending;
            CreatedAt = DateTime.UtcNow;
            HelpfulVotes = 0;
        }

        public void UpdateContent(string title, string comment, int rating)
        {
            Title = title;
            Comment = comment;
            Rating = rating;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Approve()
        {
            if (Status == ReviewStatus.Approved) return;

            Status = ReviewStatus.Approved;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Reject()
        {
            if (Status == ReviewStatus.Rejected) return;

            Status = ReviewStatus.Rejected;
            UpdatedAt = DateTime.UtcNow;
        }

        public void MarkAsVerifiedPurchase()
        {
            IsVerifiedPurchase = true;
            UpdatedAt = DateTime.UtcNow;
        }

        public void IncrementHelpfulVotes()
        {
            HelpfulVotes++;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
