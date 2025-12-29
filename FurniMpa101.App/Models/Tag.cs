using FurniMpa101.App.Models.Common;

namespace FurniMpa101.App.Models
{
     public class Tag : BaseEntity
        {
            public string Name { get; set; }
            public ICollection<BlogTag> BlogTags { get; set; } = [];
        }
}
