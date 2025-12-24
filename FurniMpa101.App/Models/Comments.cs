namespace FurniMpa101.App.Models
{
    public class Comments
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }
        public bool IsAccepted { get; set; }


    }
}
