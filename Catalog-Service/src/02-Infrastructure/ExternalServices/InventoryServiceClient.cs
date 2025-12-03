using Polly;
using Polly.Retry;
using System.Text.Json;

namespace Catalog_Service.src._02_Infrastructure.ExternalServices
{
    public class InventoryServiceClient : IInventoryServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<InventoryServiceClient> _logger;
        private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;

        public InventoryServiceClient(
            HttpClient httpClient,
            IConfiguration configuration,
            ILogger<InventoryServiceClient> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;

            var baseUrl = _configuration["ExternalServices:InventoryService:BaseUrl"];
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
                            "Retry {RetryAttempt} for inventory service call. Status: {StatusCode}. Waiting {TimeSpan} before next retry.",
                            retryAttempt,
                            outcome.Result?.StatusCode,
                            timespan);
                    });
        }

        public async Task<InventoryStatus> GetInventoryStatusAsync(int productId, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _retryPolicy.ExecuteAsync(() =>
                    _httpClient.GetAsync($"api/inventory/{productId}", cancellationToken));

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync(cancellationToken);
                    var result = JsonSerializer.Deserialize<InventoryResponse>(content);
                    return result?.Status ?? InventoryStatus.OutOfStock;
                }

                _logger.LogError("Failed to get inventory status for product {ProductId}. Status: {StatusCode}",
                    productId, response.StatusCode);
                return InventoryStatus.OutOfStock;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting inventory status for product {ProductId}", productId);
                return InventoryStatus.OutOfStock;
            }
        }

        public async Task<InventoryStatus> GetInventoryStatusBySkuAsync(string sku, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _retryPolicy.ExecuteAsync(() =>
                    _httpClient.GetAsync($"api/inventory/sku/{sku}", cancellationToken));

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync(cancellationToken);
                    var result = JsonSerializer.Deserialize<InventoryResponse>(content);
                    return result?.Status ?? InventoryStatus.OutOfStock;
                }

                _logger.LogError("Failed to get inventory status for SKU {Sku}. Status: {StatusCode}",
                    sku, response.StatusCode);
                return InventoryStatus.OutOfStock;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting inventory status for SKU {Sku}", sku);
                return InventoryStatus.OutOfStock;
            }
        }

        public async Task<bool> UpdateInventoryAsync(int productId, int quantity, CancellationToken cancellationToken = default)
        {
            try
            {
                var request = new UpdateInventoryRequest { Quantity = quantity };
                var content = new StringContent(
                    JsonSerializer.Serialize(request),
                    System.Text.Encoding.UTF8,
                    "application/json");

                var response = await _retryPolicy.ExecuteAsync(() =>
                    _httpClient.PutAsync($"api/inventory/{productId}", content, cancellationToken));

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }

                _logger.LogError("Failed to update inventory for product {ProductId}. Status: {StatusCode}",
                    productId, response.StatusCode);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating inventory for product {ProductId}", productId);
                return false;
            }
        }

        public async Task<bool> ReserveInventoryAsync(int productId, int quantity, CancellationToken cancellationToken = default)
        {
            try
            {
                var request = new ReserveInventoryRequest { Quantity = quantity };
                var content = new StringContent(
                    JsonSerializer.Serialize(request),
                    System.Text.Encoding.UTF8,
                    "application/json");

                var response = await _retryPolicy.ExecuteAsync(() =>
                    _httpClient.PostAsync($"api/inventory/{productId}/reserve", content, cancellationToken));

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<bool>(cancellationToken: cancellationToken);
                    return result;
                }

                _logger.LogError("Failed to reserve inventory for product {ProductId}. Status: {StatusCode}",
                    productId, response.StatusCode);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reserving inventory for product {ProductId}", productId);
                return false;
            }
        }

        public async Task<bool> ReleaseInventoryAsync(int productId, int quantity, CancellationToken cancellationToken = default)
        {
            try
            {
                var request = new ReleaseInventoryRequest { Quantity = quantity };
                var content = new StringContent(
                    JsonSerializer.Serialize(request),
                    System.Text.Encoding.UTF8,
                    "application/json");

                var response = await _retryPolicy.ExecuteAsync(() =>
                    _httpClient.PostAsync($"api/inventory/{productId}/release", content, cancellationToken));

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }

                _logger.LogError("Failed to release inventory for product {ProductId}. Status: {StatusCode}",
                    productId, response.StatusCode);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error releasing inventory for product {ProductId}", productId);
                return false;
            }
        }

        public async Task<bool> CheckInventoryAvailabilityAsync(int productId, int quantity, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _retryPolicy.ExecuteAsync(() =>
                    _httpClient.GetAsync($"api/inventory/{productId}/check?quantity={quantity}", cancellationToken));

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<bool>(cancellationToken: cancellationToken);
                    return result;
                }

                _logger.LogError("Failed to check inventory availability for product {ProductId}. Status: {StatusCode}",
                    productId, response.StatusCode);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking inventory availability for product {ProductId}", productId);
                return false;
            }
        }
    }

    internal class InventoryResponse
    {
        public InventoryStatus Status { get; set; }
        public int AvailableQuantity { get; set; }
    }

    internal class UpdateInventoryRequest
    {
        public int Quantity { get; set; }
    }

    internal class ReserveInventoryRequest
    {
        public int Quantity { get; set; }
    }

    internal class ReleaseInventoryRequest
    {
        public int Quantity { get; set; }
    }
}
