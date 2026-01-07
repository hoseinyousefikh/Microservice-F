using Catalog_Service.src.CrossCutting.Middleware;
using Serilog;

namespace Catalog_Service.src.CrossCutting.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseCatalogMiddleware(this IApplicationBuilder app, IConfiguration configuration)
        {
            // Use Serilog request logging
            app.UseSerilogRequestLogging();

            // Use correlation ID middleware
            app.UseMiddleware<CorrelationIdMiddleware>();

            // Use error handling middleware
            //app.UseMiddleware<ErrorHandlingMiddleware>();

            // Use rate limiting middleware
            app.UseMiddleware<RateLimitingMiddleware>();

            // Use performance metrics middleware
            app.UseMiddleware<PerformanceMetricsMiddleware>();

            // Use request logging middleware
            app.UseMiddleware<RequestLoggingMiddleware>();

            return app;
        }

        public static IApplicationBuilder UseCatalogHealthChecks(this IApplicationBuilder app)
        {
            app.UseHealthChecks("/health");
            return app;
        }
    }
}
