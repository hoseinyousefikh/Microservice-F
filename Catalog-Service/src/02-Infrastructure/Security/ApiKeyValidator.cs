// File: ApiKeyValidator.cs
using Catalog_Service.src.CrossCutting.Extensions;

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

        public async Task<ApiKeyValidationResult> ValidateAsync(string apiKey)
        {
            if (!bool.Parse(_configuration["ApiKeySettings:Enabled"] ?? "true"))
                return new ApiKeyValidationResult { IsValid = true, ServiceName = "DefaultService" };

            if (string.IsNullOrEmpty(apiKey))
            {
                _logger.LogWarning("API key is empty");
                return new ApiKeyValidationResult { IsValid = false };
            }

            var keyParts = apiKey.Split(':');
            if (keyParts.Length != 2)
            {
                _logger.LogWarning("Invalid API key format");
                return new ApiKeyValidationResult { IsValid = false };
            }

            var keyId = keyParts[0];
            var key = keyParts[1];

            // --- کد اصلاح شده ---
            bool isValidKey = await _keyManagementService.ValidateKeyAsync(keyId, key);

            if (!isValidKey)
            {
                _logger.LogWarning("Invalid API key provided");
                return new ApiKeyValidationResult { IsValid = false };
            }

            // نام سرویس را از keyId استخراج می‌کنیم یا از یک منبع دیگر می‌خوانیم
            // در اینجا فرض می‌کنیم keyId همان نام سرویس است
            string serviceName = keyId;

            return new ApiKeyValidationResult { IsValid = true, ServiceName = serviceName };
            // --- پایان کد اصلاح شده ---
        }
    }
}