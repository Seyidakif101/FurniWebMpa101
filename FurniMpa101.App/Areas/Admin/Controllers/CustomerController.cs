using FurniMpa101.App.Contexts;
using FurniMpa101.App.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace FurniMpa101.App.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CustomerController(AppDbContext _context) : Controller
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
        public async Task<IActionResult> Create(Customer customer)
        {
            if (!ModelState.IsValid) return View(customer);
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
            existCustomer.UserName = customer.UserName;
            existCustomer.Password = customer.Password;
            existCustomer.Email = customer.Email;
            _context.Customers.Update(existCustomer);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
