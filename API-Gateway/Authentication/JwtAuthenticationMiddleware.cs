using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Threading.Tasks;

namespace API_Gateway.Authentication
{
    public class JwtAuthenticationMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtAuthenticationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            //if (context.Request.Path.StartsWithSegments("/identity-service/api/auth/login") ||
            //    context.Request.Path.StartsWithSegments("/identity-service/api/auth/register") ||
            //    context.Request.Path.StartsWithSegments("/identity-service/api/account/forgot-password") ||
            //    context.Request.Path.StartsWithSegments("/identity-service/api/account/reset-password") ||
            //    context.Request.Path.StartsWithSegments("/identity-service/api/admin/users") ||

            //    context.Request.Path.StartsWithSegments("/identity-service/api/account/confirm-email"))
            //{
                await _next(context);
                //return;
            //}

            //var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            //if (token != null)
            //{
            //    var tokenValidationService = context.RequestServices.GetRequiredService<ITokenValidationService>();
            //    var validationResult = await tokenValidationService.ValidateTokenAsync(token);

            //    if (validationResult.IsValid)
            //    {
            //        context.Items["User"] = validationResult.User;
            //        await _next(context);
            //        return;
            //    }
            //}

            //context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            //await context.Response.WriteAsJsonAsync(new { Success = false, Message = "Unauthorized" });
        }
    }
}