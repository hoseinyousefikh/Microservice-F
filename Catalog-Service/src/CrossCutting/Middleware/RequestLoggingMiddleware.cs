using Serilog;
using System.Diagnostics;

namespace Catalog_Service.src.CrossCutting.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var sw = Stopwatch.StartNew();

            try
            {
                await _next(context);
            }
            finally
            {
                sw.Stop();

                var statusCode = context.Response.StatusCode;
                var method = context.Request.Method;
                var path = context.Request.Path;
                var queryString = context.Request.QueryString;
                var duration = sw.ElapsedMilliseconds;

                var correlationId = context.TraceIdentifier;

                Log.Information("Request {Method} {Path}{QueryString} responded with {StatusCode} in {Duration}ms [CorrelationId: {CorrelationId}]",
                    method, path, queryString, statusCode, duration, correlationId);
            }
        }
    }
}
