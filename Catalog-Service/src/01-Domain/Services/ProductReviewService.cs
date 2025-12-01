using Catalog_Service.src._01_Domain.Core.Contracts.Repositories;
using Catalog_Service.src._01_Domain.Core.Contracts.Services;
using Catalog_Service.src._01_Domain.Core.Entities;
using Catalog_Service.src._01_Domain.Core.Enums;
using Catalog_Service.src._02_Infrastructure.Data.Repositories;
using Catalog_Service.src.CrossCutting.Exceptions;
using System.Threading;

namespace Catalog_Service.src._01_Domain.Services
{
    public class ProductReviewService : IProductReviewService
    {
        private readonly IProductReviewRepository _productReviewRepository;
        private readonly IProductRepository _productRepository;
        private readonly ILogger<ProductReviewService> _logger;

        public ProductReviewService(
            IProductReviewRepository productReviewRepository,
            IProductRepository productRepository,
            ILogger<ProductReviewService> logger)
        {
            _productReviewRepository = productReviewRepository;
            _productRepository = productRepository;
            _logger = logger;
        }

        public async Task<ProductReview> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var review = await _productReviewRepository.GetByIdAsync(id, cancellationToken);
            if (review == null)
            {
                _logger.LogWarning("Product review with ID {ReviewId} not found", id);
                throw new NotFoundException($"Product review with ID {id} not found");
            }
            return review;
        }

