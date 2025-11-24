using System.Text.RegularExpressions;

namespace IdentityService._02_Infrastructures.Data.Security
{
    public class AntiXssMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string[] _dangerousPatterns = {
            @"<script.*?>.*?</script>",
            @"javascript:",
            @"on\w+\s*=",
            @"eval\s*\(",
            @"expression\s*\("
        };

        public AntiXssMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Method == HttpMethods.Post ||
                context.Request.Method == HttpMethods.Put ||
                context.Request.Method == HttpMethods.Patch)
            {
                context.Request.EnableBuffering();
                var body = await new StreamReader(context.Request.Body).ReadToEndAsync();
                context.Request.Body.Position = 0;

                foreach (var pattern in _dangerousPatterns)
                {
                    if (Regex.IsMatch(body, pattern, RegexOptions.IgnoreCase))
                    {
                        context.Response.StatusCode = StatusCodes.Status400BadRequest;
                        await context.Response.WriteAsync("Potential XSS attack detected");
                        return;
                    }
                }
            }

            await _next(context);
        }
    }
}
