using FurniMpa101.App.Models.Common;
using System.ComponentModel.DataAnnotations;

namespace FurniMpa101.App.Models
{
    public class Product:BaseEntity
    {
        [MaxLength(50)]
        [MinLength(3)]
        public string Name { get; set; } = null!;

        public double Price { get; set; }
        [Required]
        public string ImageUrl { get; set; }=null!;

    }
}
