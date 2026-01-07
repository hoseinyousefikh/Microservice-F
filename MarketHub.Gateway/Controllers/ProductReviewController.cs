using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MarketHub.Gateway.Controllers
{
    [ApiController]
    [Route("api/public/reviews")]
    public class ProductReviewController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<ProductReviewController> _logger;
        private const string CatalogServiceBaseUrl = "https://localhost:7070";

        public ProductReviewController(IHttpClientFactory httpClientFactory, ILogger<ProductReviewController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        private void AddAuthorizationHeader(HttpClient client)
        {
            var authHeader = Request.Headers.Authorization.FirstOrDefault();
            if (authHeader != null && authHeader.StartsWith("Bearer "))
            {
                var token = authHeader.Substring("Bearer ".Length).Trim();
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
        }

        private async Task<IActionResult> ForwardRequest(Func<Task<HttpResponseMessage>> requestAction, string operationName)
        {
            try
            {
                var response = await requestAction();
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("{OperationName} failed: {Error}", operationName, errorContent);
                    return StatusCode((int)response.StatusCode, new { Message = $"{operationName} failed.", Details = errorContent });
                }
                var successResponse = await response.Content.ReadFromJsonAsync<object>();
                return Ok(successResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during {OperationName}", operationName);
                return StatusCode(500, new { Message = $"An error occurred during {operationName}." });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetReview(int id)
        {
            return await ForwardRequest(
                () => _httpClientFactory.CreateClient().GetAsync($"{CatalogServiceBaseUrl}/api/public/reviews/{id}"),
                "Get review by ID"
            );
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateReview([FromBody] object request)
        {
            return await ForwardRequest(
                () => {
                    var client = _httpClientFactory.CreateClient();
                    AddAuthorizationHeader(client);
                    return client.PostAsJsonAsync($"{CatalogServiceBaseUrl}/api/public/reviews", request);
                },
                "Create product review"
            );
        }

        [HttpPost("{id}/helpful")]
        [Authorize]
        public async Task<IActionResult> MarkReviewHelpful(int id)
        {
            return await ForwardRequest(
                () => {
                    var client = _httpClientFactory.CreateClient();
                    AddAuthorizationHeader(client);
                    return client.PostAsync($"{CatalogServiceBaseUrl}/api/public/reviews/{id}/helpful", null);
                },
                "Mark review as helpful"
            );
        }

        [HttpGet("product/{productId}")]
        public async Task<IActionResult> GetProductReviews(int productId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string? sortBy = "date", [FromQuery] bool sortAscending = false)
        {
            var queryString = Request.QueryString.ToUriComponent();
            return await ForwardRequest(
                () => _httpClientFactory.CreateClient().GetAsync($"{CatalogServiceBaseUrl}/api/public/reviews/product/{productId}{queryString}"),
                "Get reviews for a product"
            );
        }

        [HttpGet("product/{productId}/stats")]
        public async Task<IActionResult> GetProductReviewStats(int productId)
        {
            return await ForwardRequest(
                () => _httpClientFactory.CreateClient().GetAsync($"{CatalogServiceBaseUrl}/api/public/reviews/product/{productId}/stats"),
                "Get review stats for a product"
            );
        }
    }
}
