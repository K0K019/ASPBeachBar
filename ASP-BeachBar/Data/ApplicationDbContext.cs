using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ASP_BeachBar.Data
{
    public class ApplicationDbContext : IdentityDbContext<Client>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }
        public DbSet<Drink> Drinks { get; set; } = null!;
        public DbSet<Event> Events { get; set; } = null!;

        public DbSet<Product> Products { get; set; } = null!;

        public DbSet<Reservation> Reservations { get; set; } = null!;

        public DbSet<Category> Categories { get; set; } = null!;

        public DbSet<NavigationMenuItem> NavigationMenuItems { get; set; } = null!;
    }
}
