using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MarketHub.Gateway.Controllers
{
    [ApiController]
    [Route("api/admin/products")] // مسیر در Gateway
    [Authorize]
    public class AdminProductController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<AdminProductController> _logger;
        private const string CatalogServiceBaseUrl = "https://localhost:7070";

        public AdminProductController(IHttpClientFactory httpClientFactory, ILogger<AdminProductController> logger)
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
        public async Task<IActionResult> GetProducts()
        {
            var queryString = Request.QueryString.ToUriComponent();
            return await ForwardRequest(
                () =>
                {
                    var client = _httpClientFactory.CreateClient();
                    AddAuthorizationHeader(client);
                    // اصلاح مسیر: از AdminProduct استفاده می‌کنیم
                    return client.GetAsync($"{CatalogServiceBaseUrl}/api/admin/AdminProduct{queryString}");
                },
                "Get all admin products"
            );
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            return await ForwardRequest(
                () =>
                {
                    var client = _httpClientFactory.CreateClient();
                    AddAuthorizationHeader(client);
                    // اصلاح مسیر
                    return client.GetAsync($"{CatalogServiceBaseUrl}/api/admin/AdminProduct/{id}");
                },
                "Get admin product by ID"
            );
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] object request)
        {
            return await ForwardRequest(
                () =>
                {
                    var client = _httpClientFactory.CreateClient();
                    AddAuthorizationHeader(client);
                    // اصلاح مسیر
                    return client.PostAsJsonAsync($"{CatalogServiceBaseUrl}/api/admin/AdminProduct", request);
                },
                "Create product"
            );
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] object request)
        {
            return await ForwardRequest(
                () =>
                {
                    var client = _httpClientFactory.CreateClient();
                    AddAuthorizationHeader(client);
                    // اصلاح مسیر
                    return client.PutAsJsonAsync($"{CatalogServiceBaseUrl}/api/admin/AdminProduct/{id}", request);
                },
                "Update product"
            );
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            return await ForwardRequest(
                () =>
                {
                    var client = _httpClientFactory.CreateClient();
                    AddAuthorizationHeader(client);
                    // اصلاح مسیر
                    return client.DeleteAsync($"{CatalogServiceBaseUrl}/api/admin/AdminProduct/{id}");
                },
                "Delete product"
            );
        }

        [HttpPost("{id}/publish")]
        public async Task<IActionResult> PublishProduct(int id)
        {
            return await ForwardRequest(
                () =>
                {
                    var client = _httpClientFactory.CreateClient();
                    AddAuthorizationHeader(client);
                    // اصلاح مسیر
                    return client.PostAsync($"{CatalogServiceBaseUrl}/api/admin/AdminProduct/{id}/publish", null);
                },
                "Publish product"
            );
        }

        [HttpPost("{id}/unpublish")]
        public async Task<IActionResult> UnpublishProduct(int id)
        {
            return await ForwardRequest(
                () =>
                {
                    var client = _httpClientFactory.CreateClient();
                    AddAuthorizationHeader(client);
                    // اصلاح مسیر
                    return client.PostAsync($"{CatalogServiceBaseUrl}/api/admin/AdminProduct/{id}/unpublish", null);
                },
                "Unpublish product"
            );
        }

        [HttpPost("{id}/archive")]
        public async Task<IActionResult> ArchiveProduct(int id)
        {
            return await ForwardRequest(
                () =>
                {
                    var client = _httpClientFactory.CreateClient();
                    AddAuthorizationHeader(client);
                    // اصلاح مسیر
                    return client.PostAsync($"{CatalogServiceBaseUrl}/api/admin/AdminProduct/{id}/archive", null);
                },
                "Archive product"
            );
        }

        [HttpPost("{id}/feature")]
        public async Task<IActionResult> SetAsFeatured(int id)
        {
            return await ForwardRequest(
                () =>
                {
                    var client = _httpClientFactory.CreateClient();
                    AddAuthorizationHeader(client);
                    // اصلاح مسیر
                    return client.PostAsync($"{CatalogServiceBaseUrl}/api/admin/AdminProduct/{id}/feature", null);
                },
                "Set product as featured"
            );
        }

        [HttpDelete("{id}/feature")]
        public async Task<IActionResult> RemoveFromFeatured(int id)
        {
            return await ForwardRequest(
                () =>
                {
                    var client = _httpClientFactory.CreateClient();
                    AddAuthorizationHeader(client);
                    // اصلاح مسیر
                    return client.DeleteAsync($"{CatalogServiceBaseUrl}/api/admin/AdminProduct/{id}/feature");
                },
                "Remove product from featured"
            );
        }
    }
}