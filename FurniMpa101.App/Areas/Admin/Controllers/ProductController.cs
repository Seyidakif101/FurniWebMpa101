using FurniMpa101.App.Contexts;
using FurniMpa101.App.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FurniMpa101.App.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController(AppDbContext _context) : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet("/product")]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Product product)
        {
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
