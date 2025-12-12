using Catalog_Service.src._02_Infrastructure.Data.Db;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// 1. پیکربندی Serilog برای لاگینگ پیشرفته
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog();

// 2. افزودن سرویس‌های اصلی ASP.NET Core
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// 3. پیکربندی Swagger/OpenAPI با احراز هویت JWT
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "CatalogService API",
        Version = "v1",
        Description = "API برای مدیریت کاتالوگ محصولات در سیستم فروشگاهی",
        Contact = new OpenApiContact
        {
            Name = "تیم توسعه",
            Email = "dev@example.com"
        }
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

    // افزودن فایل XML برای مستندات
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});

// 4. پیکربندی پایگاه داده
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 3,
                maxRetryDelay: TimeSpan.FromSeconds(5),
                errorNumbersToAdd: null);
        });
});

//// 5. افزودن سرویس‌های دامنه و زیرساخت
//builder.Services.AddDomainServices();
//builder.Services.AddInfrastructureServices(builder.Configuration);

// 6. پیکربندی احراز هویت JWT
builder.Services.AddAuthentication(options =>
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
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
            System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]))
    };
});

// 7. پیکربندی احراز هویت سرویس‌ها (API Key)
//builder.Services.AddServiceAuthentication();

// 8. پیکربندی Authorization با سیاست‌های سفارشی
//builder.Services.AddAuthorization(options =>
//{
//    options.AddPolicy("AdminOnly", policy =>
//        policy.RequireRole("Admin", "SuperAdmin"));

//    options.AddPolicy("VendorOnly", policy =>
//        policy.RequireRole("Vendor"));

//    options.AddPolicy("ProductOwner", policy =>
//        policy.Requirements.Add(new CatalogService.CrossCutting.Security.PolicyRequirements.ProductOwnerRequirement()));
//});

//// 9. افزودن سرویس‌های اعتبارسنجی (FluentValidation)
//builder.Services.AddFluentValidationAutoValidation();
//builder.Services.AddFluentValidationClientsideAdapters();
//builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// 10. پیکربندی CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// 11. افزودن Health Checks
builder.Services.AddHealthChecks()
    .AddDbContextCheck<AppDbContext>()
    .AddRedis(builder.Configuration["Redis:ConnectionString"])
    .AddElasticsearch(builder.Configuration["Elasticsearch:Url"]);

// 12. افزودن Response Caching
builder.Services.AddResponseCaching();

// 13. افزودن Http Client برای سرویس‌های خارجی
builder.Services.AddHttpClient("InventoryService", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ExternalServices:InventoryService:BaseUrl"]);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

builder.Services.AddHttpClient("PricingService", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ExternalServices:PricingService:BaseUrl"]);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

var app = builder.Build();

// 14. پیکربندی Middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "CatalogService API v1");
        options.RoutePrefix = string.Empty;
    });
}

// 15. افزودن Middleware به ترتیب صحیح
app.UseHttpsRedirection();

// Middleware برای مدیریت خطاها
//app.UseMiddleware<CatalogService.CrossCutting.Middleware.ErrorHandlingMiddleware>();

// Middleware برای Correlation ID
//app.UseMiddleware<CatalogService.CrossCutting.Middleware.CorrelationIdMiddleware>();

// Middleware برای لاگینگ درخواست‌ها
//app.UseMiddleware<CatalogService.CrossCutting.Middleware.RequestLoggingMiddleware>();

// Middleware برای محدودیت نرخ درخواست
//app.UseMiddleware<CatalogService.CrossCutting.Middleware.RateLimitingMiddleware>();

// Middleware برای معیارهای عملکرد
//app.UseMiddleware<CatalogService.CrossCutting.Middleware.PerformanceMetricsMiddleware>();

app.UseCors("AllowAll");

app.UseResponseCaching();

app.UseAuthentication();
app.UseAuthorization();

// 16. افزودن Health Checks endpoint
app.MapHealthChecks("/health");

// 17. افزودن Endpoints
app.MapControllers();

// 18. اجرای برنامه
try
{
    Log.Information("Starting CatalogService");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "CatalogService terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}