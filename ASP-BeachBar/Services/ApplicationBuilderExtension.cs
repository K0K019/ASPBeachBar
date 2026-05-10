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
                await SeedBeachBarDataAsync(context);
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

        public static async Task SeedBeachBarDataAsync(ApplicationDbContext context)
        {
            var categoryNames = new[] { "Beach Bites", "Seafood", "Fresh Drinks", "Cocktails" };
            foreach (var categoryName in categoryNames)
            {
                if (!await context.Categories.AnyAsync(c => c.Name == categoryName))
                {
                    context.Categories.Add(new Category { Name = categoryName });
                }
            }

            await context.SaveChangesAsync();

            var categories = await context.Categories
                .Where(c => categoryNames.Contains(c.Name))
                .ToDictionaryAsync(c => c.Name);

            var now = DateTime.Now;
            var today = DateTime.Today;

            await AddProductIfMissingAsync(new Product
            {
                Name = "Fish Tacos",
                CategoryId = categories["Seafood"].Id,
                ImageUrl = "https://images.unsplash.com/photo-1551504734-5ee1c4a1479b?auto=format&fit=crop&w=900&q=80",
                Description = "Три меки такоса с бяла риба, лайм, манго салца и свеж кориандър.",
                Weight = 320,
                Price = 15.90,
                RegisterOn = now
            });

            await AddProductIfMissingAsync(new Product
            {
                Name = "Crispy Calamari",
                CategoryId = categories["Seafood"].Id,
                ImageUrl = "https://images.unsplash.com/photo-1599487488170-d11ec9c172f0?auto=format&fit=crop&w=900&q=80",
                Description = "Хрупкави калмари с лимонов айоли и морска сол.",
                Weight = 260,
                Price = 18.50,
                RegisterOn = now
            });

            await AddProductIfMissingAsync(new Product
            {
                Name = "Sunset Bruschetta",
                CategoryId = categories["Beach Bites"].Id,
                ImageUrl = "https://images.unsplash.com/photo-1572695157366-5e585ab2b69f?auto=format&fit=crop&w=900&q=80",
                Description = "Препечен хляб, домати, босилек, маслини и сирене.",
                Weight = 240,
                Price = 10.90,
                RegisterOn = now
            });

            await AddProductIfMissingAsync(new Product
            {
                Name = "Tropical Fruit Bowl",
                CategoryId = categories["Beach Bites"].Id,
                ImageUrl = "https://images.unsplash.com/photo-1490474418585-ba9bad8fd0ea?auto=format&fit=crop&w=900&q=80",
                Description = "Ананас, диня, ягоди, мента и кокосови стърготини.",
                Weight = 300,
                Price = 9.80,
                RegisterOn = now
            });

            await AddDrinkIfMissingAsync(new Drink
            {
                Name = "Passion Fruit Mojito",
                ImageUrl = "https://images.unsplash.com/photo-1551782450-a2132b4ba21d?auto=format&fit=crop&w=900&q=80",
                IsAlcoholic = true,
                CategoryId = categories["Cocktails"].Id,
                Weight = 330,
                Price = 12.50,
                RegisterOn = now
            });

            await AddDrinkIfMissingAsync(new Drink
            {
                Name = "Watermelon Lemonade",
                ImageUrl = "https://images.unsplash.com/photo-1621263764928-df1444c5e859?auto=format&fit=crop&w=900&q=80",
                IsAlcoholic = false,
                CategoryId = categories["Fresh Drinks"].Id,
                Weight = 450,
                Price = 7.50,
                RegisterOn = now
            });

            await AddDrinkIfMissingAsync(new Drink
            {
                Name = "Aperol Spritz",
                ImageUrl = "https://images.unsplash.com/photo-1560512823-829485b8bf24?auto=format&fit=crop&w=900&q=80",
                IsAlcoholic = true,
                CategoryId = categories["Cocktails"].Id,
                Weight = 300,
                Price = 11.90,
                RegisterOn = now
            });

            await AddEventIfMissingAsync(new Event
            {
                Name = "Sunset DJ Session",
                ImageUrl = "https://images.unsplash.com/photo-1492684223066-81342ee5ff30?auto=format&fit=crop&w=900&q=80",
                Description = "Deep house сет на залез с happy hour коктейли.",
                DateReservation = today.AddDays(14).AddHours(19).AddMinutes(30),
                RegisterOn = now
            });

            await AddEventIfMissingAsync(new Event
            {
                Name = "Acoustic Beach Night",
                ImageUrl = "https://images.unsplash.com/photo-1516280440614-37939bbacd81?auto=format&fit=crop&w=900&q=80",
                Description = "Акустична вечер с китара, бяло вино и морски тапас.",
                DateReservation = today.AddDays(21).AddHours(20),
                RegisterOn = now
            });

            await AddEventIfMissingAsync(new Event
            {
                Name = "Tropical Brunch",
                ImageUrl = "https://images.unsplash.com/photo-1504674900247-0877df9cc836?auto=format&fit=crop&w=900&q=80",
                Description = "Неделен брънч с плодови купи, скариди и безалкохолни коктейли.",
                DateReservation = today.AddDays(28).AddHours(11),
                RegisterOn = now
            });

            await context.SaveChangesAsync();

            async Task AddProductIfMissingAsync(Product product)
            {
                if (!await context.Products.AnyAsync(p => p.Name == product.Name))
                {
                    context.Products.Add(product);
                }
            }

            async Task AddDrinkIfMissingAsync(Drink drink)
            {
                if (!await context.Drinks.AnyAsync(d => d.Name == drink.Name))
                {
                    context.Drinks.Add(drink);
                }
            }

            async Task AddEventIfMissingAsync(Event beachEvent)
            {
                if (!await context.Events.AnyAsync(e => e.Name == beachEvent.Name))
                {
                    context.Events.Add(beachEvent);
                }
            }
        }
    }
}
