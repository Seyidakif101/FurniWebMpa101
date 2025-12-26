using System.ComponentModel.DataAnnotations;

namespace FurniMpa101.App.ViewModels.ProductViewModels
{
    public class ProductCreateVM
    {

        [MaxLength(50)]
        [MinLength(3)]
        public string Name { get; set; } = null!;

        public double Price { get; set; }
        public IFormFile Image {  get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool IsDeleted { get; set; }
    }
}
