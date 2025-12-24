using FurniMpa101.App.Contexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FurniMpa101.App.Controllers
{
    public class BlogController(AppDbContext _context) : Controller
    {
        public async Task<IActionResult> Index()
        {
            return View(await _context.Blogs.AsQueryable().ToListAsync());
        }
    }
}
