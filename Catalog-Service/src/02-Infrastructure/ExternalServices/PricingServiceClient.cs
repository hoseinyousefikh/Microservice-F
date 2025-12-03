using Catalog_Service.src._01_Domain.Core.Primitives;
using Polly;
using Polly.Retry;
using System.Text.Json;

namespace Catalog_Service.src._02_Infrastructure.ExternalServices
{
    public class PricingServiceClient : IPricingServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<PricingServiceClient> _logger;
        private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;

        public PricingServiceClient(
            HttpClient httpClient,
            IConfiguration configuration,
            ILogger<PricingServiceClient> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;

            var baseUrl = _configuration["ExternalServices:PricingService:BaseUrl"];
            if (!string.IsNullOrEmpty(baseUrl))
            {
                _httpClient.BaseAddress = new Uri(baseUrl);
            }

            // Configure retry policy
            _retryPolicy = Policy<HttpResponseMessage>
                .Handle<HttpRequestException>()
                .OrResult(r => !r.IsSuccessStatusCode)
                .WaitAndRetryAsync(
                    retryCount: 3,
                    sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    onRetry: (outcome, timespan, retryAttempt, context) =>
                    {
                        _logger.LogWarning(
                            "Retry {RetryAttempt} for pricing service call. Status: {StatusCode}. Waiting {TimeSpan} before next retry.",
                            retryAttempt,
                            outcome.Result?.StatusCode,
                            timespan);
                    });
        }

        public async Task<Money> GetProductPriceAsync(int productId, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _retryPolicy.ExecuteAsync(() =>
                    _httpClient.GetAsync($"api/pricing/{productId}", cancellationToken));

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync(cancellationToken);
                    var result = JsonSerializer.Deserialize<PricingResponse>(content);
                    return Money.Create(result.Price, result.Currency);
                }

                _logger.LogError("Failed to get product price for product {ProductId}. Status: {StatusCode}",
                    productId, response.StatusCode);
                return Money.Create(0, "USD");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting product price for product {ProductId}", productId);
                return Money.Create(0, "USD");
            }
        }

        public async Task<Money> GetProductPriceBySkuAsync(string sku, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _retryPolicy.ExecuteAsync(() =>
                    _httpClient.GetAsync($"api/pricing/sku/{sku}", cancellationToken));

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync(cancellationToken);
                    var result = JsonSerializer.Deserialize<PricingResponse>(content);
                    return Money.Create(result.Price, result.Currency);
                }

                _logger.LogError("Failed to get product price for SKU {Sku}. Status: {StatusCode}",
                    sku, response.StatusCode);
                return Money.Create(0, "USD");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting product price for SKU {Sku}", sku);
                return Money.Create(0, "USD");
            }
        }

        public async Task<Money> GetProductDiscountPriceAsync(int productId, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _retryPolicy.ExecuteAsync(() =>
                    _httpClient.GetAsync($"api/pricing/{productId}/discount", cancellationToken));

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync(cancellationToken);
                    var result = JsonSerializer.Deserialize<PricingResponse>(content);
                    return Money.Create(result.Price, result.Currency);
                }

                _logger.LogError("Failed to get product discount price for product {ProductId}. Status: {StatusCode}",
                    productId, response.StatusCode);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting product discount price for product {ProductId}", productId);
                return null;
            }
        }

        public async Task<bool> UpdateProductPriceAsync(int productId, Money price, CancellationToken cancellationToken = default)
        {
            try
            {
                var request = new UpdatePriceRequest { Price = price.Amount, Currency = price.Currency };
                var content = new StringContent(
                    JsonSerializer.Serialize(request),
                    System.Text.Encoding.UTF8,
                    "application/json");

                var response = await _retryPolicy.ExecuteAsync(() =>
                    _httpClient.PutAsync($"api/pricing/{productId}", content, cancellationToken));

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }

                _logger.LogError("Failed to update product price for product {ProductId}. Status: {StatusCode}",
                    productId, response.StatusCode);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product price for product {ProductId}", productId);
                return false;
            }
        }

        public async Task<bool> ApplyDiscountAsync(int productId, decimal discountPercentage, CancellationToken cancellationToken = default)
        {
            try
            {
                var request = new ApplyDiscountRequest { DiscountPercentage = discountPercentage };
                var content = new StringContent(
                    JsonSerializer.Serialize(request),
                    System.Text.Encoding.UTF8,
                    "application/json");

                var response = await _retryPolicy.ExecuteAsync(() =>
                    _httpClient.PostAsync($"api/pricing/{productId}/discount", content, cancellationToken));

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }

                _logger.LogError("Failed to apply discount for product {ProductId}. Status: {StatusCode}",
                    productId, response.StatusCode);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error applying discount for product {ProductId}", productId);
                return false;
            }
        }

        public async Task<bool> RemoveDiscountAsync(int productId, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _retryPolicy.ExecuteAsync(() =>
                    _httpClient.DeleteAsync($"api/pricing/{productId}/discount", cancellationToken));

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }

                _logger.LogError("Failed to remove discount for product {ProductId}. Status: {StatusCode}",
                    productId, response.StatusCode);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing discount for product {ProductId}", productId);
                return false;
            }
        }

        public async Task<PriceHistory> GetPriceHistoryAsync(int productId, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _retryPolicy.ExecuteAsync(() =>
                    _httpClient.GetAsync($"api/pricing/{productId}/history", cancellationToken));

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync(cancellationToken);
                    var result = JsonSerializer.Deserialize<PriceHistoryResponse>(content);
                    return new PriceHistory
                    {
                        ProductId = result.ProductId,
                        OriginalPrice = result.OriginalPrice,
                        DiscountPrice = result.DiscountPrice,
                        DiscountPercentage = result.DiscountPercentage,
                        EffectiveFrom = result.EffectiveFrom,
                        EffectiveTo = result.EffectiveTo
                    };
                }

                _logger.LogError("Failed to get price history for product {ProductId}. Status: {StatusCode}",
                    productId, response.StatusCode);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting price history for product {ProductId}", productId);
                return null;
            }
        }
    }

    internal class PricingResponse
    {
        public decimal Price { get; set; }
        public string Currency { get; set; }
    }

    internal class UpdatePriceRequest
    {
        public decimal Price { get; set; }
        public string Currency { get; set; }
    }

    internal class ApplyDiscountRequest
    {
        public decimal DiscountPercentage { get; set; }
    }

    internal class PriceHistoryResponse
    {
        public int ProductId { get; set; }
        public decimal OriginalPrice { get; set; }
        public decimal? DiscountPrice { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public DateTime EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
    }
}
