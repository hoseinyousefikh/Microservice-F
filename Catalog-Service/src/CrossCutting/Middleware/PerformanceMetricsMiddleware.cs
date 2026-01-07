using System.Diagnostics;
using System.Net;
using System.Text.Json;
using Catalog_Service.src.CrossCutting.Exceptions;
using AppUnauthorizedAccessException = Catalog_Service.src.CrossCutting.Exceptions.UnauthorizedAccessException;

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
            catch (Exception ex)
            {
                // *** این بخش جدید برای مدیریت خطا اضافه شده است ***
                if (!context.Response.HasStarted)
                {
                    _logger.LogError(ex, "An unhandled exception occurred.");

                    var statusCode = ex switch
                    {
                        NotFoundException => HttpStatusCode.NotFound,
                        AppUnauthorizedAccessException => HttpStatusCode.Unauthorized,
                        BusinessRuleException => HttpStatusCode.BadRequest,
                        DuplicateEntityException => HttpStatusCode.Conflict,
                        InvalidImageException => HttpStatusCode.BadRequest,
                        ServiceUnavailableException => HttpStatusCode.ServiceUnavailable,
                        _ => HttpStatusCode.InternalServerError
                    };

                    context.Response.StatusCode = (int)statusCode;
                    context.Response.ContentType = "application/json";

                    var errorResponse = new { StatusCode = (int)statusCode, Message = ex.Message };
                    var jsonResponse = JsonSerializer.Serialize(errorResponse);

                    // پاسخ خطا را در همان MemoryStream موقت می‌نویسیم
                    await context.Response.Body.WriteAsync(System.Text.Encoding.UTF8.GetBytes(jsonResponse));
                }
                else
                {
                    _logger.LogError(ex, "An error occurred after the response started.");
                }
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

                // *** این بخش اصلاح شده است ***
                // فقط در صورتی هدر را اضافه کن که پاسخ شروع نشده باشد
                if (!context.Response.HasStarted)
                {
                    context.Response.Headers.Add("X-Response-Time-Ms", duration.ToString());
                }
                // *** پایان بخش اصلاح شده ***

                // Collect metrics (in a real implementation, you might send these to a metrics system)
                CollectMetrics(method, path, statusCode, duration);
            }
        }

        private void CollectMetrics(string method, string path, int statusCode, long duration)
        {
            var endpoint = $"{method} {path}";
            _logger.LogDebug("Performance metrics collected: Endpoint={Endpoint}, StatusCode={StatusCode}, Duration={Duration}ms",
                endpoint, statusCode, duration);
        }
    }
}