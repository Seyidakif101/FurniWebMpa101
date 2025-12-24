using System.ComponentModel.DataAnnotations;

namespace FurniMpa101.App.Models
{
    public class Product
    {
        public int Id { get; set; }
        [MaxLength(50)]
        [MinLength(3)]
        public string Name { get; set; } = null!;

        public double Price { get; set; }
        public string? ImageName { get; set; }
        [Required]
        public string ImageUrl { get; set; }=null!;
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool IsDeleted { get; set; }
    }
}
