using StackExchange.Redis;
using System.Text.Json;

namespace Catalog_Service.src._02_Infrastructure.Caching
{
    public class RedisCacheService : ICacheService
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly IDatabase _database;
        private readonly ILogger<RedisCacheService> _logger;
        private readonly string _instanceName;

        public RedisCacheService(IConfiguration configuration, ILogger<RedisCacheService> logger)
        {
            _logger = logger;
            _instanceName = configuration["Redis:InstanceName"] ?? "CatalogService:";

            var connectionString = configuration["Redis:ConnectionString"];
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentException("Redis connection string is not configured");
            }

            _redis = ConnectionMultiplexer.Connect(connectionString);
            _database = _redis.GetDatabase();

            _logger.LogInformation("Redis cache service initialized");
        }

        public async Task<T> GetAsync<T>(string key, CancellationToken cancellationToken = default)
        {
            try
            {
                var fullKey = $"{_instanceName}{key}";
                var value = await _database.StringGetAsync(fullKey);

                if (value.HasValue)
                {
                    _logger.LogDebug("Cache hit for key: {Key}", key);
                    return JsonSerializer.Deserialize<T>(value);
                }

                _logger.LogDebug("Cache miss for key: {Key}", key);
                return default;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting value from cache for key: {Key}", key);
                return default;
            }
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null, CancellationToken cancellationToken = default)
        {
            try
            {
                var fullKey = $"{_instanceName}{key}";
                var serializedValue = JsonSerializer.Serialize(value);

                await _database.StringSetAsync(fullKey, serializedValue, expiry);
                _logger.LogDebug("Value set in cache for key: {Key}", key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting value in cache for key: {Key}", key);
            }
        }

        public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            try
            {
                var fullKey = $"{_instanceName}{key}";
                await _database.KeyDeleteAsync(fullKey);
                _logger.LogDebug("Key removed from cache: {Key}", key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing key from cache: {Key}", key);
            }
        }

        public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
        {
            try
            {
                var fullKey = $"{_instanceName}{key}";
                return await _database.KeyExistsAsync(fullKey);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if key exists in cache: {Key}", key);
                return false;
            }
        }

        public async Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiry = null, CancellationToken cancellationToken = default)
        {
            try
            {
                var cachedValue = await GetAsync<T>(key, cancellationToken);
                if (cachedValue != null)
                {
                    return cachedValue;
                }

                var value = await factory();
                await SetAsync(key, value, expiry, cancellationToken);
                return value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetOrCreate for key: {Key}", key);
                throw;
            }
        }

        public async Task RemoveByPrefixAsync(string prefix, CancellationToken cancellationToken = default)
        {
            try
            {
                var fullPrefix = $"{_instanceName}{prefix}";
                var endpoints = _redis.GetEndPoints();

                foreach (var endpoint in endpoints)
                {
                    var server = _redis.GetServer(endpoint);
                    var keys = server.Keys(pattern: $"{fullPrefix}*");

                    foreach (var key in keys)
                    {
                        await _database.KeyDeleteAsync(key);
                    }
                }

                _logger.LogDebug("Keys with prefix removed from cache: {Prefix}", prefix);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing keys by prefix from cache: {Prefix}", prefix);
            }
        }
    }
}
