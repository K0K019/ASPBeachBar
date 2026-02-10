namespace ASP_BeachBar.Data
{
    public class Reservation
    {
        public int Id { get; set; }
        public string ClientId { get; set; }
        public Client Clients { get; set; }

        
        public int EventsId { get; set; }

        public Event Events { get; set; }
        public int Count { get; set; }

        public DateTime ReservationDate { get; set; }

    }
}
