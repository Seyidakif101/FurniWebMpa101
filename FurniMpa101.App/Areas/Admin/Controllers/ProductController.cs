using FurniMpa101.App.Contexts;
using FurniMpa101.App.Helpers;
using FurniMpa101.App.Models;
using FurniMpa101.App.ViewModels.ProductViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Threading.Tasks;

namespace FurniMpa101.App.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AutoValidateAntiforgeryToken]
    public class ProductController(AppDbContext _context, IWebHostEnvironment _environment) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var products = await _context.Products.Select(product => new ProductGetVM()
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                ImageUrl = product.ImageUrl,
                IsDeleted= product.IsDeleted

            }).ToListAsync();
            return View(products);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(ProductCreateVM vm)
        {
            if (!ModelState.IsValid) return View(vm);
            if (!vm.Image.CheckType())
            {
                ModelState.AddModelError("Image", "File sekil formatinda olmalidir!");
                return View(vm);
            }
            if (vm.Image.CheckSize(2))
            {
                ModelState.AddModelError("Image", "File olcusu maksimum 2MB ola biler!");
                return View(vm);
            }

            string ImageFileName = Guid.NewGuid().ToString() + vm.Image.FileName;
            string ImageUrl = Path.Combine(_environment.WebRootPath, "assets", "images", ImageFileName);
            using FileStream Stream = new(ImageUrl, FileMode.Create);
            await vm.Image.CopyToAsync(Stream);
            Product product = new()
            {
                Name = vm.Name,
                Price = vm.Price,
                ImageUrl = ImageFileName,
                CreatedDate = DateTime.UtcNow.AddHours(4),
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
            var product = await _context.Products.SingleOrDefaultAsync(x => x.Id == id);
            if (product is null) return NotFound();
            ProductUpdateVM vm = new ProductUpdateVM()
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                ImageUrl = product.ImageUrl,
            };
            return View(vm);
        }
        [HttpPost]
        public async Task<IActionResult> Update(ProductUpdateVM vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }
            var existProduct = await _context.Products.FirstOrDefaultAsync(x => x.Id == vm.Id);
            if (existProduct is null) return NotFound();
            if (!vm.Image?.CheckType() ?? false)
            {
                ModelState.AddModelError("Image", "File sekil formatinda olmalidir!");
                return View(vm);
            }
            if (vm.Image?.CheckSize(2) ?? false)
            {
                ModelState.AddModelError("Image", "File olcusu maksimum 2MB ola biler!");
                return View(vm);
            }
            existProduct.UpdatedDate= DateTime.UtcNow.AddHours(4);
            existProduct.Name = vm.Name;
            existProduct.Price = vm.Price;
            string folderPath = Path.Combine(_environment.WebRootPath, "assets", "images");
            if (vm.Image is { })
            {
                string newImage = await vm.Image.SaveFileAsync(folderPath);
                string existImage = Path.Combine(folderPath, existProduct.ImageUrl);

                ExtensionMethods.DeleteFile(existImage);
                existProduct.ImageUrl = newImage;
            }
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
