using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASP_BeachBar.Data
{
    public class Product
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Името е задължително.")]
        [Display(Name = "Име")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Категория")]
        public int CategoryId { get; set; }

        public Category Categories { get; set; } = null!;

        [Required(ErrorMessage = "Снимката е задължителна.")]
        [Url(ErrorMessage = "Въведи валиден URL адрес.")]
        [Display(Name = "Снимка")]
        public string ImageUrl { get; set; } = string.Empty;

        [Required(ErrorMessage = "Описанието е задължително.")]
        [Display(Name = "Описание")]
        public string Description { get; set; } = string.Empty;

        [Range(1, 10000, ErrorMessage = "Грамажът трябва да е положително число.")]
        [Display(Name = "Грамаж")]
        public double Weight { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        [Range(0.01, 10000, ErrorMessage = "Цената трябва да е положително число.")]
        [Display(Name = "Цена")]
        public double Price { get; set; }

        [Display(Name = "Добавен на")]
        public DateTime RegisterOn { get; set; }

        [Display(Name = "Последна актуализация")]
        public DateTime LastUpdatedOn { get; set; }
    }
}
