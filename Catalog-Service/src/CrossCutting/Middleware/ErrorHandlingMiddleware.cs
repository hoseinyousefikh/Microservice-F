using System.Net;

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
                _logger.LogError(ex, "An unhandled exception occurred");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var response = exception switch
            {
                Catalog_Service.src.CrossCutting.Exceptions.NotFoundException =>
                    new { StatusCode = (int)HttpStatusCode.NotFound, Message = exception.Message },
                Catalog_Service.src.CrossCutting.Exceptions.UnauthorizedAccessException =>
                    new { StatusCode = (int)HttpStatusCode.Unauthorized, Message = exception.Message },
                Catalog_Service.src.CrossCutting.Exceptions.BusinessRuleException =>
                    new { StatusCode = (int)HttpStatusCode.BadRequest, Message = exception.Message },
                Catalog_Service.src.CrossCutting.Exceptions.DuplicateEntityException =>
                    new { StatusCode = (int)HttpStatusCode.Conflict, Message = exception.Message },
                Catalog_Service.src.CrossCutting.Exceptions.InvalidImageException =>
                    new { StatusCode = (int)HttpStatusCode.BadRequest, Message = exception.Message },
                Catalog_Service.src.CrossCutting.Exceptions.ServiceUnavailableException =>
                    new { StatusCode = (int)HttpStatusCode.ServiceUnavailable, Message = exception.Message },
                _ => new { StatusCode = (int)HttpStatusCode.InternalServerError, Message = "An internal server error occurred" }
            };

            context.Response.StatusCode = response.StatusCode;

            await context.Response.WriteAsJsonAsync(response);
        }
    }
}
