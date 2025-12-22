using FurniMpa101.App.Contexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace FurniMpa101.App.Controllers
{
    public class ProductController:Controller
    {
        readonly AppDbContext _context;
        public ProductController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _context.Products.AsQueryable().ToListAsync());
        }
    }
}
