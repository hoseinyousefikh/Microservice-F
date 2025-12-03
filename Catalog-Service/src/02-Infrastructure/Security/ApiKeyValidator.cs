namespace Catalog_Service.src._02_Infrastructure.Security
{
    public class ApiKeyValidator
    {
        private readonly IConfiguration _configuration;
        private readonly IKeyManagementService _keyManagementService;
        private readonly ILogger<ApiKeyValidator> _logger;

        public ApiKeyValidator(
            IConfiguration configuration,
            IKeyManagementService keyManagementService,
            ILogger<ApiKeyValidator> logger)
        {
            _configuration = configuration;
            _keyManagementService = keyManagementService;
            _logger = logger;
        }

        public async Task<bool> ValidateAsync(HttpRequest request)
        {
            // Check if API key validation is enabled
            if (!bool.Parse(_configuration["ApiKeySettings:Enabled"] ?? "true"))
                return true;

            // Try to get API key from header
            if (!request.Headers.TryGetValue("X-API-Key", out var apiKeyValues))
            {
                _logger.LogWarning("API key is missing in the request");
                return false;
            }

            var apiKey = apiKeyValues.FirstOrDefault();
            if (string.IsNullOrEmpty(apiKey))
            {
                _logger.LogWarning("API key is empty");
                return false;
            }

            // Extract key ID from API key (format: "keyId:actualKey")
            var keyParts = apiKey.Split(':');
            if (keyParts.Length != 2)
            {
                _logger.LogWarning("Invalid API key format");
                return false;
            }

            var keyId = keyParts[0];
            var key = keyParts[1];

            // Validate the key
            var isValid = await _keyManagementService.ValidateKeyAsync(keyId, key);
            if (!isValid)
            {
                _logger.LogWarning("Invalid API key provided");
                return false;
            }

            return true;
        }
    }
}
