using API_Gateway.Authentication;
using API_Gateway.Configuration;
using API_Gateway.Extensions;
using API_Gateway.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// افزودن تنظیمات
builder.Services.Configure<AppSettings>(builder.Configuration);
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

// افزودن احراز هویت JWT
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
var key = Encoding.ASCII.GetBytes(jwtSettings.SecretKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer("Bearer", options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidateAudience = true,
        ValidAudience = jwtSettings.Audience,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

// افزودن سرویس‌های Ocelot
builder.Services.AddOcelot();

// افزودن سرویس‌های سفارشی
builder.Services.AddScoped<ITokenValidationService, TokenValidationService>();

var app = builder.Build();

// افزودن Middlewareها به ترتیب صحیح
app.UseMiddleware<GlobalExceptionHandler>();
app.UseMiddleware<RequestResponseLoggingMiddleware>();
app.UseMiddleware<JwtAuthenticationMiddleware>();

// افزودن احراز هویت و مجوزدهی استاندارد ASP.NET Core
app.UseAuthentication();
app.UseAuthorization();

// افزودن Ocelot به عنوان آخرین Middleware در زنجیره
await app.UseOcelot();

app.Run();