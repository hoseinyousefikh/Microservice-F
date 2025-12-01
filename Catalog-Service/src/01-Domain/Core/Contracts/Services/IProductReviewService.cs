using Catalog_Service.src._01_Domain.Core.Entities;
using Catalog_Service.src._01_Domain.Core.Enums;

namespace Catalog_Service.src._01_Domain.Core.Contracts.Services
{
    public interface IProductReviewService
    {
        // متدهای اصلی CRUD
        Task<ProductReview> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<IEnumerable<ProductReview>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<ProductReview>> GetByProductIdAsync(int productId, CancellationToken cancellationToken = default);
        Task<IEnumerable<ProductReview>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);
        Task<ProductReview> CreateAsync(int productId, string userId, string title, string comment, int rating, bool isVerifiedPurchase = false, CancellationToken cancellationToken = default);
        Task UpdateAsync(int id, string title, string comment, int rating, CancellationToken cancellationToken = default);
        Task DeleteAsync(int id, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);

        // متدهای جستجو و فیلتر
        Task<(IEnumerable<ProductReview> Reviews, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize, int? productId = null, string userId = null, ReviewStatus? status = null, int? minRating = null, int? maxRating = null, bool onlyVerified = false, string sortBy = null, bool sortAscending = true, CancellationToken cancellationToken = default);

        // متدهای برای مدیریت وضعیت بازبینی
        Task<IEnumerable<ProductReview>> GetPendingReviewsAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<ProductReview>> GetApprovedReviewsAsync(int productId, CancellationToken cancellationToken = default);
        Task<IEnumerable<ProductReview>> GetRejectedReviewsAsync(CancellationToken cancellationToken = default);
        Task ApproveAsync(int reviewId, CancellationToken cancellationToken = default);
        Task RejectAsync(int reviewId, CancellationToken cancellationToken = default);
        Task MarkAsVerifiedAsync(int reviewId, CancellationToken cancellationToken = default);

        // متدهای برای مدیریت امتیازها
        Task IncrementHelpfulVotesAsync(int reviewId, CancellationToken cancellationToken = default);
        Task DecrementHelpfulVotesAsync(int reviewId, CancellationToken cancellationToken = default);
        Task<IEnumerable<ProductReview>> GetMostHelpfulReviewsAsync(int productId, int count, CancellationToken cancellationToken = default);

        // متدهای ویژه
        Task<bool> UserHasReviewedProductAsync(string userId, int productId, CancellationToken cancellationToken = default);
        Task<ProductReview> GetUserReviewForProductAsync(string userId, int productId, CancellationToken cancellationToken = default);
        Task<int> CountAsync(CancellationToken cancellationToken = default);
        Task<int> CountByProductIdAsync(int productId, CancellationToken cancellationToken = default);
        Task<int> CountByUserIdAsync(string userId, CancellationToken cancellationToken = default);
        Task<int> CountByStatusAsync(ReviewStatus status, CancellationToken cancellationToken = default);

        // متدهای برای آمار و گزارش‌گیری
        Task<double> GetAverageRatingAsync(int productId, CancellationToken cancellationToken = default);
        Task<int> GetTotalReviewsCountAsync(int productId, CancellationToken cancellationToken = default);
        Task<int> GetVerifiedReviewsCountAsync(int productId, CancellationToken cancellationToken = default);
        Task<IDictionary<int, int>> GetRatingDistributionAsync(int productId, CancellationToken cancellationToken = default);
        Task<IEnumerable<ProductReview>> GetRecentReviewsAsync(int productId, int count, CancellationToken cancellationToken = default);

        // متدهای برای گزارش‌گیری پیشرفته
        Task<IEnumerable<ProductReview>> GetTopRatedReviewsAsync(int productId, int count, CancellationToken cancellationToken = default);
        Task<IEnumerable<ProductReview>> GetLowestRatedReviewsAsync(int productId, int count, CancellationToken cancellationToken = default);
        Task<IEnumerable<ProductReview>> GetMostHelpfulReviewsByUserAsync(string userId, int count, CancellationToken cancellationToken = default);
        Task<IDictionary<string, double>> GetAverageRatingByCategoryAsync(int categoryId, CancellationToken cancellationToken = default);
        Task<IDictionary<string, double>> GetAverageRatingByBrandAsync(int brandId, CancellationToken cancellationToken = default);
    }
}
