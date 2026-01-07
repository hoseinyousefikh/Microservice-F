using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MarketHub.Gateway.Controllers
{
    [ApiController]
    [Route("api/admin/brands")] // مسیر در Gateway
    [Authorize] // The AdminPolicy is enforced by the downstream service
    public class AdminBrandController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<AdminBrandController> _logger;
        private const string CatalogServiceBaseUrl = "https://localhost:7070";

        public AdminBrandController(IHttpClientFactory httpClientFactory, ILogger<AdminBrandController> logger)
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
        public async Task<IActionResult> GetBrands()
        {
            return await ForwardRequest(
                () => {
                    var client = _httpClientFactory.CreateClient();
                    AddAuthorizationHeader(client);
                    // اصلاح مسیر: از AdminBrand استفاده می‌کنیم
                    return client.GetAsync($"{CatalogServiceBaseUrl}/api/admin/AdminBrand");
                },
                "Get all admin brands"
            );
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBrand(int id)
        {
            return await ForwardRequest(
                () => {
                    var client = _httpClientFactory.CreateClient();
                    AddAuthorizationHeader(client);
                    // اصلاح مسیر
                    return client.GetAsync($"{CatalogServiceBaseUrl}/api/admin/AdminBrand/{id}");
                },
                "Get admin brand by ID"
            );
        }

        [HttpPost]
        public async Task<IActionResult> CreateBrand([FromBody] object request)
        {
            return await ForwardRequest(
                () => {
                    var client = _httpClientFactory.CreateClient();
                    AddAuthorizationHeader(client);
                    // اصلاح مسیر
                    return client.PostAsJsonAsync($"{CatalogServiceBaseUrl}/api/admin/AdminBrand", request);
                },
                "Create brand"
            );
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBrand(int id, [FromBody] object request)
        {
            return await ForwardRequest(
                () => {
                    var client = _httpClientFactory.CreateClient();
                    AddAuthorizationHeader(client);
                    // اصلاح مسیر
                    return client.PutAsJsonAsync($"{CatalogServiceBaseUrl}/api/admin/AdminBrand/{id}", request);
                },
                "Update brand"
            );
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBrand(int id)
        {
            return await ForwardRequest(
                () => {
                    var client = _httpClientFactory.CreateClient();
                    AddAuthorizationHeader(client);
                    // اصلاح مسیر
                    return client.DeleteAsync($"{CatalogServiceBaseUrl}/api/admin/AdminBrand/{id}");
                },
                "Delete brand"
            );
        }

        [HttpPost("{id}/activate")]
        public async Task<IActionResult> ActivateBrand(int id)
        {
            return await ForwardRequest(
                () => {
                    var client = _httpClientFactory.CreateClient();
                    AddAuthorizationHeader(client);
                    // اصلاح مسیر
                    return client.PostAsync($"{CatalogServiceBaseUrl}/api/admin/AdminBrand/{id}/activate", null);
                },
                "Activate brand"
            );
        }

        [HttpPost("{id}/deactivate")]
        public async Task<IActionResult> DeactivateBrand(int id)
        {
            return await ForwardRequest(
                () => {
                    var client = _httpClientFactory.CreateClient();
                    AddAuthorizationHeader(client);
                    // اصلاح مسیر
                    return client.PostAsync($"{CatalogServiceBaseUrl}/api/admin/AdminBrand/{id}/deactivate", null);
                },
                "Deactivate brand"
            );
        }
    }
}
