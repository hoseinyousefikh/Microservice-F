using MarketHub.Gateway.Controllers;
using Yarp.ReverseProxy.Transforms.Builder;
using Yarp.ReverseProxy.Transforms;

var builder = WebApplication.CreateBuilder(args);

// ✅ 1. افزودن سرویس HttpClientFactory (برای کنترلر AuthController)
builder.Services.AddHttpClient();

// ✅ 2. افزودن سرویس‌های YARP **فقط یک بار**
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration)
    .AddTransforms(transformBuilder =>
    {
        // این transform به YARP می‌گوید که مسیرهای /api/auth/ را به کنترلرهای محلی هدایت کند
        transformBuilder.AddRequestTransform(async transformContext =>
        {
            // اگر مسیر با /api/auth/ شروع شد، آن را به کنترلر محلی هدایت کن
            if (transformContext.Path.StartsWithSegments("/api/auth"))
            {
                // مسیر را به کنترلر محلی تغییر می‌دهد
                transformContext.Path = "/api/auth";

                // همچنین باید متد و کوئری را هم نگه داریم
                var query = transformContext.HttpContext.Request.QueryString;
                if (query.HasValue)
                {
                    transformContext.Path += query.Value;
                }
            }
            // در غیر این صورت، مسیر را دست‌نخورده بگذار تا به سرویس پایین‌دستی برود
        });
    });

// ✅ 3. افزودن سرویس‌های کنترلر **فقط یک بار**
builder.Services.AddControllers();

var app = builder.Build();

// 4. افزودن Middleware یارپ
app.MapReverseProxy();

// 5. افزودن کنترلرهای محلی
app.MapControllers();

app.Run();