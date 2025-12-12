using AutoMapper;
using Catalog_Service.src._01_Domain.Core.Contracts.Services;
using Catalog_Service.src._03_Endpoints.DTOs.Responses.Admin;
using Catalog_Service.src.CrossCutting.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Catalog_Service.src._03_Endpoints.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/[controller]")]
    [Authorize(Policy = AuthorizationPolicies.AdminPolicy)]
    public class AdminProductReviewController : ControllerBase
    {
        private readonly IProductReviewService _productReviewService;
        private readonly IMapper _mapper;
        private readonly ILogger<AdminProductReviewController> _logger;

        public AdminProductReviewController(
            IProductReviewService productReviewService,
            IMapper mapper,
            ILogger<AdminProductReviewController> logger)
        {
            _productReviewService = productReviewService;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet("product/{productId}")]
        public async Task<IActionResult> GetProductReviews(int productId)
        {
            var reviews = await _productReviewService.GetByProductIdAsync(productId);
            var response = _mapper.Map<IEnumerable<AdminProductReviewResponse>>(reviews);
            return Ok(response);
        }

        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingReviews()
        {
            var reviews = await _productReviewService.GetPendingReviewsAsync();
            var response = _mapper.Map<IEnumerable<AdminProductReviewResponse>>(reviews);
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductReview(int id)
        {
            var review = await _productReviewService.GetByIdAsync(id);
            if (review == null)
            {
                return NotFound();
            }

            var response = _mapper.Map<AdminProductReviewResponse>(review);
            return Ok(response);
        }

        [HttpPost("{id}/approve")]
        public async Task<IActionResult> ApproveReview(int id)
        {
            await _productReviewService.ApproveAsync(id);
            return NoContent();
        }

        [HttpPost("{id}/reject")]
        public async Task<IActionResult> RejectReview(int id)
        {
            await _productReviewService.RejectAsync(id);
            return NoContent();
        }

        [HttpPost("{id}/verify")]
        public async Task<IActionResult> MarkAsVerified(int id)
        {
            await _productReviewService.MarkAsVerifiedAsync(id);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReview(int id)
        {
            await _productReviewService.DeleteAsync(id);
            return NoContent();
        }
    }
}
