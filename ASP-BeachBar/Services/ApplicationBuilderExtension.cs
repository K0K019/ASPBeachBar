using ASP_BeachBar.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ASP_BeachBar.Services
{
    public static class ApplicationBuilderExtension
    {
        public static async Task<IApplicationBuilder> PrepareDataBase(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var services = scope.ServiceProvider;
            var loggerFactory = services.GetRequiredService<ILoggerFactory>();

            try
            {
                var context = services.GetRequiredService<ApplicationDbContext>();
                var userManager = services.GetRequiredService<UserManager<Client>>();
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                var configuration = services.GetRequiredService<IConfiguration>();
                var environment = services.GetRequiredService<IWebHostEnvironment>();

                await context.Database.MigrateAsync();
                await SeedRolesAsync(roleManager);
                await SeedSuperAdminAsync(userManager, configuration, environment);
            }
            catch (Exception ex)
            {
                var logger = loggerFactory.CreateLogger<Program>();
                logger.LogError(ex, "An error occurred seeding the DB.");
            }

            return app;
        }

        public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            foreach (var role in new[] { "Admin", "User", "Guest" })
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }

        public static async Task SeedSuperAdminAsync(
            UserManager<Client> userManager,
            IConfiguration configuration,
            IWebHostEnvironment environment)
        {
            var adminUserName = configuration["SeedAdmin:UserName"] ?? "superadmin";
            var adminEmail = configuration["SeedAdmin:Email"] ?? "superadmin@gmail.com";
            var adminPassword = configuration["SeedAdmin:Password"];

            var user = await userManager.FindByEmailAsync(adminEmail);
            if (user == null && string.IsNullOrWhiteSpace(adminPassword))
            {
                if (!environment.IsDevelopment())
                {
                    return;
                }

                adminPassword = "123!@#Qwe";
            }

            if (user == null)
            {
                var defaultUser = new Client
                {
                    UserName = adminUserName,
                    Email = adminEmail,
                    FirstName = "Nikola",
                    LastName = "Luchev",
                    PhoneNumber = "0899999999",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true
                };

                var result = await userManager.CreateAsync(defaultUser, adminPassword!);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(defaultUser, "Admin");
                }

                return;
            }

            if (!await userManager.IsInRoleAsync(user, "Admin"))
            {
                await userManager.AddToRoleAsync(user, "Admin");
            }
        }

    }
}
