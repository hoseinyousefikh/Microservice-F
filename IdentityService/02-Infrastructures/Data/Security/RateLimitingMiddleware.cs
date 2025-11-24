using System.Collections.Concurrent;
using System.Threading.RateLimiting;

namespace IdentityService._02_Infrastructures.Data.Security
{
    public class RateLimitingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ConcurrentDictionary<string, FixedWindowRateLimiter> _limiters;
        private readonly RateLimiterOptions _options;

        public RateLimitingMiddleware(RequestDelegate next, RateLimiterOptions options)
        {
            _next = next;
            _options = options;
            _limiters = new ConcurrentDictionary<string, FixedWindowRateLimiter>();
        }

        public async Task Invoke(HttpContext context)
        {
            var clientIp = context.Connection.RemoteIpAddress?.ToString();
            if (clientIp == null)
            {
                await _next(context);
                return;
            }

            var limiter = _limiters.GetOrAdd(clientIp, ip =>
                new FixedWindowRateLimiter(new FixedWindowRateLimiterOptions
                {
                    Window = TimeSpan.FromMinutes(1),
                    PermitLimit = _options.PermitLimit,
                    QueueLimit = _options.QueueLimit
                }));

            using var lease = await limiter.AcquireAsync();
            if (lease.IsAcquired)
            {
                await _next(context);
            }
            else
            {
                context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                await context.Response.WriteAsync("Too many requests. Please try again later.");
            }
        }
    }

    public class RateLimiterOptions
    {
        public int PermitLimit { get; set; } = 100;
        public int QueueLimit { get; set; } = 0;
    }
}
