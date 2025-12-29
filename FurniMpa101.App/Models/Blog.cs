using FurniMpa101.App.Models.Common;
using System.ComponentModel.DataAnnotations;

namespace FurniMpa101.App.Models
{
    public class Blog:BaseEntity
    {
        [MaxLength(50)]
        [MinLength(3)]
        public string Title { get; set; } = null!;
        public string Text { get; set; }
        [Required]
        public int EmployeeId { get; set; }
        public Employee? Employee { get; set; }
        [Required]
        public string ImageUrl { get; set; }
        public ICollection<BlogTag> BlogTags { get; set; } = [];
    }
}
