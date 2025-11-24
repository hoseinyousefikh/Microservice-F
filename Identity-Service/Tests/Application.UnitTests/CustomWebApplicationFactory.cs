//// --- using های ضروری ---
//using Microsoft.AspNetCore.Mvc.Testing; // برای WebApplicationFactory
//using Microsoft.AspNetCore.Hosting;      // برای IWebHostBuilder
//using Microsoft.EntityFrameworkCore;    // برای DbContextOptions و UseInMemoryDatabase
//using Microsoft.Extensions.DependencyInjection; // برای IServiceCollection
//using System.Linq;                   // برای استفاده از SingleOrDefault

//// --- using های مربوط به پروژه شما ---
//using Identity_Service.Infrastructure.Persistence; // برای ApplicationDbContext

//// --- Namespace تست ---
//// توجه: این namespace باید با محل قرارگیری فایل شما هماهنگ باشد.
//// اگر فایل در پوشه Integration.Tests است، namespace را هم تغییر دهید.
//namespace Identity_Service.Tests.Application.UnitTests
//{
//    // کلاس کمکی برای راه‌اندازی اپلیکیشن در تست‌های یکپارچه‌گی
//    public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
//    {
//        protected override void ConfigureWebHost(IWebHostBuilder builder)
//        {
//            builder.ConfigureServices(services =>
//            {
//                // حذف سرویس دیتابیس واقعی
//                var descriptor = services.SingleOrDefault(
//                    d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

//                if (descriptor != null)
//                {
//                    services.Remove(descriptor);
//                }

//                // جایگزینی با دیتابیس در حافظه
//                services.AddDbContext<ApplicationDbContext>(options =>
//                {
//                    options.UseInMemoryDatabase("InMemoryDbForTesting");
//                });

//                // ساختن سرویس‌ها برای اعمال تغییرات
//                var sp = services.BuildServiceProvider();
//                using (var scope = sp.CreateScope())
//                {
//                    var scopedServices = scope.ServiceProvider;
//                    var db = scopedServices.GetRequiredService<ApplicationDbContext>();
//                    db.Database.EnsureCreated(); // اطمینان از ساخته شدن دیتابیس در حافظه
//                }
//            });
//        }
//    }
//}