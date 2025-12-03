using Catalog_Service.src._02_Infrastructure.Caching;

namespace Catalog_Service.src._02_Infrastructure.Security
{
    public class KeyManagementService : IKeyManagementService
    {
        private readonly IConfiguration _configuration;
        private readonly ICacheService _cacheService;

        public KeyManagementService(IConfiguration configuration, ICacheService cacheService)
        {
            _configuration = configuration;
            _cacheService = cacheService;
        }

        public async Task<string> GenerateKeyAsync(string keyId)
        {
            // Generate a cryptographically secure random key
            var keyBytes = new byte[32];
            using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
            {
                rng.GetBytes(keyBytes);
            }

            var key = Convert.ToBase64String(keyBytes);

            // Store the key in cache with expiration
            var expiryMinutes = int.Parse(_configuration["ApiKeySettings:ExpiryMinutes"] ?? "1440");
            await _cacheService.SetAsync($"api_key:{keyId}", key, TimeSpan.FromMinutes(expiryMinutes));

            return key;
        }

        public async Task<bool> ValidateKeyAsync(string keyId, string key)
        {
            var storedKey = await _cacheService.GetAsync<string>($"api_key:{keyId}");

            // Check if key exists and matches
            if (string.IsNullOrEmpty(storedKey) || storedKey != key)
                return false;

            // Check if key is revoked
            return !await IsKeyRevokedAsync(keyId);
        }

        public async Task RevokeKeyAsync(string keyId)
        {
            // Mark the key as revoked in cache
            var expiryMinutes = int.Parse(_configuration["ApiKeySettings:ExpiryMinutes"] ?? "1440");
            await _cacheService.SetAsync($"revoked_key:{keyId}", true, TimeSpan.FromMinutes(expiryMinutes));
        }

        public async Task<bool> IsKeyRevokedAsync(string keyId)
        {
            return await _cacheService.GetAsync<bool>($"revoked_key:{keyId}");
        }
    }
}