        public async Task<IEnumerable<ProductReview>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _productReviewRepository.GetAllAsync(cancellationToken);
        }

        public async Task<IEnumerable<ProductReview>> GetByProductIdAsync(int productId, CancellationToken cancellationToken = default)
        {
            // Check if product exists
            var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
            if (product == null)
                throw new NotFoundException($"Product with ID {productId} not found");

            return await _productReviewRepository.GetByProductIdAsync(productId, cancellationToken);
        }

        public async Task<IEnumerable<ProductReview>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("User ID is required", nameof(userId));

            return await _productReviewRepository.GetByUserIdAsync(userId, cancellationToken);
        }

        public async Task<ProductReview> CreateAsync(int productId, string userId, string title, string comment, int rating, bool isVerifiedPurchase = false, CancellationToken cancellationToken = default)
        {
            // Validate inputs
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("User ID is required", nameof(userId));

            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title is required", nameof(title));

            if (string.IsNullOrWhiteSpace(comment))
                throw new ArgumentException("Comment is required", nameof(comment));

            if (rating < 1 || rating > 5)
                throw new ArgumentException("Rating must be between 1 and 5", nameof(rating));

            // Check if product exists
            var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
            if (product == null)
                throw new NotFoundException($"Product with ID {productId} not found");

            // Check if user already reviewed this product
            if (await _productReviewRepository.UserHasReviewedProductAsync(userId, productId, cancellationToken))
                throw new BusinessRuleException("User has already reviewed this product");

            // Create review
            var review = new ProductReview(productId, userId, title, comment, rating, isVerifiedPurchase);

            // Add to repository
            review = await _productReviewRepository.AddReviewAsync(review, cancellationToken);
            await _productReviewRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Created new product review with ID {ReviewId} for product with ID {ProductId} by user {UserId}", review.Id, productId, userId);
            return review;
        }

        public async Task UpdateAsync(int id, string title, string comment, int rating, CancellationToken cancellationToken = default)
        {
            var review = await GetByIdAsync(id, cancellationToken);

            // Validate inputs
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title is required", nameof(title));

            if (string.IsNullOrWhiteSpace(comment))
                throw new ArgumentException("Comment is required", nameof(comment));

            if (rating < 1 || rating > 5)
                throw new ArgumentException("Rating must be between 1 and 5", nameof(rating));

            review.UpdateContent(title, comment, rating);
            _productReviewRepository.Update(review);
            await _productReviewRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Updated product review with ID {ReviewId}", id);
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var review = await GetByIdAsync(id, cancellationToken);

            _productReviewRepository.Remove(review);
            await _productReviewRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Deleted product review with ID {ReviewId}", id);
        }

        public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _productReviewRepository.ExistsAsync(id, cancellationToken);
        }

        public async Task<(IEnumerable<ProductReview> Reviews, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize, int? productId = null, string userId = null, ReviewStatus? status = null, int? minRating = null, int? maxRating = null, bool onlyVerified = false, string sortBy = null, bool sortAscending = true, CancellationToken cancellationToken = default)
        {
            // Check if product exists (if specified)
            if (productId.HasValue && !await _productRepository.ExistsAsync(productId.Value, cancellationToken))
                throw new NotFoundException($"Product with ID {productId} not found");

            return await _productReviewRepository.GetPagedAsync(pageNumber, pageSize, productId, userId, status, minRating, maxRating, onlyVerified, sortBy, sortAscending, cancellationToken);
        }

        public async Task<IEnumerable<ProductReview>> GetPendingReviewsAsync(CancellationToken cancellationToken = default)
        {
            return await _productReviewRepository.GetPendingReviewsAsync(cancellationToken);
        }

        public async Task<IEnumerable<ProductReview>> GetApprovedReviewsAsync(int productId, CancellationToken cancellationToken = default)
        {
            // Check if product exists
            var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
            if (product == null)
                throw new NotFoundException($"Product with ID {productId} not found");

            return await _productReviewRepository.GetApprovedReviewsAsync(productId, cancellationToken);
        }

        public async Task<IEnumerable<ProductReview>> GetRejectedReviewsAsync(CancellationToken cancellationToken = default)
        {
            return await _productReviewRepository.GetRejectedReviewsAsync(cancellationToken);
        }

        public async Task ApproveAsync(int reviewId, CancellationToken cancellationToken = default)
        {
            await _productReviewRepository.ApproveAsync(reviewId, cancellationToken);
            _logger.LogInformation("Approved product review with ID {ReviewId}", reviewId);
        }

        public async Task RejectAsync(int reviewId, CancellationToken cancellationToken = default)
        {
            await _productReviewRepository.RejectAsync(reviewId, cancellationToken);
            _logger.LogInformation("Rejected product review with ID {ReviewId}", reviewId);
        }

        public async Task MarkAsVerifiedAsync(int reviewId, CancellationToken cancellationToken = default)
        {
            await _productReviewRepository.MarkAsVerifiedAsync(reviewId, cancellationToken);
            _logger.LogInformation("Marked product review with ID {ReviewId} as verified", reviewId);
        }

        public async Task IncrementHelpfulVotesAsync(int reviewId, CancellationToken cancellationToken = default)
        {
            await _productReviewRepository.IncrementHelpfulVotesAsync(reviewId, cancellationToken);
            _logger.LogInformation("Incremented helpful votes for product review with ID {ReviewId}", reviewId);
        }

        public async Task DecrementHelpfulVotesAsync(int reviewId, CancellationToken cancellationToken = default)
        {
            await _productReviewRepository.DecrementHelpfulVotesAsync(reviewId, cancellationToken);
            _logger.LogInformation("Decremented helpful votes for product review with ID {ReviewId}", reviewId);
        }

        public async Task<IEnumerable<ProductReview>> GetMostHelpfulReviewsAsync(int productId, int count, CancellationToken cancellationToken = default)
        {
            // Check if product exists
            var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
            if (product == null)
                throw new NotFoundException($"Product with ID {productId} not found");

            return await _productReviewRepository.GetMostHelpfulReviewsAsync(productId, count, cancellationToken);
        }

        public async Task<bool> UserHasReviewedProductAsync(string userId, int productId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("User ID is required", nameof(userId));

            // Check if product exists
            var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
            if (product == null)
                throw new NotFoundException($"Product with ID {productId} not found");

            return await _productReviewRepository.UserHasReviewedProductAsync(userId, productId, cancellationToken);
        }

        public async Task<ProductReview> GetUserReviewForProductAsync(string userId, int productId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("User ID is required", nameof(userId));

            // Check if product exists
            var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
            if (product == null)
                throw new NotFoundException($"Product with ID {productId} not found");

            return await _productReviewRepository.GetUserReviewForProductAsync(userId, productId, cancellationToken);
        }

        public async Task<int> CountAsync(CancellationToken cancellationToken = default)
        {
            return await _productReviewRepository.CountAsync(cancellationToken);
        }

        public async Task<int> CountByProductIdAsync(int productId, CancellationToken cancellationToken = default)
        {
            // Check if product exists
            var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
            if (product == null)
                throw new NotFoundException($"Product with ID {productId} not found");

            return await _productReviewRepository.CountByProductIdAsync(productId, cancellationToken);
        }

        public async Task<int> CountByUserIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("User");
            return await _productReviewRepository.CountByUserIdAsync(userId, cancellationToken);
        }
        public async Task<int> CountByStatusAsync(ReviewStatus status, CancellationToken cancellationToken = default)
        {
            return await _productReviewRepository.CountByStatusAsync(status, cancellationToken);
        }

        public async Task<double> GetAverageRatingAsync(int productId, CancellationToken cancellationToken = default)
        {
            // Check if product exists
            var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
            if (product == null)
                throw new NotFoundException($"Product with ID {productId} not found");

            return await _productReviewRepository.GetAverageRatingAsync(productId, cancellationToken);
        }

        public async Task<int> GetTotalReviewsCountAsync(int productId, CancellationToken cancellationToken = default)
        {
            // Check if product exists
            var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
            if (product == null)
                throw new NotFoundException($"Product with ID {productId} not found");

            return await _productReviewRepository.GetTotalReviewsCountAsync(productId, cancellationToken);
        }

        public async Task<int> GetVerifiedReviewsCountAsync(int productId, CancellationToken cancellationToken = default)
        {
            // Check if product exists
            var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
            if (product == null)
                throw new NotFoundException($"Product with ID {productId} not found");

            return await _productReviewRepository.GetVerifiedReviewsCountAsync(productId, cancellationToken);
        }

        public async Task<IDictionary<int, int>> GetRatingDistributionAsync(int productId, CancellationToken cancellationToken = default)
        {
            // Check if product exists
            var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
            if (product == null)
                throw new NotFoundException($"Product with ID {productId} not found");

            return await _productReviewRepository.GetRatingDistributionAsync(productId, cancellationToken);
        }

        public async Task<IEnumerable<ProductReview>> GetRecentReviewsAsync(int productId, int count, CancellationToken cancellationToken = default)
        {
            // Check if product exists
            var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
            if (product == null)
                throw new NotFoundException($"Product with ID {productId} not found");

            return await _productReviewRepository.GetRecentReviewsAsync(productId, count, cancellationToken);
        }

        public async Task<IEnumerable<ProductReview>> GetTopRatedReviewsAsync(int productId, int count, CancellationToken cancellationToken = default)
        {
            // Check if product exists
            var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
            if (product == null)
                throw new NotFoundException($"Product with ID {productId} not found");

            return await _productReviewRepository.GetTopRatedReviewsAsync(productId, count, cancellationToken);
        }

        public async Task<IEnumerable<ProductReview>> GetLowestRatedReviewsAsync(int productId, int count, CancellationToken cancellationToken = default)
        {
            // Check if product exists
            var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
            if (product == null)
                throw new NotFoundException($"Product with ID {productId} not found");

            return await _productReviewRepository.GetLowestRatedReviewsAsync(productId, count, cancellationToken);
        }

        public async Task<IEnumerable<ProductReview>> GetMostHelpfulReviewsByUserAsync(string userId, int count, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("User ID is required", nameof(userId));

            return await _productReviewRepository.GetMostHelpfulReviewsByUserAsync(userId, count, cancellationToken);
        }

        public async Task<IDictionary<string, double>> GetAverageRatingByCategoryAsync(int categoryId, CancellationToken cancellationToken = default)
        {
            // In a real implementation, you would need to check if the category exists
            // For now, we'll assume the category exists
            return await _productReviewRepository.GetAverageRatingByCategoryAsync(categoryId, cancellationToken);
        }

        public async Task<IDictionary<string, double>> GetAverageRatingByBrandAsync(int brandId, CancellationToken cancellationToken = default)
        {
            // In a real implementation, you would need to check if the brand exists
            // For now, we'll assume the brand exists
            return await _productReviewRepository.GetAverageRatingByBrandAsync(brandId, cancellationToken);
        }
    }
}

