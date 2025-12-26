using FurniMpa101.App.Models.Common;

namespace FurniMpa101.App.ViewModels.EmployeeViewModels
{
    public class EmployeeCreateVM:BaseEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Position { get; set; }
        public string Description { get; set; }
        public IFormFile Image { get; set; }
    }
}
