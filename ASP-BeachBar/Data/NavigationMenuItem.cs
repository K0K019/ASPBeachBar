using System.ComponentModel.DataAnnotations;

namespace ASP_BeachBar.Data
{
    public class NavigationMenuItem
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(80)]
        public string Text { get; set; } = string.Empty;

        [Required]
        [MaxLength(80)]
        public string Controller { get; set; } = string.Empty;

        [Required]
        [MaxLength(80)]
        public string Action { get; set; } = string.Empty;

        [MaxLength(80)]
        public string Area { get; set; } = string.Empty;

        [MaxLength(80)]
        public string? RequiredRole { get; set; }

        public int SortOrder { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
