// --- using های اصلی ---
using System.Reflection;
using System.Text;
using FluentValidation;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SharedKernel.Application.Abstractions.Services;

// --- using های مربوط به لایه‌های پروژه ---
using Identity_Service.Application.Common;
using Identity_Service.Application.Common.Services;
using Identity_Service.Application.Interfaces;
using Identity_Service.Infrastructure.ExternalServices;
using Identity_Service.Infrastructure.Persistence.Context;
using Identity_Service.Infrastructure.Persistence.Context.Abstractions;
using Identity_Service.Infrastructure.Services;

// --- Aliases برای جلوگیری از تداخل namespace ---
using AppDbContext = Identity_Service.Infrastructure.Persistence.Context.ApplicationDbContext;
using IAppDbContext = Identity_Service.Infrastructure.Persistence.Context.Abstractions.IApplicationDbContext;
using InfraRole = Identity_Service.Infrastructure.Persistence.Entities.Role;
using InfraUser = Identity_Service.Infrastructure.Persistence.Entities.User;
using IUserRepo = Identity_Service.Infrastructure.Persistence.Repositories.Abstractions.IUserRepository;

var builder = WebApplication.CreateBuilder(args);

// --- 1. افزودن DbContext ---
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

// --- 2. پیکربندی ASP.NET Core Identity ---
builder.Services.AddIdentity<InfraUser, InfraRole>(options =>
{
    // تنظیمات رمز عبور
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;
    // تنظیمات کاربر
    options.User.RequireUniqueEmail = true;
    // تنظیمات قفل شدن حساب
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders(); // ارائه‌دهنده توکن‌های برای ریست پسورد و ایمیل و...

// --- 3. پیکربندی احراز هویت (JWT) ---
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

// --- 4. پیکربندی MediatR, CQRS & Validation ---
// MediatR را از اسمبلی حاوی هندلرها (معمولاً Application) بارگذاری کن
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
// FluentValidation را از اسمبلی حاوی Validator ها بارگذاری کن
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);
builder.Services.AddFluentValidationAutoValidation();

// --- 5. پیکربندی AutoMapper ---
// AutoMapper را با معرفی اسمبلی‌های حاوی Profile ها ثبت کن
// این روش بهتر از Assembly.GetExecutingAssembly() است چون پروژه‌های دیگر را هم شامل می‌شود
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// --- 6. ثبت سرویس‌های اختصاصی (Dependency Injection) ---
// سرویس‌های Application
builder.Services.AddScoped<IIdentityService, IdentityService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();

// سرویس‌های Infrastructure
builder.Services.AddScoped<IAppDbContext, AppDbContext>();
builder.Services.AddScoped<IUserContextService, UserContextService>();
builder.Services.AddHttpContextAccessor(); // برای IUserContextService ضروری است

// --- 7. پیکربندی سرویس ایمیل ---
builder.Services.Configure<Identity_Service.Infrastructure.ExternalServices.EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddTransient<SendGridClient>();
builder.Services.AddTransient<IEmailService, EmailService>();

// --- 8. افزودن API Controllers & Swagger ---
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "MarketHub Identity Service", Version = "v1" });

    // تعریف Security Scheme برای JWT
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter JWT with Bearer into field",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    // اعمال Security Requirement به تمام عملیات
    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
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
            new string[] {}
        }
    });
});

var app = builder.Build();

// --- کانفیگ میدل‌ویر (Request Pipeline) ---
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// ترتیب مهم است: حتماً قبل از UseAuthorization باشد
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// --- اعمال Migration و داده‌های اولیه (Seeding) ---
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<AppDbContext>();
    var logger = services.GetRequiredService<ILogger<Program>>();

    try
    {
        // ایجاد دیتابیس و اعمال Migrationها در صورت وجود نداشتن
        context.Database.Migrate();
        logger.LogInformation("Database migrated successfully.");

        // TODO: کلاس SeedData را پیاده‌سازی کنید
        // مثال برای فراخوانی:
        // var userManager = services.GetRequiredService<UserManager<InfraUser>>();
        // var roleManager = services.GetRequiredService<RoleManager<InfraRole>>();
        // await SeedData.Initialize(services, userManager, roleManager);
        // logger.LogInformation("Database seeded successfully.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred during migration or seeding.");
    }
}

app.Run();

// این خط برای Top-level statements در دات نت 6 و بالاتر ضروری است.
public partial class Program { }