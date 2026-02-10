namespace ASP_BeachBar.Data
{
    public class Drink
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string ImageUrl { get; set; }

        public bool IsAlcoholic { get; set; }

        public string CategoryId { get; set; }

        public double Weight { get; set; }

        public double Price { get; set; }

        public DateTime RegisterOn { get; set; }
    }
}
