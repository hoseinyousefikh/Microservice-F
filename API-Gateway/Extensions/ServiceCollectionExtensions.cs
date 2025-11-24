using API_Gateway.Authentication;

namespace API_Gateway.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddOcelotWithJwtSupport(this IServiceCollection services)
        {
            services.AddScoped<ITokenValidationService, TokenValidationService>();
            return services;
        }
    }
}
