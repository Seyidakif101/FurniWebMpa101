using FurniMpa101.App.Contexts;
using FurniMpa101.App.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Logging;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;

namespace FurniMpa101.App.Areas.Admin.Controllers
{
        [Area("Admin")]
    public class BlogController(AppDbContext _context) : Controller
    {
        public async Task<IActionResult> Index()
        {
            return View(await _context.Blogs.AsQueryable().Include(b => b.Employee).ToListAsync());
        }
        public async Task<IActionResult> Create()
        {
            await ViewsBagEmployeeId();
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Blog blog)
        {
            if (!ModelState.IsValid)
            {
                await ViewsBagEmployeeId();
                return View(blog);
            }

            var isExistingEmployee = await _context.Employees.AnyAsync(e => e.Id == blog.EmployeeId);

            if (!isExistingEmployee)
            {
                ModelState.AddModelError("EmployeeId", "Secdiyiniz employee yoxdu!");
                await ViewsBagEmployeeId();
                return View(blog);
            }
            blog.CreateDate = DateTime.UtcNow.AddHours(4);
            await _context.Blogs.AddAsync(blog);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Delete(int id)
        {
            var blog = await _context.Blogs.FindAsync(id);
            if (blog is null) return NotFound();
            _context.Blogs.Remove(blog);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var blog = await _context.Blogs.FindAsync(id);
            if (blog is not { }) return NotFound();
            await ViewsBagEmployeeId();
            return View(blog);
        }
        [HttpPost]
        public async Task<IActionResult> Update(Blog blog)
        {
            if (!ModelState.IsValid)
            {
                await ViewsBagEmployeeId();
                return View();
            }
            var existBlog = await _context.Blogs.FindAsync(blog.Id);
            if (existBlog is null) return NotFound();
            var isExistingBlog = await _context.Blogs.AnyAsync(c => c.Id == blog.EmployeeId);
            if (!isExistingBlog)
            {
                ModelState.AddModelError("EmployeeId", "Secdiyiniz employee yoxdu!");
                await ViewsBagEmployeeId();
                return View(blog);
            }
            existBlog.UpdateDate = DateTime.UtcNow.AddHours(4); 
            existBlog.Title = blog.Title;
            existBlog.Text = blog.Text;
            existBlog.ImageName = blog.ImageName;
            existBlog.ImageUrl = blog.ImageUrl;
            existBlog.EmployeeId = blog.EmployeeId;
            _context.Blogs.Update(existBlog);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        private async Task ViewsBagEmployeeId()
        {
            var emplyees = await _context.Employees.ToListAsync();
            ViewBag.Employees = emplyees;
        }
    }
}
