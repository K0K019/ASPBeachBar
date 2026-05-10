using System.ComponentModel.DataAnnotations;

namespace ASP_BeachBar.Data
{
    public class Event
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Името е задължително.")]
        [Display(Name = "Име")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Снимката е задължителна.")]
        [Url(ErrorMessage = "Въведи валиден URL адрес.")]
        [Display(Name = "Снимка")]
        public string ImageUrl { get; set; } = string.Empty;

        [Required(ErrorMessage = "Описанието е задължително.")]
        [Display(Name = "Описание")]
        public string Description { get; set; } = string.Empty;

        [Display(Name = "Дата и час")]
        public DateTime DateReservation { get; set; }

        [Display(Name = "Добавено на")]
        public DateTime RegisterOn { get; set; }

        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    }
}
