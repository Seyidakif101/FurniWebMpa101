using FurniMpa101.App.Models.Common;
using System.ComponentModel.DataAnnotations;

namespace FurniMpa101.App.Models
{
    public class Customer:BaseEntity
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        public string ImageUrl { get; set; }
    }
}
