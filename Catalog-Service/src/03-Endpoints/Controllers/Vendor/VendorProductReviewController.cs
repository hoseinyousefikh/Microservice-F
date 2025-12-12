using AutoMapper;
using Catalog_Service.src._01_Domain.Core.Contracts.Services;
using Catalog_Service.src._01_Domain.Core.Enums;
using Catalog_Service.src._03_Endpoints.DTOs.Responses;
using Catalog_Service.src.CrossCutting.Exceptions;
using Catalog_Service.src.CrossCutting.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Catalog_Service.src._03_Endpoints.Controllers.Vendor
{
    [ApiController]
    [Route("api/vendor/products/{productId}/reviews")]
    [Authorize(Roles = RoleConstants.Vendor)]
    public class VendorProductReviewController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IProductReviewService _productReviewService;
        private readonly IMapper _mapper;

        public VendorProductReviewController(
            IProductService productService,
            IProductReviewService productReviewService,
            IMapper mapper)
        {
            _productService = productService;
            _productReviewService = productReviewService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResponse<VendorProductReviewResponse>>> GetReviews(
            int productId,
            [FromQuery] ReviewSearchRequest request,
            CancellationToken cancellationToken)
        {
            var product = await _productService.GetByIdAsync(productId, cancellationToken);
            if (product == null)
                throw new NotFoundException("Product", productId);

            var (reviews, totalCount) = await _productReviewService.GetPagedAsync(
                request.PageNumber,
                request.PageSize,
                productId,
                null, // userId
                request.Status,
                request.MinRating,
                request.MaxRating,
                request.OnlyVerified,
                request.SortBy,
                request.SortAscending,
                cancellationToken);

            var reviewResponses = _mapper.Map<IEnumerable<VendorProductReviewResponse>>(reviews);

            return Ok(new PagedResponse<VendorProductReviewResponse>
            {
                Items = reviewResponses,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<VendorProductReviewResponse>> GetReview(
            int productId,
            int id,
            CancellationToken cancellationToken)
        {
            var product = await _productService.GetByIdAsync(productId, cancellationToken);
            if (product == null)
                throw new NotFoundException("Product", productId);

            var review = await _productReviewService.GetByIdAsync(id, cancellationToken);
            if (review == null || review.ProductId != productId)
                throw new NotFoundException("ProductReview", id);

            var reviewResponse = _mapper.Map<VendorProductReviewResponse>(review);
            return Ok(reviewResponse);
        }

        [HttpPost("{id}/approve")]
        public async Task<IActionResult> ApproveReview(int productId, int id, CancellationToken cancellationToken)
        {
            var product = await _productService.GetByIdAsync(productId, cancellationToken);
            if (product == null)
                throw new NotFoundException("Product", productId);

            var review = await _productReviewService.GetByIdAsync(id, cancellationToken);
            if (review == null || review.ProductId != productId)
                throw new NotFoundException("ProductReview", id);

            await _productReviewService.ApproveAsync(id, cancellationToken);
            return NoContent();
        }

        [HttpPost("{id}/reject")]
        public async Task<IActionResult> RejectReview(int productId, int id, CancellationToken cancellationToken)
        {
            var product = await _productService.GetByIdAsync(productId, cancellationToken);
            if (product == null)
                throw new NotFoundException("Product", productId);

            var review = await _productReviewService.GetByIdAsync(id, cancellationToken);
            if (review == null || review.ProductId != productId)
                throw new NotFoundException("ProductReview", id);

            await _productReviewService.RejectAsync(id, cancellationToken);
            return NoContent();
        }

        [HttpPost("{id}/verify")]
        public async Task<IActionResult> VerifyReview(int productId, int id, CancellationToken cancellationToken)
        {
            var product = await _productService.GetByIdAsync(productId, cancellationToken);
            if (product == null)
                throw new NotFoundException("Product", productId);

            var review = await _productReviewService.GetByIdAsync(id, cancellationToken);
            if (review == null || review.ProductId != productId)
                throw new NotFoundException("ProductReview", id);

            await _productReviewService.MarkAsVerifiedAsync(id, cancellationToken);
            return NoContent();
        }

        [HttpGet("stats")]
        public async Task<ActionResult<ProductReviewStatsResponse>> GetReviewStats(int productId, CancellationToken cancellationToken)
        {
            var product = await _productService.GetByIdAsync(productId, cancellationToken);
            if (product == null)
                throw new NotFoundException("Product", productId);

            var averageRating = await _productReviewService.GetAverageRatingAsync(productId, cancellationToken);
            var totalReviews = await _productReviewService.GetTotalReviewsCountAsync(productId, cancellationToken);
            var verifiedReviews = await _productReviewService.GetVerifiedReviewsCountAsync(productId, cancellationToken);
            var ratingDistribution = await _productReviewService.GetRatingDistributionAsync(productId, cancellationToken);

            return Ok(new ProductReviewStatsResponse
            {
                AverageRating = averageRating,
                TotalReviews = totalReviews,
                VerifiedReviews = verifiedReviews,
                RatingDistribution = ratingDistribution
            });
        }
    }

    public class ReviewSearchRequest
    {
        public ReviewStatus? Status { get; set; }
        public int? MinRating { get; set; }
        public int? MaxRating { get; set; }
        public bool OnlyVerified { get; set; }
        public string SortBy { get; set; } = "date";
        public bool SortAscending { get; set; } = false;
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    public class VendorProductReviewResponse
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Comment { get; set; }
        public int Rating { get; set; }
        public ReviewStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsVerifiedPurchase { get; set; }
        public int HelpfulVotes { get; set; }
        public string UserId { get; set; }
    }

    public class ProductReviewStatsResponse
    {
        public double AverageRating { get; set; }
        public int TotalReviews { get; set; }
        public int VerifiedReviews { get; set; }
        public IDictionary<int, int> RatingDistribution { get; set; }
    }
}
