using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MarketHub.Gateway.Controllers
{
    [ApiController]
    [Route("api/vendor/products/{productId}/reviews")]
    [Authorize]
    public class VendorProductReviewController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<VendorProductReviewController> _logger;
        private const string CatalogServiceBaseUrl = "https://localhost:7070";

        public VendorProductReviewController(IHttpClientFactory httpClientFactory, ILogger<VendorProductReviewController> logger)
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

        [HttpGet]
        public async Task<IActionResult> GetReviews(int productId)
        {
            var queryString = Request.QueryString.ToUriComponent();
            return await ForwardRequest(
                () => {
                    var client = _httpClientFactory.CreateClient();
                    AddAuthorizationHeader(client);
                    return client.GetAsync($"{CatalogServiceBaseUrl}/api/vendor/products/{productId}/reviews{queryString}");
                },
                "Get reviews for vendor product"
            );
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetReview(int productId, int id)
        {
            return await ForwardRequest(
                () => {
                    var client = _httpClientFactory.CreateClient();
                    AddAuthorizationHeader(client);
                    return client.GetAsync($"{CatalogServiceBaseUrl}/api/vendor/products/{productId}/reviews/{id}");
                },
                "Get review by ID for vendor product"
            );
        }

        [HttpPost("{id}/approve")]
        public async Task<IActionResult> ApproveReview(int productId, int id)
        {
            return await ForwardRequest(
                () => {
                    var client = _httpClientFactory.CreateClient();
                    AddAuthorizationHeader(client);
                    return client.PostAsync($"{CatalogServiceBaseUrl}/api/vendor/products/{productId}/reviews/{id}/approve", null);
                },
                "Approve review for vendor product"
            );
        }

        [HttpPost("{id}/reject")]
        public async Task<IActionResult> RejectReview(int productId, int id)
        {
            return await ForwardRequest(
                () => {
                    var client = _httpClientFactory.CreateClient();
                    AddAuthorizationHeader(client);
                    return client.PostAsync($"{CatalogServiceBaseUrl}/api/vendor/products/{productId}/reviews/{id}/reject", null);
                },
                "Reject review for vendor product"
            );
        }

        [HttpPost("{id}/verify")]
        public async Task<IActionResult> VerifyReview(int productId, int id)
        {
            return await ForwardRequest(
                () => {
                    var client = _httpClientFactory.CreateClient();
                    AddAuthorizationHeader(client);
                    return client.PostAsync($"{CatalogServiceBaseUrl}/api/vendor/products/{productId}/reviews/{id}/verify", null);
                },
                "Verify review for vendor product"
            );
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetReviewStats(int productId)
        {
            return await ForwardRequest(
                () => {
                    var client = _httpClientFactory.CreateClient();
                    AddAuthorizationHeader(client);
                    return client.GetAsync($"{CatalogServiceBaseUrl}/api/vendor/products/{productId}/reviews/stats");
                },
                "Get review stats for vendor product"
            );
        }
    }
}
