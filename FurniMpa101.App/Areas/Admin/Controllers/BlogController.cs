using FurniMpa101.App.Contexts;
using FurniMpa101.App.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FurniMpa101.App.Areas.Admin.Controllers
{
        [Area("Admin")]
    public class BlogController(AppDbContext _context) : Controller
    {
        public async Task<IActionResult> Index()
        {
            return View(await _context.Blogs.AsQueryable().ToListAsync());
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Blog blog)
        {
            if (!ModelState.IsValid) return View(blog);
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
            return View(blog);
        }
        [HttpPost]
        public async Task<IActionResult> Update(Blog blog)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            var existBlog = await _context.Blogs.FindAsync(blog.Id);
            if (existBlog is null) return NotFound();
            existBlog.UpdateDate = DateTime.UtcNow.AddHours(4); 
            existBlog.Title = blog.Title;
            existBlog.Text = blog.Text;
            existBlog.ImageName = blog.ImageName;
            existBlog.ImageUrl = blog.ImageUrl;
            _context.Blogs.Update(existBlog);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
