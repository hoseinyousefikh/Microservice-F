using System.Diagnostics;

namespace Catalog_Service.src.CrossCutting.Middleware
{
    public class PerformanceMetricsMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;
        private readonly ILogger<PerformanceMetricsMiddleware> _logger;

        public PerformanceMetricsMiddleware(
            RequestDelegate next,
            IConfiguration configuration,
            ILogger<PerformanceMetricsMiddleware> logger)
        {
            _next = next;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var sw = Stopwatch.StartNew();
            var originalBodyStream = context.Response.Body;

            // Create a new memory stream to capture the response
            using var responseBody = new System.IO.MemoryStream();
            context.Response.Body = responseBody;

            try
            {
                await _next(context);
            }
            finally
            {
                sw.Stop();
                var duration = sw.ElapsedMilliseconds;

                // Reset response body stream
                responseBody.Seek(0, System.IO.SeekOrigin.Begin);
                await responseBody.CopyToAsync(originalBodyStream);

                // Log performance metrics
                var statusCode = context.Response.StatusCode;
                var method = context.Request.Method;
                var path = context.Request.Path;

                // Get performance threshold from configuration
                var slowRequestThreshold = int.Parse(_configuration["PerformanceMetrics:SlowRequestThresholdMs"] ?? "1000");

                // Log slow requests
                if (duration > slowRequestThreshold)
                {
                    _logger.LogWarning("Slow request detected: {Method} {Path} took {Duration}ms with status {StatusCode}",
                        method, path, duration, statusCode);
                }

                // Add performance metrics header
                context.Response.Headers.Add("X-Response-Time-Ms", duration.ToString());

                // Collect metrics (in a real implementation, you might send these to a metrics system)
                CollectMetrics(method, path, statusCode, duration);
            }
        }

        private void CollectMetrics(string method, string path, int statusCode, long duration)
        {
            // In a real implementation, you would send these metrics to a system like Prometheus, InfluxDB, etc.
            // For now, we'll just log them

            var endpoint = $"{method} {path}";

            _logger.LogDebug("Performance metrics collected: Endpoint={Endpoint}, StatusCode={StatusCode}, Duration={Duration}ms",
                endpoint, statusCode, duration);
        }
    }
}
