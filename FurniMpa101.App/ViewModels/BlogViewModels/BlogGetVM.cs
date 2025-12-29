using System.ComponentModel.DataAnnotations;

namespace FurniMpa101.App.ViewModels.BlogViewModels
{
    public class BlogGetVM
    {
        public int Id { get; set; }
        [MaxLength(50)]
        [MinLength(3)]
        public string Title { get; set; } = null!;
        public string Text { get; set; }
        [Required]
        public string EmployeeName { get; set; }
        public string? ImageUrl { get; set; }
        public List<string> TagNames { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
