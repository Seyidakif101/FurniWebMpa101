using System.ComponentModel.DataAnnotations;

namespace FurniMpa101.App.ViewModels.CustomerViewModels
{
    public class CustomerUpdateVM
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        public IFormFile? Image { get; set; }
        public string? ImageUrl { get; set; }
    }
}
