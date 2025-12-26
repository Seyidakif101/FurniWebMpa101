using FurniMpa101.App.Contexts;
using FurniMpa101.App.Models;
using FurniMpa101.App.ViewModels.EmployeeViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FurniMpa101.App.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AutoValidateAntiforgeryToken]
    public class EmployeeController(AppDbContext _context, IWebHostEnvironment _environment) : Controller
    {
        public async Task<IActionResult> Index()
        {
            return View(await _context.Employees.AsQueryable().ToListAsync());
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(EmployeeCreateVM vm )
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
            Employee employee = new()
            {
                FirstName = vm.FirstName,
                LastName = vm.LastName,
                Position = vm.Position,
                Description = vm.Description,
                ImageUrl=ImageFileName
            };
            await _context.Employees.AddAsync(employee);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Delete(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee is null) return NotFound();
            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();
            string folderUrl = Path.Combine(_environment.WebRootPath, "assets", "images");
            string ImageUrl = Path.Combine(folderUrl, employee.ImageUrl);

            if (System.IO.File.Exists(ImageUrl))
            {
                System.IO.File.Delete(ImageUrl);
            }
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee is not { }) return NotFound();
            return View(employee);
        }
        [HttpPost]
        public async Task<IActionResult> Update(Employee employee)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            var existEmployee = await _context.Employees.FindAsync(employee.Id);
            if (existEmployee is null) return NotFound();
            existEmployee.UpdatedDate = DateTime.UtcNow.AddHours(4);
            existEmployee.FirstName = employee.FirstName;
            existEmployee.LastName = employee.LastName;
            existEmployee.Position = employee.Position;
            existEmployee.Description = employee.Description;
            existEmployee.ImageUrl = employee.ImageUrl;
            _context.Employees.Update(existEmployee);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

    }
}
