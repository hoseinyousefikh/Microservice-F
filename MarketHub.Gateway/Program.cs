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
    // کلید امضای توکن را از سرویس هویت کپی کرده‌ایم
    var key = Encoding.UTF8.GetBytes("ThisIsAVeryLongAndSecureSecretKeyForJWTTokenGenerationThatIsAtLeast64BytesLong!");

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        // این مقادیر نیز با فایل appsettings.json مطابقت دارند
        ValidIssuer = "MarketHub.IdentityService",
        ValidAudience = "MarketHub.Apps",
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

// --- شروع بخش جدید: افزودن CORS ---
// 2. افزودن سرویس CORS
// یک نام برای سیاست CORS خود انتخاب می‌کنیم (مثلاً "VueAppCorsPolicy")
// origins باید دقیقاً آدرسی باشد که برنامه Vue شما روی آن اجرا می‌شود.
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "VueAppCorsPolicy",
                      policy =>
                      {
                          // آدرس فرانت‌اند Vue.js شما را اینجا قرار دهید
                          // معمولاً http://localhost:5173 است اما می‌تواند متفاوت باشد.
                          // بهتر است هر دو http و https را برای اطمینان اضافه کنید.
                          policy.WithOrigins("http://localhost:5173",
                                            "https://localhost:5173")
                                .AllowAnyHeader()
                                .AllowAnyMethod()
                                .AllowCredentials(); // AllowCredentials برای ارسال کوکی یا توکن در هدرها مهم است
                      });
});
// --- پایان بخش جدید ---


// 3. ثبت سرویس‌های اصلی برنامه
builder.Services.AddControllers();
builder.Services.AddHttpClient();

// 4. ثبت و پیکربندی YARP
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration);


var app = builder.Build();

// 5. ترتیب صحیح میدل‌ورها (بسیار مهم)
app.UseHttpsRedirection();

// --- شروع بخش جدید: فعال‌سازی CORS ---
// این خط باید قبل از UseAuthentication و UseAuthorization قرار بگیرد
app.UseCors("VueAppCorsPolicy");
// --- پایان بخش جدید ---

app.UseAuthentication();
app.UseAuthorization();

app.MapReverseProxy();
app.MapControllers();

app.Run();