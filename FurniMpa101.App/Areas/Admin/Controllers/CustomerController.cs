using FurniMpa101.App.Contexts;
using FurniMpa101.App.Models;
using FurniMpa101.App.ViewModels.CustomerViewModels;
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
            return View( await _context.Customers.AsQueryable().ToListAsync());
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
            vm.CreatedDate = DateTime.UtcNow.AddHours(4);
            Customer customer = new()
            {
                UserName=vm.UserName,
                Password=vm.Password,
                Email=vm.Email,
                ImageUrl=ImageFileName

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
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer is not { }) return NotFound();
            return View(customer);
        }
        [HttpPost]
        public async Task<IActionResult> Update(Customer customer)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            var existCustomer = await _context.Customers.FindAsync(customer.Id);
            if (existCustomer is null) return NotFound();
            existCustomer.UpdatedDate = DateTime.UtcNow.AddHours(4);
            existCustomer.UserName = customer.UserName;
            existCustomer.Password = customer.Password;
            existCustomer.Email = customer.Email;
            _context.Customers.Update(existCustomer);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
