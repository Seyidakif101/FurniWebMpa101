using FurniMpa101.App.Contexts;
using FurniMpa101.App.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Threading.Tasks;

namespace FurniMpa101.App.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController(AppDbContext _context) : Controller
    {
        public async Task<IActionResult> Index()
        {
            return View(await _context.Products.AsQueryable().ToListAsync());
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Product product)
        {
            if (!ModelState.IsValid) return View(product);
            product.CreatedDate = DateTime.UtcNow.AddHours(4);
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Delete(int id)
        {
            var shipping = await _context.Products.FindAsync(id);
            if (shipping is null) return NotFound();
            _context.Products.Remove(shipping);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product is not { }) return NotFound();
            return View(product);
        }
        [HttpPost]
        public async Task<IActionResult> Update(Product product)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            var existProduct = await _context.Products.FindAsync(product.Id);
            if (existProduct is null) return NotFound();
            existProduct.UpdatedDate= DateTime.UtcNow.AddHours(4);
            existProduct.Name = product.Name;
            existProduct.Price = product.Price;
            existProduct.ImageName = product.ImageName;
            existProduct.ImageUrl = product.ImageUrl;
            _context.Products.Update(existProduct);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Toggle(int id)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            var existIsDeleted = await _context.Products.FindAsync(id);
            if (existIsDeleted is null) return NotFound();
            existIsDeleted.IsDeleted=!existIsDeleted.IsDeleted;
            _context.Products.Update(existIsDeleted);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
