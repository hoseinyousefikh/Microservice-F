using AutoMapper;
using Catalog_Service.src._01_Domain.Core.Contracts.Services;
using Catalog_Service.src._01_Domain.Core.Enums;
using Catalog_Service.src._03_Endpoints.DTOs.Requests.Public;
using Catalog_Service.src._03_Endpoints.DTOs.Responses;
using Catalog_Service.src._03_Endpoints.DTOs.Responses.Public;
using Catalog_Service.src.CrossCutting.Exceptions;
using Catalog_Service.src.CrossCutting.Security;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Catalog_Service.src._03_Endpoints.Controllers.Public
{
    [ApiController]
    [Route("api/public/reviews")]
    public class ProductReviewController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IProductReviewService _productReviewService;
        private readonly IMapper _mapper;
        private readonly IValidator<CreateProductReviewRequest> _reviewValidator;

        public ProductReviewController(
            IProductService productService,
            IProductReviewService productReviewService,
            IMapper mapper,
            IValidator<CreateProductReviewRequest> reviewValidator)
        {
            _productService = productService;
            _productReviewService = productReviewService;
            _mapper = mapper;
            _reviewValidator = reviewValidator;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductReviewResponse>> GetReview(int id, CancellationToken cancellationToken)
        {
            var review = await _productReviewService.GetByIdAsync(id, cancellationToken);
            if (review == null || review.Status != ReviewStatus.Approved)
                throw new NotFoundException("ProductReview", id);

            var reviewResponse = _mapper.Map<ProductReviewResponse>(review);
            return Ok(reviewResponse);
        }

        [HttpPost]
        [Authorize(Roles = RoleConstants.Customer)]
        public async Task<ActionResult<ProductReviewResponse>> CreateReview(
    [FromBody] CreateProductReviewRequest request,
    CancellationToken cancellationToken)
        {
            await _reviewValidator.ValidateAndThrowAsync(request, cancellationToken);

            var product = await _productService.GetByIdAsync(request.ProductId, cancellationToken);
            if (product == null || product.Status != ProductStatus.Published)
                throw new NotFoundException("Product", request.ProductId);

            var userId = User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(userId))
                throw new global::Catalog_Service.src.CrossCutting.Exceptions.UnauthorizedAccessException("User", "CreateReview");

            var existingReview = await _productReviewService.GetUserReviewForProductAsync(userId, request.ProductId, cancellationToken);
            if (existingReview != null)
                throw new BusinessRuleException("You have already reviewed this product");

            var review = await _productReviewService.CreateAsync(
                request.ProductId,
                userId,
                request.Title,
                request.Comment,
                request.Rating,
                request.IsVerifiedPurchase,
                cancellationToken);

            var reviewResponse = _mapper.Map<ProductReviewResponse>(review);
            return CreatedAtAction(nameof(GetReview), new { id = review.Id }, reviewResponse);
        }

        [HttpPost("{id}/helpful")]
        [Authorize(Roles = RoleConstants.Customer)]
        public async Task<IActionResult> MarkReviewHelpful(int id, CancellationToken cancellationToken)
        {
            var review = await _productReviewService.GetByIdAsync(id, cancellationToken);
            if (review == null || review.Status != ReviewStatus.Approved)
                throw new NotFoundException("ProductReview", id);

            await _productReviewService.IncrementHelpfulVotesAsync(id, cancellationToken);
            return NoContent();
        }

        [HttpGet("product/{productId}")]
        public async Task<ActionResult<PagedResponse<ProductReviewResponse>>> GetProductReviews(
            int productId,
            [FromQuery] ReviewSearchRequest request,
            CancellationToken cancellationToken)
        {
            var product = await _productService.GetByIdAsync(productId, cancellationToken);
            if (product == null || product.Status != ProductStatus.Published)
                throw new NotFoundException("Product", productId);

            var (reviews, totalCount) = await _productReviewService.GetPagedAsync(
                request.PageNumber,
                request.PageSize,
                productId,
                null, // userId
                ReviewStatus.Approved, // Only show approved reviews
                null, // minRating
                null, // maxRating
                false, // onlyVerified
                request.SortBy,
                request.SortAscending,
                cancellationToken);

            var reviewResponses = _mapper.Map<IEnumerable<ProductReviewResponse>>(reviews);

            return Ok(new PagedResponse<ProductReviewResponse>
            {
                Items = reviewResponses,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            });
        }

        [HttpGet("product/{productId}/stats")]
        public async Task<ActionResult<ProductReviewStatsResponse>> GetProductReviewStats(
            int productId,
            CancellationToken cancellationToken)
        {
            var product = await _productService.GetByIdAsync(productId, cancellationToken);
            if (product == null || product.Status != ProductStatus.Published)
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
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string SortBy { get; set; } = "date";
        public bool SortAscending { get; set; } = false;
    }

    public class ProductReviewStatsResponse
    {
        public double AverageRating { get; set; }
        public int TotalReviews { get; set; }
        public int VerifiedReviews { get; set; }
        public IDictionary<int, int> RatingDistribution { get; set; }
    }
}
