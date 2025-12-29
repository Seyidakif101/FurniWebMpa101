using FurniMpa101.App.Contexts;
using FurniMpa101.App.Models;
using FurniMpa101.App.ViewModels.BlogViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Logging;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;

namespace FurniMpa101.App.Areas.Admin.Controllers
{
        [Area("Admin")]
    public class BlogController(AppDbContext _context, IWebHostEnvironment _environment) : Controller
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
        public async Task<IActionResult> Create(BlogCreateVM vm)
        {
            if (!ModelState.IsValid) return View(vm);
            var isExistingEmployee = await _context.Employees.AnyAsync(e => e.Id ==vm.EmployeeId);

            if (!isExistingEmployee)
            {
                ModelState.AddModelError("EmployeeId", "Secdiyiniz employee yoxdu!");
                await ViewsBagEmployeeId();
                return View(vm);
            }
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
            Blog blog = new()
            {
                Title = vm.Title,
                Text = vm.Text,
                EmployeeId = vm.EmployeeId,
                ImageUrl = ImageFileName,
                CreatedDate = DateTime.UtcNow.AddHours(4)
            };

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
            string folderUrl = Path.Combine(_environment.WebRootPath, "assets", "images");
            string ImageUrl = Path.Combine(folderUrl, blog.ImageUrl);

            if (System.IO.File.Exists(ImageUrl))
            {
                System.IO.File.Delete(ImageUrl);
            }
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
            existBlog.UpdatedDate = DateTime.UtcNow.AddHours(4); 
            existBlog.Title = blog.Title;
            existBlog.Text = blog.Text;
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
