using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MarketHub.Gateway.Controllers
{
    [ApiController]
    [Route("api/admin/productvariants")]
    [Authorize]
    public class AdminProductVariantController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<AdminProductVariantController> _logger;
        private const string CatalogServiceBaseUrl = "https://localhost:7070";

        public AdminProductVariantController(IHttpClientFactory httpClientFactory, ILogger<AdminProductVariantController> logger)
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

        [HttpGet("product/{productId}")]
        public async Task<IActionResult> GetProductVariants(int productId)
        {
            return await ForwardRequest(
                () => {
                    var client = _httpClientFactory.CreateClient();
                    AddAuthorizationHeader(client);
                    return client.GetAsync($"{CatalogServiceBaseUrl}/api/admin/productvariants/product/{productId}");
                },
                "Get product variants for admin"
            );
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductVariant(int id)
        {
            return await ForwardRequest(
                () => {
                    var client = _httpClientFactory.CreateClient();
                    AddAuthorizationHeader(client);
                    return client.GetAsync($"{CatalogServiceBaseUrl}/api/admin/productvariants/{id}");
                },
                "Get product variant by ID for admin"
            );
        }

        [HttpPost]
        public async Task<IActionResult> CreateProductVariant([FromBody] object request)
        {
            return await ForwardRequest(
                () => {
                    var client = _httpClientFactory.CreateClient();
                    AddAuthorizationHeader(client);
                    return client.PostAsJsonAsync($"{CatalogServiceBaseUrl}/api/admin/productvariants", request);
                },
                "Create product variant"
            );
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProductVariant(int id, [FromBody] object request)
        {
            return await ForwardRequest(
                () => {
                    var client = _httpClientFactory.CreateClient();
                    AddAuthorizationHeader(client);
                    return client.PutAsJsonAsync($"{CatalogServiceBaseUrl}/api/admin/productvariants/{id}", request);
                },
                "Update product variant"
            );
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProductVariant(int id)
        {
            return await ForwardRequest(
                () => {
                    var client = _httpClientFactory.CreateClient();
                    AddAuthorizationHeader(client);
                    return client.DeleteAsync($"{CatalogServiceBaseUrl}/api/admin/productvariants/{id}");
                },
                "Delete product variant"
            );
        }

        [HttpPost("{id}/activate")]
        public async Task<IActionResult> ActivateProductVariant(int id)
        {
            return await ForwardRequest(
                () => {
                    var client = _httpClientFactory.CreateClient();
                    AddAuthorizationHeader(client);
                    return client.PostAsync($"{CatalogServiceBaseUrl}/api/admin/productvariants/{id}/activate", null);
                },
                "Activate product variant"
            );
        }

        [HttpPost("{id}/deactivate")]
        public async Task<IActionResult> DeactivateProductVariant(int id)
        {
            return await ForwardRequest(
                () => {
                    var client = _httpClientFactory.CreateClient();
                    AddAuthorizationHeader(client);
                    return client.PostAsync($"{CatalogServiceBaseUrl}/api/admin/productvariants/{id}/deactivate", null);
                },
                "Deactivate product variant"
            );
        }
    }
}

