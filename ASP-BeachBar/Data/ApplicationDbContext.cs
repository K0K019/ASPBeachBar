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
        public DbSet<Drink> Drinks { get; set; }
        public DbSet<Event> Events { get; set; }

        public DbSet<Product> Products { get; set; }

        public DbSet<Reservation> Reservations { get; set; }
    }
}
