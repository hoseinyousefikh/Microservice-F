using IdentityService._01_Domain.Core.Entities;
using IdentityService._01_Domain.Core.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace IdentityService._02_Infrastructures.Data
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(AppDbContext context, IServiceProvider services)
        {
            var loggerFactory = services.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger(typeof(DbInitializer).FullName);
            var roleManager = services.GetRequiredService<RoleManager<ApplicationRole>>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

            logger.LogInformation("Database initialization started");

            await context.Database.EnsureCreatedAsync();

            await SeedRolesAsync(roleManager, logger);
            await SeedSuperAdminAsync(userManager, logger);

            logger.LogInformation("Database initialization completed");
        }

        private static async Task SeedRolesAsync(RoleManager<ApplicationRole> roleManager, ILogger logger)
        {
            foreach (var role in Enum.GetValues<UserRole>())
            {
                var roleName = role.ToString();
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    var result = await roleManager.CreateAsync(new ApplicationRole
                    {
                        Name = roleName,
                        Description = $"{role} role"
                    });

                    if (result.Succeeded)
                    {
                        logger.LogInformation($"Role '{roleName}' created successfully");
                    }
                    else
                    {
                        logger.LogError($"Error creating role '{roleName}': {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    }
                }
            }
        }

        private static async Task SeedSuperAdminAsync(UserManager<ApplicationUser> userManager, ILogger logger)
        {
            const string adminEmail = "admin@identityservice.com";
            const string adminPassword = "P@ssw0rd!Admin";

            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FirstName = "Super",
                    LastName = "Admin",
                    EmailConfirmed = true,
                    Status = AccountStatus.Active,
                    CreatedAt = DateTime.UtcNow
                };

                var result = await userManager.CreateAsync(adminUser, adminPassword);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, UserRole.SuperAdmin.ToString());
                    logger.LogInformation("Super admin user created successfully");
                }
                else
                {
                    logger.LogError($"Error creating super admin: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }
        }
    }
}