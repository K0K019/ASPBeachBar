namespace ASP_BeachBar.Data
{
    public class Category
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public ICollection<Drink> Drinks { get; set; }
        public ICollection<Product> Products { get; set; }
    }
}
