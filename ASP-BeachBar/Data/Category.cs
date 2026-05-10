using System.ComponentModel.DataAnnotations;

namespace ASP_BeachBar.Data
{
    public class Category
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Името е задължително.")]
        [Display(Name = "Име")]
        public string Name { get; set; } = string.Empty;

        public ICollection<Drink> Drinks { get; set; } = new List<Drink>();

        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
