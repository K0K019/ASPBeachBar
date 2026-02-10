namespace ASP_BeachBar.Data
{
    public class Event
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string ImageUrl { get; set; }
        public string Description { get; set; }

        public DateTime DateReservation { get; set; }

        public DateTime RegisterOn { get; set; }

        public ICollection<Reservation> Reservations { get; set; }

    }
}