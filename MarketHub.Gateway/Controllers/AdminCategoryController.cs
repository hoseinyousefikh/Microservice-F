using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace MarketHub.Gateway.Controllers
{
    [ApiController]
    [Route("api/admin/categories")] // مسیر در Gateway
    [Authorize] // The AdminPolicy is enforced by the downstream service
    public class AdminCategoryController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<AdminCategoryController> _logger;
        private const string CatalogServiceBaseUrl = "https://localhost:7070";

        public AdminCategoryController(IHttpClientFactory httpClientFactory, ILogger<AdminCategoryController> logger)
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
        public async Task<IActionResult> GetCategories()
        {
            return await ForwardRequest(
                () => {
                    var client = _httpClientFactory.CreateClient();
                    AddAuthorizationHeader(client);
                    // اصلاح مسیر: از AdminCategory استفاده می‌کنیم
                    return client.GetAsync($"{CatalogServiceBaseUrl}/api/admin/AdminCategory");
                },
                "Get all admin categories"
            );
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategory(int id)
        {
            return await ForwardRequest(
                () => {
                    var client = _httpClientFactory.CreateClient();
                    AddAuthorizationHeader(client);
                    // اصلاح مسیر
                    return client.GetAsync($"{CatalogServiceBaseUrl}/api/admin/AdminCategory/{id}");
                },
                "Get admin category by ID"
            );
        }

        [HttpGet("tree")]
        public async Task<IActionResult> GetCategoryTree()
        {
            return await ForwardRequest(
                () => {
                    var client = _httpClientFactory.CreateClient();
                    AddAuthorizationHeader(client);
                    // اصلاح مسیر
                    return client.GetAsync($"{CatalogServiceBaseUrl}/api/admin/AdminCategory/tree");
                },
                "Get admin category tree"
            );
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] object request)
        {
            return await ForwardRequest(
                () => {
                    var client = _httpClientFactory.CreateClient();
                    AddAuthorizationHeader(client);
                    // اصلاح مسیر
                    return client.PostAsJsonAsync($"{CatalogServiceBaseUrl}/api/admin/AdminCategory", request);
                },
                "Create category"
            );
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] object request)
        {
            return await ForwardRequest(
                () => {
                    var client = _httpClientFactory.CreateClient();
                    AddAuthorizationHeader(client);
                    // اصلاح مسیر
                    return client.PutAsJsonAsync($"{CatalogServiceBaseUrl}/api/admin/AdminCategory/{id}", request);
                },
                "Update category"
            );
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            return await ForwardRequest(
                () => {
                    var client = _httpClientFactory.CreateClient();
                    AddAuthorizationHeader(client);
                    // اصلاح مسیر
                    return client.DeleteAsync($"{CatalogServiceBaseUrl}/api/admin/AdminCategory/{id}");
                },
                "Delete category"
            );
        }

        [HttpPost("{id}/activate")]
        public async Task<IActionResult> ActivateCategory(int id)
        {
            return await ForwardRequest(
                () => {
                    var client = _httpClientFactory.CreateClient();
                    AddAuthorizationHeader(client);
                    // اصلاح مسیر
                    return client.PostAsync($"{CatalogServiceBaseUrl}/api/admin/AdminCategory/{id}/activate", null);
                },
                "Activate category"
            );
        }

        [HttpPost("{id}/deactivate")]
        public async Task<IActionResult> DeactivateCategory(int id)
        {
            return await ForwardRequest(
                () => {
                    var client = _httpClientFactory.CreateClient();
                    AddAuthorizationHeader(client);
                    // اصلاح مسیر
                    return client.PostAsync($"{CatalogServiceBaseUrl}/api/admin/AdminCategory/{id}/deactivate", null);
                },
                "Deactivate category"
            );
        }

        [HttpPost("{id}/move")]
        public async Task<IActionResult> MoveCategory(int id, [FromBody] object request)
        {
            return await ForwardRequest(
                () => {
                    var client = _httpClientFactory.CreateClient();
                    AddAuthorizationHeader(client);
                    // اصلاح مسیر
                    return client.PostAsJsonAsync($"{CatalogServiceBaseUrl}/api/admin/AdminCategory/{id}/move", request);
                },
                "Move category"
            );
        }
    }
}