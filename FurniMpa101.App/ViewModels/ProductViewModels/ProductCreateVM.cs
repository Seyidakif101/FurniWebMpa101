using System.ComponentModel.DataAnnotations;

namespace FurniMpa101.App.ViewModels.ProductViewModels
{
    public class ProductCreateVM
    {
        public int Id { get; set; }

        [MaxLength(50)]
        [MinLength(3)]
        public string Name { get; set; } = null!;
        public List<int> TagIds { get; set; }
        public double Price { get; set; }
        public IFormFile Image {  get; set; }
        public bool IsDeleted { get; set; }
    }
}
