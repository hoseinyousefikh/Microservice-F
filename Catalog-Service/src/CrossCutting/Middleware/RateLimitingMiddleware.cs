using Microsoft.Extensions.Caching.Memory;

namespace Catalog_Service.src.CrossCutting.Middleware
{
    public class RateLimitingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IMemoryCache _cache;
        private readonly IConfiguration _configuration;
        private readonly ILogger<RateLimitingMiddleware> _logger;

        public RateLimitingMiddleware(
            RequestDelegate next,
            IMemoryCache cache,
            IConfiguration configuration,
            ILogger<RateLimitingMiddleware> logger)
        {
            _next = next;
            _cache = cache;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Skip rate limiting for certain paths
            var path = context.Request.Path.Value;
            if (IsExcludedPath(path))
            {
                await _next(context);
                return;
            }

            // Get client identifier (IP address or authenticated user ID)
            var clientId = GetClientId(context);

            // Get rate limit settings
            var rateLimitSettings = _configuration.GetSection("RateLimiting");
            var requestsPerMinute = int.Parse(rateLimitSettings["RequestsPerMinute"] ?? "60");
            var banDurationMinutes = int.Parse(rateLimitSettings["BanDurationMinutes"] ?? "5");

            // Check if client is banned
            var banKey = $"RateLimit_Ban_{clientId}";
            if (_cache.TryGetValue(banKey, out _))
            {
                context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                await context.Response.WriteAsync("Too many requests. Please try again later.");
                return;
            }

            // Check rate limit
            var cacheKey = $"RateLimit_{clientId}_{DateTime.UtcNow:yyyyMMddHHmm}";
            var currentRequestCount = _cache.GetOrCreate(cacheKey, entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1);
                return 0;
            });

            if (currentRequestCount >= requestsPerMinute)
            {
                // Ban the client
                _cache.Set(banKey, true, TimeSpan.FromMinutes(banDurationMinutes));

                _logger.LogWarning("Client {ClientId} has been rate limited and banned for {BanDurationMinutes} minutes",
                    clientId, banDurationMinutes);

                context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                await context.Response.WriteAsync("Too many requests. Please try again later.");
                return;
            }

            // Increment request count
            _cache.Set(cacheKey, currentRequestCount + 1);

            // Add rate limit headers
            context.Response.Headers.Add("X-RateLimit-Limit", requestsPerMinute.ToString());
            context.Response.Headers.Add("X-RateLimit-Remaining", (requestsPerMinute - currentRequestCount - 1).ToString());
            context.Response.Headers.Add("X-RateLimit-Reset", DateTimeOffset.UtcNow.AddMinutes(1 - DateTimeOffset.UtcNow.Minute).ToUnixTimeSeconds().ToString());

            await _next(context);
        }

        private bool IsExcludedPath(string path)
        {
            var excludedPaths = _configuration.GetSection("RateLimiting:ExcludedPaths").Get<string[]>();
            return excludedPaths?.Any(excludedPath => path.StartsWith(excludedPath, StringComparison.OrdinalIgnoreCase)) ?? false;
        }

        private string GetClientId(HttpContext context)
        {
            // If user is authenticated, use user ID
            if (context.User.Identity.IsAuthenticated)
            {
                return context.User.FindFirst("sub")?.Value ?? context.User.Identity.Name;
            }

            // Otherwise use IP address
            return context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        }
    }
}
