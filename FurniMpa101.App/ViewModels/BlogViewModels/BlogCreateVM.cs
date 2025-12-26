using FurniMpa101.App.Models;
using FurniMpa101.App.Models.Common;
using System.ComponentModel.DataAnnotations;

namespace FurniMpa101.App.ViewModels.BlogViewModels
{
    public class BlogCreateVM:BaseEntity
    {

        [MaxLength(50)]
        [MinLength(3)]
        public string Title { get; set; } = null!;
        public string Text { get; set; }
        [Required]
        public int EmployeeId { get; set; }
        public IFormFile Image { get; set; }
    }
}
