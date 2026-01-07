using Microsoft.AspNetCore.Mvc;

namespace MarketHub.Gateway.Controllers
{
    [ApiController]
    [Route("api/public/categories")]
    public class CategoryController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<CategoryController> _logger;
        private const string CatalogServiceBaseUrl = "https://localhost:7070";

        public CategoryController(IHttpClientFactory httpClientFactory, ILogger<CategoryController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
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
                () => _httpClientFactory.CreateClient().GetAsync($"{CatalogServiceBaseUrl}/api/public/categories"),
                "Get categories"
            );
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategory(int id)
        {
            return await ForwardRequest(
                () => _httpClientFactory.CreateClient().GetAsync($"{CatalogServiceBaseUrl}/api/public/categories/{id}"),
                "Get category by ID"
            );
        }

        [HttpGet("{id}/subcategories")]
        public async Task<IActionResult> GetSubCategories(int id)
        {
            return await ForwardRequest(
                () => _httpClientFactory.CreateClient().GetAsync($"{CatalogServiceBaseUrl}/api/public/categories/{id}/subcategories"),
                "Get subcategories"
            );
        }

        [HttpGet("{id}/products")]
        public async Task<IActionResult> GetCategoryProducts(int id, [FromQuery] int count = 20)
        {
            return await ForwardRequest(
                () => _httpClientFactory.CreateClient().GetAsync($"{CatalogServiceBaseUrl}/api/public/categories/{id}/products?count={count}"),
                "Get products by category"
            );
        }

        [HttpGet("root")]
        public async Task<IActionResult> GetRootCategories()
        {
            return await ForwardRequest(
                () => _httpClientFactory.CreateClient().GetAsync($"{CatalogServiceBaseUrl}/api/public/categories/root"),
                "Get root categories"
            );
        }

        [HttpGet("tree")]
        public async Task<IActionResult> GetCategoryTree()
        {
            return await ForwardRequest(
                () => _httpClientFactory.CreateClient().GetAsync($"{CatalogServiceBaseUrl}/api/public/categories/tree"),
                "Get category tree"
            );
        }

        [HttpGet("{id}/path")]
        public async Task<IActionResult> GetCategoryPath(int id)
        {
            return await ForwardRequest(
                () => _httpClientFactory.CreateClient().GetAsync($"{CatalogServiceBaseUrl}/api/public/categories/{id}/path"),
                "Get category path"
            );
        }
    }
}
