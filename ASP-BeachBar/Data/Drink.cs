using System.ComponentModel.DataAnnotations;

namespace ASP_BeachBar.Data
{
    public class Drink
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Името е задължително.")]
        [Display(Name = "Име")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Снимката е задължителна.")]
        [Url(ErrorMessage = "Въведи валиден URL адрес.")]
        [Display(Name = "Снимка")]
        public string ImageUrl { get; set; } = string.Empty;

        [Display(Name = "Алкохолна напитка")]
        public bool IsAlcoholic { get; set; }

        [Display(Name = "Категория")]
        public int CategoryId { get; set; }

        public Category Categories { get; set; } = null!;

        [Range(1, 10000, ErrorMessage = "Количеството трябва да е положително число.")]
        [Display(Name = "Количество")]
        public double Weight { get; set; }

        [Range(0.01, 10000, ErrorMessage = "Цената трябва да е положително число.")]
        [Display(Name = "Цена")]
        public double Price { get; set; }

        [Display(Name = "Добавена на")]
        public DateTime RegisterOn { get; set; }

        [Display(Name = "Последна актуализация")]
        public DateTime LastUpdatedOn { get; set; }
    }
}
