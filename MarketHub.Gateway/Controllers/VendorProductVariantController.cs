using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MarketHub.Gateway.Controllers
{
    [ApiController]
    [Route("api/vendor/products/{productId}/variants")]
    [Authorize]
    public class VendorProductVariantController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<VendorProductVariantController> _logger;
        private const string CatalogServiceBaseUrl = "https://localhost:7070";

        public VendorProductVariantController(IHttpClientFactory httpClientFactory, ILogger<VendorProductVariantController> logger)
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
        public async Task<IActionResult> GetVariants(int productId)
        {
            return await ForwardRequest(
                () => {
                    var client = _httpClientFactory.CreateClient();
                    AddAuthorizationHeader(client);
                    return client.GetAsync($"{CatalogServiceBaseUrl}/api/vendor/products/{productId}/variants");
                },
                "Get variants for vendor product"
            );
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetVariant(int productId, int id)
        {
            return await ForwardRequest(
                () => {
                    var client = _httpClientFactory.CreateClient();
                    AddAuthorizationHeader(client);
                    return client.GetAsync($"{CatalogServiceBaseUrl}/api/vendor/products/{productId}/variants/{id}");
                },
                "Get variant by ID for vendor product"
            );
        }

        [HttpPost]
        public async Task<IActionResult> CreateVariant(int productId, [FromBody] object request)
        {
            return await ForwardRequest(
                () => {
                    var client = _httpClientFactory.CreateClient();
                    AddAuthorizationHeader(client);
                    return client.PostAsJsonAsync($"{CatalogServiceBaseUrl}/api/vendor/products/{productId}/variants", request);
                },
                "Create variant for vendor product"
            );
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateVariant(int productId, int id, [FromBody] object request)
        {
            return await ForwardRequest(
                () => {
                    var client = _httpClientFactory.CreateClient();
                    AddAuthorizationHeader(client);
                    return client.PutAsJsonAsync($"{CatalogServiceBaseUrl}/api/vendor/products/{productId}/variants/{id}", request);
                },
                "Update variant for vendor product"
            );
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVariant(int productId, int id)
        {
            return await ForwardRequest(
                () => {
                    var client = _httpClientFactory.CreateClient();
                    AddAuthorizationHeader(client);
                    return client.DeleteAsync($"{CatalogServiceBaseUrl}/api/vendor/products/{productId}/variants/{id}");
                },
                "Delete variant for vendor product"
            );
        }

        [HttpPost("{id}/activate")]
        public async Task<IActionResult> ActivateVariant(int productId, int id)
        {
            return await ForwardRequest(
                () => {
                    var client = _httpClientFactory.CreateClient();
                    AddAuthorizationHeader(client);
                    return client.PostAsync($"{CatalogServiceBaseUrl}/api/vendor/products/{productId}/variants/{id}/activate", null);
                },
                "Activate variant for vendor product"
            );
        }

        [HttpPost("{id}/deactivate")]
        public async Task<IActionResult> DeactivateVariant(int productId, int id)
        {
            return await ForwardRequest(
                () => {
                    var client = _httpClientFactory.CreateClient();
                    AddAuthorizationHeader(client);
                    return client.PostAsync($"{CatalogServiceBaseUrl}/api/vendor/products/{productId}/variants/{id}/deactivate", null);
                },
                "Deactivate variant for vendor product"
            );
        }

        [HttpPost("{id}/stock")]
        public async Task<IActionResult> UpdateVariantStock(int productId, int id, [FromBody] object request)
        {
            return await ForwardRequest(
                () => {
                    var client = _httpClientFactory.CreateClient();
                    AddAuthorizationHeader(client);
                    return client.PostAsJsonAsync($"{CatalogServiceBaseUrl}/api/vendor/products/{productId}/variants/{id}/stock", request);
                },
                "Update stock for vendor product variant"
            );
        }
    }
}
