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
using Catalog_Service.src.CrossCutting.Validation.Admin;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using StackExchange.Redis;
using Microsoft.EntityFrameworkCore.SqlServer;
namespace Catalog_Service.src.CrossCutting.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCatalogServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Register DbContext
            // Register DbContext
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")),
                ServiceLifetime.Scoped);

            // Register Repositories
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IBrandRepository, BrandRepository>();
            services.AddScoped<IImageRepository, ImageRepository>();
            services.AddScoped<IProductVariantRepository, ProductVariantRepository>();
            services.AddScoped<IProductAttributeRepository, ProductAttributeRepository>();
            services.AddScoped<IProductReviewRepository, ProductReviewRepository>();
            services.AddScoped<IProductTagRepository, ProductTagRepository>();

            // Register Services
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IBrandService, BrandService>();
            services.AddScoped<IImageService, ImageService>();
            services.AddScoped<IProductVariantService, ProductVariantService>();
            services.AddScoped<IProductAttributeService, ProductAttributeService>();
            services.AddScoped<IProductReviewService, ProductReviewService>();
            services.AddScoped<IProductTagService, ProductTagService>();
            services.AddScoped<ISlugService, SlugService>();

            // Register Caching
            services.AddSingleton<IConnectionMultiplexer>(sp =>
                ConnectionMultiplexer.Connect(configuration.GetConnectionString("Redis")));
            services.AddScoped<ICacheService, RedisCacheService>();

            // Register File Storage
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

            // Register External Services
            services.AddHttpClient<IInventoryServiceClient, InventoryServiceClient>();
            services.AddHttpClient<IPricingServiceClient, PricingServiceClient>();

            // Register Security Services
            services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
            services.AddScoped<IKeyManagementService, KeyManagementService>();
            services.AddScoped<ApiKeyValidator>();

            // Register Authentication
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
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
                        System.Text.Encoding.UTF8.GetBytes(configuration["JwtSettings:SecretKey"]))
                };
            });

            // Register Authorization
            services.AddAuthorization(options =>
            {
                AuthorizationPolicies.Configure(options);
            });

            // Register Validators
            services.AddValidatorsFromAssemblyContaining<CreateCategoryValidator>();

            // Register AutoMapper
            services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile<AdminMappingProfile>();
                cfg.AddProfile<VendorMappingProfile>();
                cfg.AddProfile<PublicMappingProfile>();
            });

            return services;
        }

        public static IServiceCollection AddCatalogHealthChecks(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHealthChecks()
                .AddSqlServer(configuration.GetConnectionString("DefaultConnection"))
                .AddRedis(configuration.GetConnectionString("Redis"))
                .AddCheck<ExternalServicesHealthCheck>("External Services");

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
                // Check Inventory Service
                var inventoryStatus = await _inventoryServiceClient.GetInventoryStatusAsync(1, cancellationToken);

                // Check Pricing Service
                var pricingStatus = await _pricingServiceClient.GetProductPriceAsync(1, cancellationToken);

                return HealthCheckResult.Healthy("External services are healthy");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy("External services are unhealthy", ex);
            }
        }
    }
}
