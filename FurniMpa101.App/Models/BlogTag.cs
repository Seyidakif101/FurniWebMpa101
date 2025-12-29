using FurniMpa101.App.Models.Common;

namespace FurniMpa101.App.Models
{
    public class BlogTag:BaseEntity
    {
        public Blog Blog { get; set; } = null!;
        public int BlogId { get; set; }
        public Tag Tag { get; set; } = null!;
        public int TagId { get; set; }


    }
}
