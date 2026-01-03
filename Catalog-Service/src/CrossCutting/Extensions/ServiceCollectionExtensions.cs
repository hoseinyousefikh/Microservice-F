// File: ServiceCollectionExtensions.cs
using Catalog_Service.src._01_Domain.Core.Contracts.Repositories;
using Catalog_Service.src._01_Domain.Core.Contracts.Services;
using Catalog_Service.src._01_Domain.Services;
using Catalog_Service.src._02_Infrastructure.Caching;
using Catalog_Service.src._02_Infrastructure.Data.Db;
using Catalog_Service.src._02_Infrastructure.Data.Repositories;
using Catalog_Service.src._02_Infrastructure.ExternalServices;
using Catalog_Service.src._02_Infrastructure.FileStorage;
using Catalog_Service.src._02_Infrastructure.Security;
using Catalog_Service.src._03_Endpoints.Mappers;
using Catalog_Service.src.CrossCutting.Security;
using Catalog_Service.src.CrossCutting.Validation;
using Catalog_Service.src.CrossCutting.Validation.Admin;
using FluentValidation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System.Security.Claims;
using System.Security.Principal;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace Catalog_Service.src.CrossCutting.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDomainServices(this IServiceCollection services)
        {
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IBrandService, BrandService>();
            services.AddScoped<IImageService, ImageService>();
            services.AddScoped<IProductVariantService, ProductVariantService>();
            services.AddScoped<IProductAttributeService, ProductAttributeService>();
            services.AddScoped<IProductReviewService, ProductReviewService>();
            services.AddScoped<IProductTagService, ProductTagService>();
            services.AddScoped<ISlugService, SlugService>();

            services.AddCustomValidation();

            return services;
        }

        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")),
                ServiceLifetime.Scoped);

            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IBrandRepository, BrandRepository>();
            services.AddScoped<IImageRepository, ImageRepository>();
            services.AddScoped<IProductVariantRepository, ProductVariantRepository>();
            services.AddScoped<IProductAttributeRepository, ProductAttributeRepository>();
            services.AddScoped<IProductReviewRepository, ProductReviewRepository>();
            services.AddScoped<IProductTagRepository, ProductTagRepository>();

            services.AddSingleton<IConnectionMultiplexer>(sp =>
                ConnectionMultiplexer.Connect(configuration.GetConnectionString("Redis")));
            services.AddScoped<ICacheService, RedisCacheService>();

            services.Configure<FileStorageOptions>(configuration.GetSection("FileStorage"));
            services.AddScoped<IFileStorage, LocalFileStorage>();

            if (configuration.GetValue<bool>("FileStorage:UseS3"))
            {
                services.AddScoped<IFileStorage, S3FileStorage>();
            }
            else if (configuration.GetValue<bool>("FileStorage:UseAzureBlob"))
            {
                services.AddScoped<IFileStorage, AzureBlobStorage>();
            }

            services.AddHttpClient<IInventoryServiceClient, InventoryServiceClient>();
            services.AddHttpClient<IPricingServiceClient, PricingServiceClient>();

            services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
            services.AddScoped<IKeyManagementService, KeyManagementService>();
            services.AddScoped<ApiKeyValidator>();

            // بخش AddAuthentication از اینجا حذف شد

            services.AddAuthorization(options =>
            {
                AuthorizationPolicies.Configure(options);
            });

            services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile<AdminMappingProfile>();
                cfg.AddProfile<VendorMappingProfile>();
                cfg.AddProfile<PublicMappingProfile>();
            });

            // این متد را می‌توان چندین بار فراخوانی کرد، اما Health Check ها فقط یک بار اضافه خواهند شد
            services.AddCatalogHealthChecks(configuration);

            return services;
        }

        // *** این متد جدید برای پیکربندی یکپارچه احراز هویت اضافه شده است ***
        public static IServiceCollection AddAppAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer("Bearer", options =>
            {
                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["JwtSettings:Issuer"],
                    ValidAudience = configuration["JwtSettings:Audience"],
                    IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
                        System.Text.Encoding.UTF8.GetBytes(configuration["JwtSettings:SecretKey"])),

                    // *** این دو خط کلیدی هستند ***
                    // این دو خط به Catalog-Service میگویند که از نام‌های استاندارد مایکروسافت استفاده کند
                    RoleClaimType = ClaimTypes.Role,
                    NameClaimType = ClaimTypes.Name
                };
            })
            .AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>("ApiKey", options => { });

            return services;
        }
        // --- کد اصلاح شده با نام منحصر به فرد ---
        public static IServiceCollection AddCatalogHealthChecks(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHealthChecks()
                // نام "catalog-db" را برای SQL Server مشخص می‌کنیم
                .AddSqlServer(configuration.GetConnectionString("DefaultConnection"), name: "catalog-db")
                // نام "catalog-redis" را برای Redis مشخص می‌کنیم تا از تکراری جلوگیری شود
                .AddRedis(configuration.GetConnectionString("Redis"), name: "catalog-redis")
                .AddCheck<ExternalServicesHealthCheck>("External Services");

            return services;
        }

        public static IServiceCollection AddCustomValidation(this IServiceCollection services)
        {
            services.AddValidatorsFromAssemblyContaining<CreateCategoryValidator>();
            services.AddTransient<IValidatorFactory, CustomValidatorFactory>();
            services.AddMvc(options =>
            {
                options.Filters.Add<CustomValidationFilter>();
                options.Filters.Add<CustomClientValidationDataFilter>();
            });

            return services;
        }
    }

    public class ExternalServicesHealthCheck : IHealthCheck
    {
        private readonly IInventoryServiceClient _inventoryServiceClient;
        private readonly IPricingServiceClient _pricingServiceClient;

        public ExternalServicesHealthCheck(
            IInventoryServiceClient inventoryServiceClient,
            IPricingServiceClient pricingServiceClient)
        {
            _inventoryServiceClient = inventoryServiceClient;
            _pricingServiceClient = pricingServiceClient;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                await _inventoryServiceClient.GetInventoryStatusAsync(1, cancellationToken);
                await _pricingServiceClient.GetProductPriceAsync(1, cancellationToken);
                return HealthCheckResult.Healthy("External services are healthy");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy("External services are unhealthy", ex);
            }
        }
    }

    public class ApiKeyAuthenticationOptions : AuthenticationSchemeOptions
    {
        public const string DefaultScheme = "ApiKey";
        public string Scheme => DefaultScheme;
        public string ApiKeyHeaderName { get; set; } = "X-API-KEY";
    }

    public class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyAuthenticationOptions>
    {
        private readonly ApiKeyValidator _apiKeyValidator;

        public ApiKeyAuthenticationHandler(
            IOptionsMonitor<ApiKeyAuthenticationOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            ApiKeyValidator apiKeyValidator) : base(options, logger, encoder, clock)
        {
            _apiKeyValidator = apiKeyValidator;
        }

        private static System.Security.Claims.Claim CreateClaim(string type, string value)
        {
            return new System.Security.Claims.Claim(type, value);
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.TryGetValue(Options.ApiKeyHeaderName, out var apiKeyValues))
            {
                return AuthenticateResult.NoResult();
            }

            var apiKey = apiKeyValues.FirstOrDefault();
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                return AuthenticateResult.NoResult();
            }

            var validationResult = await _apiKeyValidator.ValidateAsync(apiKey);
            if (!validationResult.IsValid)
            {
                return AuthenticateResult.Fail("Invalid API Key");
            }

            var claims = new[]
            {
                CreateClaim(System.Security.Claims.ClaimTypes.Name, validationResult.ServiceName),
                CreateClaim(System.Security.Claims.ClaimTypes.Role, "Service")
            };

            var identity = new System.Security.Claims.ClaimsIdentity(claims, Scheme.Name);
            var principal = new System.Security.Claims.ClaimsPrincipal(identity);

            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return AuthenticateResult.Success(ticket);
        }
    }
}