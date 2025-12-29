using FurniMpa101.App.Contexts;
using FurniMpa101.App.Helpers;
using FurniMpa101.App.Models;
using FurniMpa101.App.ViewModels.CustomerViewModels;
using FurniMpa101.App.ViewModels.ProductViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace FurniMpa101.App.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CustomerController(AppDbContext _context, IWebHostEnvironment _environment) : Controller
    {
        public async Task<IActionResult> Index()
        {

            var customers = await _context.Customers.Select(customer => new CustomerGetVM()
            {
                Id = customer.Id,
                UserName = customer.UserName,
                Password = customer.Password,
                Email = customer.Email,
                ImageUrl = customer.ImageUrl

            }).ToListAsync();
            return View(customers);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CustomerCreateVM vm)
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
            Customer customer = new()
            {
                UserName=vm.UserName,
                Password=vm.Password,
                Email=vm.Email,
                ImageUrl=ImageFileName,
                CreatedDate = DateTime.UtcNow.AddHours(4)
            };
            await _context.Customers.AddAsync(customer);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Delete(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer is null) return NotFound();
            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();
            string folderUrl = Path.Combine(_environment.WebRootPath, "assets", "images");
            string ImageUrl = Path.Combine(folderUrl, customer.ImageUrl);

            if (System.IO.File.Exists(ImageUrl))
            {
                System.IO.File.Delete(ImageUrl);
            }
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var customer = await _context.Customers.SingleOrDefaultAsync(x => x.Id == id);
            if (customer is null) return NotFound();
            CustomerUpdateVM vm = new CustomerUpdateVM()
            {
                Id = customer.Id,
                UserName = customer.UserName,
                Password = customer.Password,
                Email = customer.Email,
                ImageUrl = customer.ImageUrl
            };
            return View(vm);
        }
        [HttpPost]
        public async Task<IActionResult> Update(CustomerUpdateVM vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }
            var existCustomer = await _context.Customers.FirstOrDefaultAsync(x => x.Id == vm.Id);
            if (existCustomer is null) return NotFound();
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
            if (existCustomer is null) return NotFound();
            existCustomer.UpdatedDate = DateTime.UtcNow.AddHours(4);
            existCustomer.UserName = vm.UserName;
            existCustomer.Password = vm.Password;
            existCustomer.Email = vm.Email;
            string folderPath = Path.Combine(_environment.WebRootPath, "assets", "images");
            if (vm.Image is { })
            {
                string newImage = await vm.Image.SaveFileAsync(folderPath);
                string existImage = Path.Combine(folderPath, existCustomer.ImageUrl);

                ExtensionMethods.DeleteFile(existImage);
                existCustomer.ImageUrl = newImage;
            }
            _context.Customers.Update(existCustomer);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
