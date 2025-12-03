using Catalog_Service.src._01_Domain.Core.Enums;

namespace Catalog_Service.src._03_Endpoints.DTOs.Responses.Public
{
    public class ProductReviewResponse
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Comment { get; set; }
        public int Rating { get; set; }
        public ReviewStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsVerifiedPurchase { get; set; }
        public int HelpfulVotes { get; set; }
        public string UserName { get; set; }
    }
}
