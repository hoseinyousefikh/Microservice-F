//using Identity_Service.Infrastructure.Persistence.Entities;
//using Microsoft.AspNetCore.Identity;

//namespace Identity_Service
//{
//    public static class SeedData
//    {
//        public static async Task Initialize(IServiceProvider serviceProvider)
//        {
//            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
//            var roleManager = serviceProvider.GetRequiredService<RoleManager<Role>>();

//            // 1. ایجاد نقش‌ها اگر وجود نداشته باشند
//            string[] roleNames = { "Admin", "Customer", "Seller" };
//            foreach (var roleName in roleNames)
//            {
//                if (!await roleManager.RoleExistsAsync(roleName))
//                {
//                    await roleManager.CreateAsync(new Role { Name = roleName });
//                }
//            }

//            // 2. ایجاد کاربر ادمین اگر وجود نداشته باشد
//            var adminEmail = "admin@markethub.com";
//            var adminPassword = "Admin@123"; // از یک رمز قوی‌تر در محیط پروداکشن استفاده کنید

//            var adminUser = await userManager.FindByEmailAsync(adminEmail);
//            if (adminUser == null)
//            {
//                adminUser = new User
//                {
//                    UserName = adminEmail,
//                    Email = adminEmail,
//                    EmailConfirmed = true, // مهم است تا نیاز به تایید ایمیل نباشد
//                    FirstName = "Admin",
//                    LastName = "User"
//                };

//                var result = await userManager.CreateAsync(adminUser, adminPassword);
//                if (result.Succeeded)
//                {
//                    // 3. اضافه کردن کاربر به نقش Admin
//                    await userManager.AddToRoleAsync(adminUser, "Admin");
//                }
//            }
//        }
//    }
//}
