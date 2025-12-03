using Catalog_Service.src._02_Infrastructure.Logging;

namespace Catalog_Service.src.CrossCutting.Middleware
{
    public class CorrelationIdMiddleware
    {
        private readonly RequestDelegate _next;

        public CorrelationIdMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Check if correlation ID is already present in the request header
            if (!context.Request.Headers.TryGetValue("X-Correlation-ID", out var correlationId) ||
                string.IsNullOrEmpty(correlationId))
            {
                // Generate a new correlation ID
                correlationId = Guid.NewGuid().ToString();
            }

            // Add correlation ID to the response header
            context.Response.OnStarting(() =>
            {
                if (!context.Response.Headers.ContainsKey("X-Correlation-ID"))
                {
                    context.Response.Headers.Add("X-Correlation-ID", correlationId);
                }
                return Task.CompletedTask;
            });

            // Set correlation ID in the current context for logging
            CorrelationContextManager.Current = new CorrelationContext
            {
                CorrelationId = correlationId,
                RequestPath = context.Request.Path,
                IpAddress = context.Connection.RemoteIpAddress?.ToString(),
                StartTime = DateTime.UtcNow
            };

            await _next(context);
        }
    }
}
