using System.ComponentModel.DataAnnotations;

namespace ASP_BeachBar.Data
{
    public class Reservation
    {
        public int Id { get; set; }

        public string ClientId { get; set; } = string.Empty;

        public Client Clients { get; set; } = null!;

        public int EventsId { get; set; }

        public Event Events { get; set; } = null!;

        [Range(1, 100, ErrorMessage = "Броят места трябва да е поне 1.")]
        [Display(Name = "Брой места")]
        public int Count { get; set; }

        [Display(Name = "Резервирано на")]
        public DateTime ReservationDate { get; set; }
    }
}
