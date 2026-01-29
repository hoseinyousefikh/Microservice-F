using MarketHub.Gateway.Controllers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// 1. افزودن و پیکربندی احراز هویت JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    var key = Encoding.UTF8.GetBytes("ThisIsAVeryLongAndSecureSecretKeyForJWTTokenGenerationThatIsAtLeast64BytesLong!");
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = "MarketHub.IdentityService",
        ValidAudience = "MarketHub.Apps",
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

// 2. 🔥 اصلاح CORS برای فرانت‌اند React 🔥
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "ReactAppCorsPolicy",
                      policy =>
                      {
                          // تمام پورت‌های رایج توسعه فرانت‌اند را اضافه کنید
                          policy.WithOrigins(
                              "http://localhost:3000",    // Create React App پیش‌فرض
                              "https://localhost:3000",
                              "http://localhost:5173",    // Vite پیش‌فرض
                              "https://localhost:5173",
                              "http://localhost:5174",
                              "https://localhost:5174",
                              "http://localhost:8000",    // Python HTTP Server
                              "https://localhost:8000",
                              "http://127.0.0.1:5500",    // Live Server VS Code
                              "http://127.0.0.1:5500/product.html",
                              "http://localhost:5500",
                              "https://localhost:5500",
                              // --- آدرس جدید شما اضافه شد ---
                              "http://127.0.0.1:5501",    // آدرس فعلی فرانت‌اند شما
                              "http://127.0.0.1:5501/product.html" // آدرس کامل فایل
                          )
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials(); // اگر از کوکی‌ها یا احراز هویت استفاده می‌کنید
                      });

    // 🔥 گزینه دوم: برای توسعه، همه را اجازه بده (امن نیست برای production)
    // اگر پالیسی بالا جواب نداد، می‌توانید به طور موقت از این استفاده کنید
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

// 3. ثبت سرویس‌های اصلی
builder.Services.AddControllers();
builder.Services.AddHttpClient();

// 4. ثبت و پیکربندی YARP
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration);


var app = builder.Build();

// 5. ترتیب میدل‌ورها
app.UseHttpsRedirection();

// 🔥 از این خط استفاده کنید (یکی از دو گزینه زیر):
app.UseCors("ReactAppCorsPolicy");  // گزینه اول: محدود به پورت‌های مشخص
// یا
// app.UseCors("AllowAll");         // گزینه دوم: همه را اجازه بده (برای تست راحت‌تر)

app.UseAuthentication();
app.UseAuthorization();

app.MapReverseProxy();
app.MapControllers();

app.Run();