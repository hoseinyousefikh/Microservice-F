using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace MarketHub.Gateway.Controllers
{
    [ApiController]
    [Route("api/public/products")] // مسیر یکسان با کنترلر اصلی
    public class ProductController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<ProductController> _logger;
        private const string CatalogServiceBaseUrl = "https://localhost:7070"; // آدرس سرویس کاتالوگ

        public ProductController(IHttpClientFactory httpClientFactory, ILogger<ProductController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        /// <summary>
        /// یک متد کمکی برای ارسال درخواست به سرویس پایین‌دستی و پردازش پاسخ
        /// این متد از تکرار کد برای مدیریت خطاها و پاسخ‌ها جلوگیری می‌کند.
        /// </summary>
        /// <param name="requestAction">تابعی که درخواست را اجرا می‌کند</param>
        /// <param name="operationName">نام عملیات برای لاگ‌گیری</param>
        /// <returns>نتیجه IActionResult</returns>
        private async Task<IActionResult> ForwardRequest(Func<Task<HttpResponseMessage>> requestAction, string operationName)
        {
            try
            {
                var response = await requestAction();

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("{OperationName} failed: {Error}", operationName, errorContent);

                    try
                    {
                        // تلاش برای دیسریالایز کردن خطا به صورت یک JSON
                        var errorJson = JsonSerializer.Deserialize<object>(errorContent);
                        return StatusCode((int)response.StatusCode, errorJson);
                    }
                    catch (JsonException)
                    {
                        // اگر محتوا JSON نبود، آن را به صورت یک رشته برمی‌گردانیم
                        return StatusCode((int)response.StatusCode, new { Message = $"{operationName} failed.", Details = errorContent });
                    }
                }

                // خواندن پاسخ موفق به صورت یک آبجکت عمومی
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
        public async Task<IActionResult> SearchProducts(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? searchTerm = null,
            [FromQuery] int? categoryId = null,
            [FromQuery] int? brandId = null,
            [FromQuery] decimal? minPrice = null,
            [FromQuery] decimal? maxPrice = null,
            [FromQuery] string? sortBy = null,
            [FromQuery] bool sortAscending = true)
        {
            var queryString = Request.QueryString.ToUriComponent();
            return await ForwardRequest(
                () => {
                    var client = _httpClientFactory.CreateClient();
                    return client.GetAsync($"{CatalogServiceBaseUrl}/api/public/products{queryString}");
                },
                "Product search"
            );
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            return await ForwardRequest(
                () => {
                    var client = _httpClientFactory.CreateClient();
                    return client.GetAsync($"{CatalogServiceBaseUrl}/api/public/products/{id}");
                },
                "Get product by ID"
            );
        }

        [HttpGet("{id}/variants")]
        public async Task<IActionResult> GetProductVariants(int id)
        {
            return await ForwardRequest(
                () => {
                    var client = _httpClientFactory.CreateClient();
                    return client.GetAsync($"{CatalogServiceBaseUrl}/api/public/products/{id}/variants");
                },
                "Get product variants"
            );
        }

        [HttpGet("{id}/reviews")]
        public async Task<IActionResult> GetProductReviews(int id, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string? sortBy = "date", [FromQuery] bool sortAscending = false)
        {
            var queryString = Request.QueryString.ToUriComponent();
            return await ForwardRequest(
                () => {
                    var client = _httpClientFactory.CreateClient();
                    return client.GetAsync($"{CatalogServiceBaseUrl}/api/public/products/{id}/reviews{queryString}");
                },
                "Get product reviews"
            );
        }

        [HttpGet("{id}/stats")]
        public async Task<IActionResult> GetProductStats(int id)
        {
            return await ForwardRequest(
                () => {
                    var client = _httpClientFactory.CreateClient();
                    return client.GetAsync($"{CatalogServiceBaseUrl}/api/public/products/{id}/stats");
                },
                "Get product stats"
            );
        }

        [HttpGet("featured")]
        public async Task<IActionResult> GetFeaturedProducts([FromQuery] int count = 10)
        {
            var queryString = Request.QueryString.ToUriComponent();
            return await ForwardRequest(
                () => {
                    var client = _httpClientFactory.CreateClient();
                    return client.GetAsync($"{CatalogServiceBaseUrl}/api/public/products/featured{queryString}");
                },
                "Get featured products"
            );
        }

        [HttpGet("newest")]
        public async Task<IActionResult> GetNewestProducts([FromQuery] int count = 10)
        {
            var queryString = Request.QueryString.ToUriComponent();
            return await ForwardRequest(
                () => {
                    var client = _httpClientFactory.CreateClient();
                    return client.GetAsync($"{CatalogServiceBaseUrl}/api/public/products/newest{queryString}");
                },
                "Get newest products"
            );
        }

        [HttpGet("bestselling")]
        public async Task<IActionResult> GetBestSellingProducts([FromQuery] int count = 10)
        {
            var queryString = Request.QueryString.ToUriComponent();
            return await ForwardRequest(
                () => {
                    var client = _httpClientFactory.CreateClient();
                    return client.GetAsync($"{CatalogServiceBaseUrl}/api/public/products/bestselling{queryString}");
                },
                "Get best-selling products"
            );
        }

        [HttpGet("by-category/{categoryId}")]
        public async Task<IActionResult> GetProductsByCategory(int categoryId, [FromQuery] int count = 20)
        {
            var queryString = Request.QueryString.ToUriComponent();
            return await ForwardRequest(
                () => {
                    var client = _httpClientFactory.CreateClient();
                    return client.GetAsync($"{CatalogServiceBaseUrl}/api/public/products/by-category/{categoryId}{queryString}");
                },
                "Get products by category"
            );
        }

        [HttpGet("by-brand/{brandId}")]
        public async Task<IActionResult> GetProductsByBrand(int brandId, [FromQuery] int count = 20)
        {
            var queryString = Request.QueryString.ToUriComponent();
            return await ForwardRequest(
                () => {
                    var client = _httpClientFactory.CreateClient();
                    return client.GetAsync($"{CatalogServiceBaseUrl}/api/public/products/by-brand/{brandId}{queryString}");
                },
                "Get products by brand"
            );
        }
    }
}
