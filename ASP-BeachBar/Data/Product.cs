using System.ComponentModel.DataAnnotations.Schema;

namespace ASP_BeachBar.Data
{
    public class Product
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int CategoryId { get; set; }

        public Category Categories { get; set; }

        public string ImageUrl { get; set; }

        public string Description { get; set; }

        public double Weight { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public double Price { get; set; }


        public DateTime RegisterOn { get; set; }



    }
}
