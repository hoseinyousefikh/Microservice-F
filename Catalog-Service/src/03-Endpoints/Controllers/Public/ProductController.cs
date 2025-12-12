using AutoMapper;
using Catalog_Service.src._01_Domain.Core.Contracts.Services;
using Catalog_Service.src._01_Domain.Core.Enums;
using Catalog_Service.src._03_Endpoints.DTOs.Requests.Public;
using Catalog_Service.src._03_Endpoints.DTOs.Responses;
using Catalog_Service.src._03_Endpoints.DTOs.Responses.Public;
using Catalog_Service.src.CrossCutting.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Catalog_Service.src._03_Endpoints.Controllers.Public
{
    [ApiController]
    [Route("api/public/products")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IProductVariantService _productVariantService;
        private readonly IProductReviewService _productReviewService;
        private readonly IMapper _mapper;
        private readonly IValidator<ProductSearchRequest> _searchValidator;
        private readonly IValidator<CreateProductReviewRequest> _reviewValidator;

        public ProductController(
            IProductService productService,
            IProductVariantService productVariantService,
            IProductReviewService productReviewService,
            IMapper mapper,
            IValidator<ProductSearchRequest> searchValidator,
            IValidator<CreateProductReviewRequest> reviewValidator)
        {
            _productService = productService;
            _productVariantService = productVariantService;
            _productReviewService = productReviewService;
            _mapper = mapper;
            _searchValidator = searchValidator;
            _reviewValidator = reviewValidator;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResponse<ProductResponse>>> SearchProducts(
            [FromQuery] ProductSearchRequest request,
            CancellationToken cancellationToken = default)
        {
            await _searchValidator.ValidateAndThrowAsync(request, cancellationToken);

            var result = await _productService.GetPagedAsync(
                request.PageNumber,
                request.PageSize,
                request.SearchTerm,
                request.CategoryId,
                request.BrandId,
                ProductStatus.Published,
                request.MinPrice,
                request.MaxPrice,
                request.SortBy,
                request.SortAscending,
                cancellationToken);

            var products = result.Products;
            var totalCount = result.TotalCount;

            var productResponses = _mapper.Map<IEnumerable<ProductResponse>>(products);

            return Ok(new PagedResponse<ProductResponse>
            {
                Items = productResponses,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductResponse>> GetProduct(int id, CancellationToken cancellationToken = default)
        {
            var product = await _productService.GetByIdAsync(id, cancellationToken);
            if (product == null || product.Status != ProductStatus.Published)
                throw new NotFoundException("Product", id);

            await _productService.IncrementViewCountAsync(id, cancellationToken);

            var productResponse = _mapper.Map<ProductResponse>(product);
            return Ok(productResponse);
        }

        [HttpGet("{id}/variants")]
        public async Task<ActionResult<IEnumerable<ProductVariantResponse>>> GetProductVariants(
            int id,
            CancellationToken cancellationToken = default)
        {
            var product = await _productService.GetByIdAsync(id, cancellationToken);
            if (product == null || product.Status != ProductStatus.Published)
                throw new NotFoundException("Product", id);

            var variants = await _productVariantService.GetActiveVariantsAsync(id, cancellationToken);
            var variantResponses = _mapper.Map<IEnumerable<ProductVariantResponse>>(variants);
            return Ok(variantResponses);
        }

        [HttpGet("{id}/reviews")]
        public async Task<ActionResult<PagedResponse<ProductReviewResponse>>> GetProductReviews(
            int id,
            [FromQuery] ProductReviewSearchRequest request,
            CancellationToken cancellationToken = default)
        {
            var product = await _productService.GetByIdAsync(id, cancellationToken);
            if (product == null || product.Status != ProductStatus.Published)
                throw new NotFoundException("Product", id);

            var result = await _productReviewService.GetPagedAsync(
                request.PageNumber,
                request.PageSize,
                id,
                null,
                ReviewStatus.Approved,
                null,
                null,
                false,
                request.SortBy,
                request.SortAscending,
                cancellationToken);

            var reviews = result.Reviews;
            var totalCount = result.TotalCount;

            var reviewResponses = _mapper.Map<IEnumerable<ProductReviewResponse>>(reviews);

            return Ok(new PagedResponse<ProductReviewResponse>
            {
                Items = reviewResponses,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            });
        }

        [HttpGet("{id}/stats")]
        public async Task<ActionResult<ProductStatsResponse>> GetProductStats(int id, CancellationToken cancellationToken = default)
        {
            var product = await _productService.GetByIdAsync(id, cancellationToken);
            if (product == null || product.Status != ProductStatus.Published)
                throw new NotFoundException("Product", id);

            var averageRating = await _productReviewService.GetAverageRatingAsync(id, cancellationToken);
            var totalReviews = await _productReviewService.GetTotalReviewsCountAsync(id, cancellationToken);
            var ratingDistribution = await _productReviewService.GetRatingDistributionAsync(id, cancellationToken);
            var minPrice = await _productVariantService.GetMinPriceAsync(id, cancellationToken);
            var maxPrice = await _productVariantService.GetMaxPriceAsync(id, cancellationToken);

            return Ok(new ProductStatsResponse
            {
                AverageRating = averageRating,
                TotalReviews = totalReviews,
                RatingDistribution = ratingDistribution,
                MinPrice = minPrice,
                MaxPrice = maxPrice
            });
        }

        [HttpGet("featured")]
        public async Task<ActionResult<IEnumerable<ProductResponse>>> GetFeaturedProducts(
            [FromQuery] int count = 10,
            CancellationToken cancellationToken = default)
        {
            var products = await _productService.GetFeaturedProductsAsync(count, cancellationToken);
            var productResponses = _mapper.Map<IEnumerable<ProductResponse>>(products);
            return Ok(productResponses);
        }

        [HttpGet("newest")]
        public async Task<ActionResult<IEnumerable<ProductResponse>>> GetNewestProducts(
            [FromQuery] int count = 10,
            CancellationToken cancellationToken = default)
        {
            var products = await _productService.GetNewestProductsAsync(count, cancellationToken);
            var productResponses = _mapper.Map<IEnumerable<ProductResponse>>(products);
            return Ok(productResponses);
        }

        [HttpGet("bestselling")]
        public async Task<ActionResult<IEnumerable<ProductResponse>>> GetBestSellingProducts(
            [FromQuery] int count = 10,
            CancellationToken cancellationToken = default)
        {
            var products = await _productService.GetBestSellingProductsAsync(count, cancellationToken);
            var productResponses = _mapper.Map<IEnumerable<ProductResponse>>(products);
            return Ok(productResponses);
        }

        [HttpGet("by-category/{categoryId}")]
        public async Task<ActionResult<IEnumerable<ProductResponse>>> GetProductsByCategory(
            int categoryId,
            [FromQuery] int count = 20,
            CancellationToken cancellationToken = default)
        {
            var products = await _productService.GetByCategoryAsync(categoryId, cancellationToken);
            var productResponses = _mapper.Map<IEnumerable<ProductResponse>>(products.Take(count));
            return Ok(productResponses);
        }

        [HttpGet("by-brand/{brandId}")]
        public async Task<ActionResult<IEnumerable<ProductResponse>>> GetProductsByBrand(
            int brandId,
            [FromQuery] int count = 20,
            CancellationToken cancellationToken = default)
        {
            var products = await _productService.GetByBrandAsync(brandId, cancellationToken);
            var productResponses = _mapper.Map<IEnumerable<ProductResponse>>(products.Take(count));
            return Ok(productResponses);
        }
    }

    public class ProductReviewSearchRequest
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string SortBy { get; set; } = "date";
        public bool SortAscending { get; set; } = false;
    }

    public class ProductStatsResponse
    {
        public double AverageRating { get; set; }
        public int TotalReviews { get; set; }
        public IDictionary<int, int> RatingDistribution { get; set; }
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
    }
}
