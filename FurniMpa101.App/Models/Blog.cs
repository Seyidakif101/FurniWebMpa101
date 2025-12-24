using System.ComponentModel.DataAnnotations;

namespace FurniMpa101.App.Models
{
    public class Blog
    {
        public int Id { get; set; }
        [MaxLength(50)]
        [MinLength(3)]
        public string Title { get; set; } = null!;
        public string Text { get; set; }
        public int EmployeeId { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public string ImageName { get; set; }
        [Required]
        public string ImageUrl { get; set; }
    }
}
