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
                await SeedSampleDataAsync(context, userManager);
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
            foreach (var role in new[] { "Admin", "Client", "User", "Guest" })
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

        private static async Task SeedSampleDataAsync(ApplicationDbContext context, UserManager<Client> userManager)
        {
            await CleanupOldSampleDataAsync(context);
            await SeedNavigationAsync(context);

            var categoryNames = new[]
            {
                "Храна",
                "Салати",
                "Разядки",
                "Предястия",
                "Основни ястия",
                "Морски ястия",
                "Десерти",
                "Алкохолни напитки",
                "Безалкохолни напитки",
                "Алкохолни коктейли",
                "Безалкохолни коктейли"
            };

            foreach (var categoryName in categoryNames)
            {
                if (!await context.Categories.AnyAsync(c => c.Name == categoryName))
                {
                    context.Categories.Add(new Category { Name = categoryName });
                }
            }

            await context.SaveChangesAsync();

            var categories = await context.Categories.ToDictionaryAsync(c => c.Name, c => c.Id);
            var now = DateTime.Now;

            await AddProductIfMissingAsync(context, categories["Салати"], "Шопска салата с печена чушка",
                "https://commons.wikimedia.org/wiki/Special:FilePath/Shopska%20Salad.JPG?width=900",
                "Домати, краставици, печена чушка, козе сирене и магданозено олио.", 350, 10.90, now);
            await AddProductIfMissingAsync(context, categories["Салати"], "Овчарска салата Shoreline",
                "https://commons.wikimedia.org/wiki/Special:FilePath/Chef%20salad%20with%20ham%20at%20Wes%20Mans%20Restaurant,%20White%20GA.jpg?width=900",
                "Класическа овчарска салата с яйце, шунка, кашкавал и свежи зеленчуци.", 420, 12.90, now);
            await AddProductIfMissingAsync(context, categories["Салати"], "Салата с розов домат и сирене",
                "https://commons.wikimedia.org/wiki/Special:FilePath/Cherry%20tomatoes%20and%20bulgarit%20cheese%20salad%20at%20a%20tapas%20bar%20in%20Tel%20Aviv.jpg?width=900",
                "Розови домати, бяло саламурено сирене, босилек и студено пресован зехтин.", 320, 11.50, now);
            await AddProductIfMissingAsync(context, categories["Разядки"], "Катък с печени чушки и орехи",
                "https://commons.wikimedia.org/wiki/Special:FilePath/Homemade%20pepper%20spread.jpg?width=900",
                "Домашен катък с печени чушки, орехи и топла пърленка.", 260, 9.90, now);
            await AddProductIfMissingAsync(context, categories["Разядки"], "Кьопоолу с хрупкави брускети",
                "https://commons.wikimedia.org/wiki/Special:FilePath/Kyopoolu_bg.jpg?width=900",
                "Печен патладжан, чушка, чесън, магданоз и брускети със зехтин.", 280, 9.50, now);
            await AddProductIfMissingAsync(context, categories["Разядки"], "Снежанка с краставица и копър",
                "https://commons.wikimedia.org/wiki/Special:FilePath/Snezhanka%20Salad.jpg?width=900",
                "Цедено мляко, краставица, копър, чесън и печени орехи.", 240, 8.90, now);
            await AddProductIfMissingAsync(context, categories["Предястия"], "Тиквички с млечен сос",
                "https://commons.wikimedia.org/wiki/Special:FilePath/Fried%20Zucchini%20dish.jpg?width=900",
                "Хрупкави тиквички с чеснов млечен сос и пресен копър.", 300, 10.90, now);
            await AddProductIfMissingAsync(context, categories["Предястия"], "Кюфтенца от тиквички",
                "https://commons.wikimedia.org/wiki/Special:FilePath/Zucchini%20Fritters%20-%20Lunch%20at%20Yanyali%20Fehmi%20Lokantasi%20(6421047753).jpg?width=900",
                "Леки зеленчукови кюфтенца със сирене, мента и млечен дип.", 280, 11.90, now);
            await AddProductIfMissingAsync(context, categories["Предястия"], "Баница хапки със сирене",
                "https://commons.wikimedia.org/wiki/Special:FilePath/Banitsa%20and%20yogurt.jpg?width=900",
                "Мини баница с масло, сирене и поднесена с кисело мляко.", 260, 8.90, now);
            await AddProductIfMissingAsync(context, categories["Предястия"], "Пърленка с шарена сол",
                "https://commons.wikimedia.org/wiki/Special:FilePath/Flatbread.JPG?width=900",
                "Топла пърленка с масло, чесън и шарена сол.", 220, 6.90, now);
            await AddProductIfMissingAsync(context, categories["Основни ястия"], "Мини кебапчета с лютеница",
                "https://commons.wikimedia.org/wiki/Special:FilePath/Kebapcheta.JPG?width=900",
                "Кебапчета на грил с домашна лютеница, лук и пърленка.", 360, 14.90, now);
            await AddProductIfMissingAsync(context, categories["Основни ястия"], "Пилешки шишчета с чубрица",
                "https://commons.wikimedia.org/wiki/Special:FilePath/Gfp-teriyaki-chicken-skewers.jpg?width=900",
                "Мариновано пилешко филе с чубрица, печени зеленчуци и билков сос.", 380, 16.90, now);
            await AddProductIfMissingAsync(context, categories["Основни ястия"], "Кавърма в хрупкава питка",
                "https://commons.wikimedia.org/wiki/Special:FilePath/Kavurma%20(lamb).jpg?width=900",
                "Свинско, лук, чушки и доматен сос, сервирани в запечена питка.", 420, 17.90, now);
            await AddProductIfMissingAsync(context, categories["Основни ястия"], "Родопски пататник хапки",
                "https://commons.wikimedia.org/wiki/Special:FilePath/Patatnik.jpg?width=900",
                "Картофи, сирене, джоджен и масло, поднесени като споделено ястие.", 320, 12.90, now);
            await AddProductIfMissingAsync(context, categories["Морски ястия"], "Черноморски сафрид с лимон",
                "https://commons.wikimedia.org/wiki/Special:FilePath/Kaliningrad%20-%20Fried%20baltic%20sprat%20at%20Shprot%20restraunt%202.jpg?width=900",
                "Пържен сафрид, лимон, морска сол и свежа салата.", 320, 15.90, now);
            await AddProductIfMissingAsync(context, categories["Морски ястия"], "Миди по бургаски",
                "https://commons.wikimedia.org/wiki/Special:FilePath/Mussels%20cooked%20in%20white%20wine%20in%20Queensland%2002.jpg?width=900",
                "Черноморски миди с бяло вино, чесън, девесил и масло.", 500, 18.90, now);
            await AddProductIfMissingAsync(context, categories["Морски ястия"], "Скариди с ракия и чесън",
                "https://commons.wikimedia.org/wiki/Special:FilePath/Prawns%20in%20garlic%20butter%20-%20Taupo%2C%20New%20Zealand.jpg?width=900",
                "Скариди на тиган с чесън, люта чушка, масло и капка ракия.", 260, 19.90, now);
            await AddProductIfMissingAsync(context, categories["Морски ястия"], "Пъстърва филе с билки",
                "https://commons.wikimedia.org/wiki/Special:FilePath/Grilled_trout_fish.jpg?width=900",
                "Печено филе от пъстърва с билки, лимон и зеленчуци.", 360, 18.50, now);
            await AddProductIfMissingAsync(context, categories["Десерти"], "Крем карамел с морска сол",
                "https://commons.wikimedia.org/wiki/Special:FilePath/Creme_caramel.jpg?width=900",
                "Класически крем карамел с щипка морска сол и карамелен сос.", 180, 7.90, now);
            await AddProductIfMissingAsync(context, categories["Десерти"], "Цедено мляко с мед и орехи",
                "https://commons.wikimedia.org/wiki/Special:FilePath/Yogurt%20salad%20with%20nuts.jpg?width=900",
                "Цедено мляко, български мед, орехи и сезонни плодове.", 220, 8.50, now);

            await AddDrinkIfMissingAsync(context, categories["Алкохолни напитки"], "Просеко",
                "https://images.unsplash.com/photo-1510812431401-41d2bd2722f3?auto=format&fit=crop&w=900&q=80",
                true, 150, 7.90, now);
            await AddDrinkIfMissingAsync(context, categories["Алкохолни напитки"], "Студена сангрия",
                "https://images.unsplash.com/photo-1551024709-8f23befc6f87?auto=format&fit=crop&w=900&q=80",
                true, 300, 9.90, now);
            await AddDrinkIfMissingAsync(context, categories["Безалкохолни напитки"], "Домашна лимонада",
                "https://images.unsplash.com/photo-1621263764928-df1444c5e859?auto=format&fit=crop&w=900&q=80",
                false, 350, 5.90, now);
            await AddDrinkIfMissingAsync(context, categories["Безалкохолни напитки"], "Студен чай праскова",
                "https://images.unsplash.com/photo-1556679343-c7306c1976bc?auto=format&fit=crop&w=900&q=80",
                false, 330, 4.90, now);
            await AddDrinkIfMissingAsync(context, categories["Алкохолни коктейли"], "Мохито",
                "https://images.unsplash.com/photo-1551538827-9c037cb4f32a?auto=format&fit=crop&w=900&q=80",
                true, 300, 11.90, now);
            await AddDrinkIfMissingAsync(context, categories["Алкохолни коктейли"], "Passion Fruit Margarita",
                "https://images.unsplash.com/photo-1556855810-ac404aa91e85?auto=format&fit=crop&w=900&q=80",
                true, 260, 12.90, now);
            await AddDrinkIfMissingAsync(context, categories["Безалкохолни коктейли"], "Virgin Mojito",
                "https://images.unsplash.com/photo-1622597467836-f3285f2131b8?auto=format&fit=crop&w=900&q=80",
                false, 300, 7.90, now);
            await AddDrinkIfMissingAsync(context, categories["Безалкохолни коктейли"], "Tropical Sunrise",
                "https://images.unsplash.com/photo-1546171753-97d7676e4602?auto=format&fit=crop&w=900&q=80",
                false, 320, 8.50, now);

            await AddEventIfMissingAsync(context, "Франкофонски фестивал Солей",
                "https://images.unsplash.com/photo-1514525253161-7a46d19cd819?auto=format&fit=crop&w=1200&q=80",
                "Вечер, вдъхновена от Международния франкофонски фестивал Солей в Созопол: музика, кино, изложби и арт работилници. Програмата е базирана на изданието от 1-8 юни 2025 г., когато фестивалът включваше концерт на Маргарита Хранова, изложби и анимационна програма.",
                new DateTime(2026, 6, 1, 20, 0, 0), now);
            await AddEventIfMissingAsync(context, "Музите - младежка арт вечер",
                "https://images.unsplash.com/photo-1501386761578-eac5c94b800a?auto=format&fit=crop&w=1200&q=80",
                "Младежка вечер с танцови, музикални и театрални изпълнения, вдъхновена от Международния фестивал-конкурс на изкуствата Музите, който традиционно събира участници в Созопол през юли.",
                new DateTime(2026, 7, 4, 20, 0, 0), now);
            await AddEventIfMissingAsync(context, "Фолклорна вечер Созопол",
                "https://images.unsplash.com/photo-1528495612343-9ca9f4a4de28?auto=format&fit=crop&w=1200&q=80",
                "Вечер с български фолклор, танцови групи и морско настроение, вдъхновена от World Cup of Folklore - Sozopol и конкурсните програми на сцените в града.",
                new DateTime(2026, 7, 12, 20, 0, 0), now);
            await AddEventIfMissingAsync(context, "Аполония - вечер на изкуствата",
                "https://images.unsplash.com/photo-1508973379184-7517410fb0bc?auto=format&fit=crop&w=1200&q=80",
                "Културна вечер с джаз, литература и кино акценти, вдъхновена от Празниците на изкуствата Аполония 2025, които представиха над 70 събития в Созопол.",
                new DateTime(2026, 8, 30, 19, 0, 0), now);
            await AddEventIfMissingAsync(context, "Вино и любов край морето",
                "https://images.unsplash.com/photo-1510812431401-41d2bd2722f3?auto=format&fit=crop&w=1200&q=80",
                "Вечер с български винопроизводители, занаятчийски сирена и музикална програма, вдъхновена от фестивала Вино и любов - Созопол 2025 на площад Кулата.",
                new DateTime(2026, 9, 6, 19, 30, 0), now);

            await context.SaveChangesAsync();

            var sampleClient = await userManager.FindByEmailAsync("client@beachbar.test");
            if (sampleClient == null)
            {
                sampleClient = new Client
                {
                    UserName = "client",
                    Email = "client@beachbar.test",
                    FirstName = "Тест",
                    LastName = "Клиент",
                    PhoneNumber = "0888123456",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true
                };

                var result = await userManager.CreateAsync(sampleClient, "123!@#Qwe");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(sampleClient, "Client");
                }
            }
            else if (!await userManager.IsInRoleAsync(sampleClient, "Client"))
            {
                await userManager.AddToRoleAsync(sampleClient, "Client");
            }

            var firstEvent = await context.Events.OrderBy(e => e.DateReservation).FirstOrDefaultAsync(e => e.DateReservation >= DateTime.Now);
            if (sampleClient != null && firstEvent != null &&
                !await context.Reservations.AnyAsync(r => r.ClientId == sampleClient.Id && r.EventsId == firstEvent.Id))
            {
                context.Reservations.Add(new Reservation
                {
                    ClientId = sampleClient.Id,
                    EventsId = firstEvent.Id,
                    Count = 4,
                    ReservationDate = now,
                    Status = ReservationStatus.Active
                });
                await context.SaveChangesAsync();
            }
        }

        private static async Task SeedNavigationAsync(ApplicationDbContext context)
        {
            await UpsertNavigationItemAsync(context, "Меню", "Products", "Index", 10);
            await UpsertNavigationItemAsync(context, "Напитки", "Drinks", "Index", 20);
            await UpsertNavigationItemAsync(context, "Галерия", "Home", "Gallery", 30);
            await UpsertNavigationItemAsync(context, "Събития", "Events", "Index", 40);
            await UpsertNavigationItemAsync(context, "Резервация", "Reservations", "Create", 50);
            await UpsertNavigationItemAsync(context, "За нас", "Home", "About", 60);
            await UpsertNavigationItemAsync(context, "Категории", "Categories", "Index", 90, "Admin");
            await UpsertNavigationItemAsync(context, "Всички резервации", "Reservations", "Index", 100, "Admin");
            await UpsertNavigationItemAsync(context, "Справка по събитие", "Reservations", "ByEvent", 110, "Admin");

            await DeactivateNavigationItemAsync(context, "Drinks", "AlcoholicCocktails");
            await DeactivateNavigationItemAsync(context, "Drinks", "NonAlcoholicCocktails");
            await DeactivateNavigationItemAsync(context, "Home", "Contacts");

            await context.SaveChangesAsync();
        }

        private static async Task CleanupOldSampleDataAsync(ApplicationDbContext context)
        {
            var oldProductNames = new[]
            {
                "Fish Tacos",
                "Crispy Calamari",
                "Sunset Bruschetta",
                "Tropical Fruit Bowl",
                "Скариди на плоча",
                "Брускети с домати",
                "Плодова купа",
                "Такос с риба"
            };
            var oldDrinkNames = new[] { "Passion Fruit Mojito", "Watermelon Lemonade", "Aperol Spritz" };
            var oldEventNames = new[] { "Sunset DJ Session", "Acoustic Beach Night", "Tropical Brunch" };
            var oldCategoryNames = new[] { "Beach Bites", "Seafood", "Fresh Drinks", "Cocktails" };

            var oldProducts = await context.Products
                .Where(p => oldProductNames.Contains(p.Name))
                .ToListAsync();
            context.Products.RemoveRange(oldProducts);

            var oldDrinks = await context.Drinks
                .Where(d => oldDrinkNames.Contains(d.Name))
                .ToListAsync();
            context.Drinks.RemoveRange(oldDrinks);

            var oldEvents = await context.Events
                .Where(e => oldEventNames.Contains(e.Name))
                .ToListAsync();
            if (oldEvents.Count > 0)
            {
                var oldEventIds = oldEvents.Select(e => e.Id).ToList();
                var oldReservations = await context.Reservations
                    .Where(r => oldEventIds.Contains(r.EventsId))
                    .ToListAsync();
                context.Reservations.RemoveRange(oldReservations);
                context.Events.RemoveRange(oldEvents);
            }

            await context.SaveChangesAsync();

            var oldCategories = await context.Categories
                .Where(c => oldCategoryNames.Contains(c.Name) &&
                    !context.Products.Any(p => p.CategoryId == c.Id) &&
                    !context.Drinks.Any(d => d.CategoryId == c.Id))
                .ToListAsync();
            context.Categories.RemoveRange(oldCategories);

            await context.SaveChangesAsync();
        }

        private static async Task UpsertNavigationItemAsync(
            ApplicationDbContext context,
            string text,
            string controller,
            string action,
            int sortOrder,
            string? requiredRole = null)
        {
            var item = await context.NavigationMenuItems
                .FirstOrDefaultAsync(n => n.Controller == controller && n.Action == action);

            if (item == null)
            {
                context.NavigationMenuItems.Add(new NavigationMenuItem
                {
                    Text = text,
                    Controller = controller,
                    Action = action,
                    Area = string.Empty,
                    RequiredRole = requiredRole,
                    SortOrder = sortOrder,
                    IsActive = true
                });
                return;
            }

            item.Text = text;
            item.RequiredRole = requiredRole;
            item.SortOrder = sortOrder;
            item.IsActive = true;
        }

        private static async Task DeactivateNavigationItemAsync(
            ApplicationDbContext context,
            string controller,
            string action)
        {
            var item = await context.NavigationMenuItems
                .FirstOrDefaultAsync(n => n.Controller == controller && n.Action == action);

            if (item != null)
            {
                item.IsActive = false;
            }
        }

        private static async Task AddProductIfMissingAsync(
            ApplicationDbContext context,
            int categoryId,
            string name,
            string imageUrl,
            string description,
            double weight,
            double price,
            DateTime now)
        {
            var existingProduct = await context.Products.FirstOrDefaultAsync(p => p.Name == name);
            if (existingProduct != null)
            {
                existingProduct.CategoryId = categoryId;
                existingProduct.ImageUrl = imageUrl;
                existingProduct.Description = description;
                existingProduct.Weight = weight;
                existingProduct.Price = price;
                existingProduct.LastUpdatedOn = now;
                return;
            }

            context.Products.Add(new Product
            {
                Name = name,
                CategoryId = categoryId,
                ImageUrl = imageUrl,
                Description = description,
                Weight = weight,
                Price = price,
                RegisterOn = now,
                LastUpdatedOn = now
            });
        }

        private static async Task AddDrinkIfMissingAsync(
            ApplicationDbContext context,
            int categoryId,
            string name,
            string imageUrl,
            bool isAlcoholic,
            double weight,
            double price,
            DateTime now)
        {
            if (await context.Drinks.AnyAsync(d => d.Name == name))
            {
                return;
            }

            context.Drinks.Add(new Drink
            {
                Name = name,
                CategoryId = categoryId,
                ImageUrl = imageUrl,
                IsAlcoholic = isAlcoholic,
                Weight = weight,
                Price = price,
                RegisterOn = now,
                LastUpdatedOn = now
            });
        }

        private static async Task AddEventIfMissingAsync(
            ApplicationDbContext context,
            string name,
            string imageUrl,
            string description,
            DateTime dateReservation,
            DateTime now)
        {
            var existingEvent = await context.Events.FirstOrDefaultAsync(e => e.Name == name);
            if (existingEvent != null)
            {
                existingEvent.ImageUrl = imageUrl;
                existingEvent.Description = description;
                existingEvent.DateReservation = dateReservation;
                existingEvent.LastUpdatedOn = now;
                return;
            }

            context.Events.Add(new Event
            {
                Name = name,
                ImageUrl = imageUrl,
                Description = description,
                DateReservation = dateReservation,
                RegisterOn = now,
                LastUpdatedOn = now
            });
        }

    }
}
