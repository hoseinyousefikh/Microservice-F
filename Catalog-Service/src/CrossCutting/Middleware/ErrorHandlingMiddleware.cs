using System.Net;
using System.Text.Json;
using Catalog_Service.src.CrossCutting.Exceptions;

namespace Catalog_Service.src.CrossCutting.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                // اگر پاسخ شروع شده بود، فقط لاگ می‌کنیم
                if (context.Response.HasStarted)
                {
                    _logger.LogError(ex, "An error occurred after the response started.");
                    return;
                }

                _logger.LogError(ex, "An unhandled exception occurred.");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            // --- کد اصلاح شده ---
            // نام کامل کلاس سفارشی برای جلوگیری از ابهام استفاده شده است
            var statusCode = exception switch
            {
                Catalog_Service.src.CrossCutting.Exceptions.NotFoundException => HttpStatusCode.NotFound,
                Catalog_Service.src.CrossCutting.Exceptions.UnauthorizedAccessException => HttpStatusCode.Unauthorized,
                Catalog_Service.src.CrossCutting.Exceptions.BusinessRuleException => HttpStatusCode.BadRequest,
                Catalog_Service.src.CrossCutting.Exceptions.DuplicateEntityException => HttpStatusCode.Conflict,
                Catalog_Service.src.CrossCutting.Exceptions.InvalidImageException => HttpStatusCode.BadRequest,
                Catalog_Service.src.CrossCutting.Exceptions.ServiceUnavailableException => HttpStatusCode.ServiceUnavailable,
                _ => HttpStatusCode.InternalServerError
            };
            // --- پایان کد اصلاح شده ---

            context.Response.StatusCode = (int)statusCode;
            context.Response.ContentType = "application/json";

            var errorResponse = new
            {
                StatusCode = (int)statusCode,
                Message = exception.Message
            };

            // از JsonSerializer برای سریالایز کردن استفاده می‌کنیم تا از وابستگی WriteAsJsonAsync کمتر کنیم
            var jsonResponse = JsonSerializer.Serialize(errorResponse);
            await context.Response.WriteAsync(jsonResponse);
        }
    }
}