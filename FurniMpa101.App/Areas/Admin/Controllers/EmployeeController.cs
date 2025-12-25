using FurniMpa101.App.Contexts;
using FurniMpa101.App.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FurniMpa101.App.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AutoValidateAntiforgeryToken]
    public class EmployeeController(AppDbContext _context) : Controller
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
        public async Task<IActionResult> Create(Employee employee)
        {
            if (!ModelState.IsValid) return View(employee);
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
            existEmployee.FirstName = employee.FirstName;
            existEmployee.LastName = employee.LastName;
            existEmployee.Position = employee.Position;
            existEmployee.Description = employee.Description;
            existEmployee.ImageName = employee.ImageName;
            existEmployee.ImageUrl = employee.ImageUrl;
            _context.Employees.Update(existEmployee);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

    }
}
