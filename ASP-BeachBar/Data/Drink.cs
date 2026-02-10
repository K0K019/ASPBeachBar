namespace ASP_BeachBar.Data
{
    public class Drink
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string ImageUrl { get; set; }

        public bool isAlcoholic { get; set; }

        public string CategoryId { get; set; }

        public double weight { get; set; }

        public double price { get; set; }

        public DateTime RegisterOn { get; set; }
    }
}
