using System.Security.Claims;

namespace Catalog_Service.src._02_Infrastructure.Security
{
    public class ServiceAuthenticationHandler
    {
        private readonly RequestDelegate _next;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly ApiKeyValidator _apiKeyValidator;
        private readonly ILogger<ServiceAuthenticationHandler> _logger;

        public ServiceAuthenticationHandler(
            RequestDelegate next,
            IJwtTokenGenerator jwtTokenGenerator,
            ApiKeyValidator apiKeyValidator,
            ILogger<ServiceAuthenticationHandler> logger)
        {
            _next = next;
            _jwtTokenGenerator = jwtTokenGenerator;
            _apiKeyValidator = apiKeyValidator;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Skip authentication for health checks and other public endpoints
            if (IsPublicEndpoint(context.Request.Path))
            {
                await _next(context);
                return;
            }

            var isAuthenticated = false;

            // Try JWT authentication first
            var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
            {
                var token = authHeader.Substring("Bearer ".Length).Trim();
                var principal = _jwtTokenGenerator.ValidateToken(token);

                if (principal != null)
                {
                    context.User = principal;
                    isAuthenticated = true;
                    _logger.LogDebug("User authenticated via JWT");
                }
            }

            // If JWT failed, try API key authentication
            if (!isAuthenticated)
            {
                if (await _apiKeyValidator.ValidateAsync(context.Request))
                {
                    // Create a simple identity for API key authentication
                    var claims = new[] { new Claim(ClaimTypes.Role, "Service") };
                    var identity = new ClaimsIdentity(claims, "ApiKey");
                    context.User = new ClaimsPrincipal(identity);
                    isAuthenticated = true;
                    _logger.LogDebug("Service authenticated via API key");
                }
            }

            if (!isAuthenticated)
            {
                _logger.LogWarning("Authentication failed for request to {Path}", context.Request.Path);
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Unauthorized");
                return;
            }

            await _next(context);
        }

        private bool IsPublicEndpoint(string path)
        {
            // Define public endpoints that don't require authentication
            var publicEndpoints = new[]
            {
                "/health",
                "/metrics",
                "/api/public/products",
                "/api/public/categories",
                "/api/public/brands"
            };

            return publicEndpoints.Any(endpoint => path.StartsWith(endpoint, StringComparison.OrdinalIgnoreCase));
        }
    }
}
