using Microsoft.AspNetCore.Diagnostics;
using Catalog_Service.src._02_Infrastructure.Data.Db;
using Catalog_Service.src.CrossCutting.Extensions;
using Catalog_Service.src.CrossCutting.Security.PolicyRequirements;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Reflection;
using Catalog_Service.src.CrossCutting.Exceptions;

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

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});

// 4. *** ابتدا احراز هویت را پیکربندی کنید ***
builder.Services.AddAppAuthentication(builder.Configuration);

// 5. *** سپس سرویس‌های زیرساخت را اضافه کنید (شامل AddAuthorization) ***
builder.Services.AddDomainServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

// 6. پیکربندی CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// 7. افزودن Response Caching
builder.Services.AddResponseCaching();

// 8. افزودن Http Client برای سرویس‌های خارجی
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

// 9. پیکربندی Middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "CatalogService API v1");
        options.RoutePrefix = string.Empty;
    });
}

// 10. افزودن Middleware به ترتیب صحیح
app.UseHttpsRedirection();

// *** این بخش بسیار مهم است. از متد الحاقی سفارشی خود استفاده کنید ***
app.UseCatalogMiddleware(builder.Configuration);
// *** پایان بخش مهم ***

app.UseCors("AllowAll");
app.UseResponseCaching();

app.UseAuthentication();
app.UseAuthorization();

// 11. افزودن Health Checks endpoint
app.MapHealthChecks("/health");

// 12. افزودن Endpoints
app.MapControllers();

// 13. اجرای برنامه
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