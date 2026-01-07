using Microsoft.AspNetCore.Mvc;

namespace MarketHub.Gateway.Controllers
{
    [ApiController]
    [Route("api/public/brands")]
    public class BrandController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<BrandController> _logger;
        private const string CatalogServiceBaseUrl = "https://localhost:7070";

        public BrandController(IHttpClientFactory httpClientFactory, ILogger<BrandController> logger)
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
        public async Task<IActionResult> GetBrands()
        {
            return await ForwardRequest(
                () => _httpClientFactory.CreateClient().GetAsync($"{CatalogServiceBaseUrl}/api/public/brands"),
                "Get brands"
            );
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBrand(int id)
        {
            return await ForwardRequest(
                () => _httpClientFactory.CreateClient().GetAsync($"{CatalogServiceBaseUrl}/api/public/brands/{id}"),
                "Get brand by ID"
            );
        }

        [HttpGet("{id}/products")]
        public async Task<IActionResult> GetBrandProducts(int id, [FromQuery] int count)
        {
            return await ForwardRequest(
                () => _httpClientFactory.CreateClient().GetAsync($"{CatalogServiceBaseUrl}/api/public/brands/{id}/products?count={count}"),
                "Get products by brand"
            );
        }

        [HttpGet("top")]
        public async Task<IActionResult> GetTopBrands([FromQuery] int count)
        {
            return await ForwardRequest(
                () => _httpClientFactory.CreateClient().GetAsync($"{CatalogServiceBaseUrl}/api/public/brands/top?count={count}"),
                "Get top brands"
            );
        }

        [HttpGet("featured")]
        public async Task<IActionResult> GetFeaturedBrands([FromQuery] int count)
        {
            return await ForwardRequest(
                () => _httpClientFactory.CreateClient().GetAsync($"{CatalogServiceBaseUrl}/api/public/brands/featured?count={count}"),
                "Get featured brands"
            );
        }
    }
}
