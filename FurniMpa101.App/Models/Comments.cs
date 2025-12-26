using FurniMpa101.App.Models.Common;

namespace FurniMpa101.App.Models
{
    public class Comments:BaseEntity
    {
        public string Text { get; set; }
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }
        public bool IsAccepted { get; set; }


    }
}
