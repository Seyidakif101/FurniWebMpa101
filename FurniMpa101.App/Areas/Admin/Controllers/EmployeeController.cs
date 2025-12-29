using FurniMpa101.App.Contexts;
using FurniMpa101.App.Helpers;
using FurniMpa101.App.Models;
using FurniMpa101.App.ViewModels.EmployeeViewModels;
using FurniMpa101.App.ViewModels.ProductViewModels;
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
            var employees = await _context.Employees.Select(employee => new EmployeeGetVM()
            {
                Id = employee.Id,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                ImageUrl = employee.ImageUrl,
                Position = employee.Position,
                Description= employee.Description

            }).ToListAsync();
            return View(employees);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(EmployeeCreateVM vm )
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
            Employee employee = new()
            {
                FirstName = vm.FirstName,
                LastName = vm.LastName,
                Position = vm.Position,
                Description = vm.Description,
                ImageUrl=ImageFileName,
                CreatedDate = DateTime.UtcNow.AddHours(4)
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
            var employee = await _context.Employees.SingleOrDefaultAsync(x => x.Id == id);
            if (employee is null) return NotFound();
            EmployeeUpdateVM vm = new EmployeeUpdateVM()
            {

                Id = employee.Id,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                ImageUrl = employee.ImageUrl,
                Position = employee.Position,
                Description = employee.Description

            };
            return View(vm);
        }
        [HttpPost]
        public async Task<IActionResult> Update(EmployeeUpdateVM vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }
            var existEmployee = await _context.Employees.FirstOrDefaultAsync(x => x.Id == vm.Id);
            if (existEmployee is null) return NotFound();
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
            existEmployee.UpdatedDate = DateTime.UtcNow.AddHours(4);
            existEmployee.FirstName = vm.FirstName;
            existEmployee.LastName = vm.LastName;
            existEmployee.Position = vm.Position;
            existEmployee.Description = vm.Description;
            string folderPath = Path.Combine(_environment.WebRootPath, "assets", "images");
            if (vm.Image is { })
            {
                string newImage = await vm.Image.SaveFileAsync(folderPath);
                string existImage = Path.Combine(folderPath, existEmployee.ImageUrl);

                ExtensionMethods.DeleteFile(existImage);
                existEmployee.ImageUrl = newImage;
            }
            _context.Employees.Update(existEmployee);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

    }
}
