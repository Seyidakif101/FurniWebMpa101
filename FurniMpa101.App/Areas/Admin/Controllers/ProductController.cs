using FurniMpa101.App.Contexts;
using FurniMpa101.App.Models;
using FurniMpa101.App.ViewModels.ProductViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Threading.Tasks;

namespace FurniMpa101.App.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController(AppDbContext _context, IWebHostEnvironment _environment) : Controller
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
        public async Task<IActionResult> Create(ProductCreateVM vm)
        {
            if (!ModelState.IsValid) return View(vm);
            if (!vm.Image.ContentType.Contains("image"))
            {
                ModelState.AddModelError("Image", "File sekil formatinda olmalidir!");
                return View(vm);
            }
            if (vm.Image.Length > 2 * 1024 * 1024)
            {
                ModelState.AddModelError("Image", "File olcusu maksimum 2MB ola biler!");
                return View(vm);
            }
            string ImageFileName = Guid.NewGuid().ToString() + vm.Image.FileName;
            string ImageUrl = Path.Combine(_environment.WebRootPath, "assets", "images", ImageFileName);
            using FileStream Stream = new(ImageUrl, FileMode.Create);
            await vm.Image.CopyToAsync(Stream);
            vm.CreatedDate = DateTime.UtcNow.AddHours(4);
            Product product = new()
            {
                Name = vm.Name,
                Price = vm.Price,
                ImageUrl = ImageFileName,
                CreatedDate = DateTime.UtcNow,

            };

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product is null) return NotFound();
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            string folderUrl = Path.Combine(_environment.WebRootPath, "assets", "images");
            string ImageUrl = Path.Combine(folderUrl, product.ImageUrl);

            if (System.IO.File.Exists(ImageUrl))
            {
                System.IO.File.Delete(ImageUrl);
            }
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
