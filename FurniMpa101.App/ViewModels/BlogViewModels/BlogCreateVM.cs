using System.ComponentModel.DataAnnotations;

namespace FurniMpa101.App.ViewModels.BlogViewModels
{
    public class BlogCreateVM
    {
        public int Id { get; set; }
        [MaxLength(50)]
        [MinLength(3)]
        public string Title { get; set; } = null!;
        public string Text { get; set; }
        [Required]
        public int EmployeeId { get; set; }
        public IFormFile Image { get; set; }

        public List<int> TagIds { get; set; }
    }
}
