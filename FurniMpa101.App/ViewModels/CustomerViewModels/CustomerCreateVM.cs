using FurniMpa101.App.Models.Common;
using System.ComponentModel.DataAnnotations;

namespace FurniMpa101.App.ViewModels.CustomerViewModels
{
    public class CustomerCreateVM:BaseEntity
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        public IFormFile Image { get; set; }
    }
}
