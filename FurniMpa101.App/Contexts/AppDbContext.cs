using FurniMpa101.App.Models;
using Microsoft.EntityFrameworkCore;

namespace FurniMpa101.App.Contexts
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<Product> Products { get; set; }
        public DbSet<Employee> Employees { get; set; }

    }
}
