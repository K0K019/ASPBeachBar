using Microsoft.AspNetCore.Identity;

namespace ASP_BeachBar.Data
{
    public class Client : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public ICollection<Reservation> Reservations { get; set; }

    }
}
