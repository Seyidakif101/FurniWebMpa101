using FurniMpa101.App.Contexts;
using FurniMpa101.App.Helpers;
using FurniMpa101.App.Models;
using FurniMpa101.App.ViewModels.BlogViewModels;
using FurniMpa101.App.ViewModels.ProductViewModels;
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
            var blogs = await _context.Blogs.Include(x => x.Employee).Select(blog => new BlogGetVM()
            {
                Id = blog.Id,
                Title = blog.Title,
                Text = blog.Text,
                EmployeeName = blog.Employee.FirstName,
                ImageUrl = blog.ImageUrl

            }).ToListAsync();
            return View(blogs);
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
            foreach (var tagId in vm.TagIds)
            {
                var isExistTag = await _context.Tags.AnyAsync(x => x.Id == tagId);
                if (!isExistTag)
                {

                    ModelState.AddModelError("TagIds", "Bele bir tag yoxdur");
                    return View(vm);
                }
            }
            if (!isExistingEmployee)
            {
                ModelState.AddModelError("EmployeeId", "Secdiyiniz employee yoxdu!");
                await ViewsBagEmployeeId();
                return View(vm);
            }
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
            Blog blog = new()
            {
                Title = vm.Title,
                Text = vm.Text,
                EmployeeId = vm.EmployeeId,
                ImageUrl = ImageFileName,
                CreatedDate = DateTime.UtcNow.AddHours(4),
                BlogTags = []
            };
            foreach (var tagId in vm.TagIds)
            {
                BlogTag blogTag = new()
                {
                    TagId = tagId,
                    Blog = blog
                };
                blog.BlogTags.Add(blogTag);

            }

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
            var blog = await _context.Blogs.Include(x => x.BlogTags).SingleOrDefaultAsync(x => x.Id == id);
            if (blog is null) return NotFound();
            BlogUpdateVM vm = new BlogUpdateVM()
            {
                Id = blog.Id,
                Title = blog.Title,
                Text = blog.Text,
                EmployeeId = blog.EmployeeId,
                ImageUrl = blog.ImageUrl,
                TagIds = blog.BlogTags.Select(x => x.TagId).ToList()
            };
            return View(vm);
        }
        [HttpPost]
        public async Task<IActionResult> Update(BlogUpdateVM vm)
        {
            if (!ModelState.IsValid)
            {
                await ViewsBagEmployeeId();
                return View(vm);
            }
            var existBlog = await _context.Blogs.Include(x => x.BlogTags).FirstOrDefaultAsync(x => x.Id == vm.Id);;
            if (existBlog is null) return NotFound();
            var isExistingBlog = await _context.Blogs.AnyAsync(c => c.Id == vm.EmployeeId);
            if (!isExistingBlog)
            {
                ModelState.AddModelError("EmployeeId", "Secdiyiniz employee yoxdu!");
                await ViewsBagEmployeeId();
                return View(vm);
            }
            foreach (var tagId in vm.TagIds)
            {
                var isExistTag = await _context.Tags.AnyAsync(x => x.Id == tagId);
                if (!isExistTag)
                {

                    ModelState.AddModelError("TagIds", "Bele bir tag yoxdur");
                    return View(vm);
                }
            }
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
            existBlog.UpdatedDate = DateTime.UtcNow.AddHours(4); 
            existBlog.Title = vm.Title;
            existBlog.Text = vm.Text;
            existBlog.ImageUrl = vm.ImageUrl;
            existBlog.EmployeeId = vm.EmployeeId;
            existBlog.BlogTags = [];
            foreach (var tagId in vm.TagIds)
            {
                BlogTag blogTag = new()
                {
                    TagId = tagId,
                    BlogId = existBlog.Id
                };
                existBlog.BlogTags.Add(blogTag);
            }
            string folderPath = Path.Combine(_environment.WebRootPath, "assets", "images", "website-images");
            if (vm.Image is { })
            {
                string newImage = await vm.Image.SaveFileAsync(folderPath);
                string existImage = Path.Combine(folderPath, existBlog.ImageUrl);

                ExtensionMethods.DeleteFile(existImage);
                existBlog.ImageUrl = newImage;
            }
            _context.Blogs.Update(existBlog);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        private async Task ViewsBagEmployeeId()
        {
            var emplyees = await _context.Employees.ToListAsync();
            ViewBag.Employees = emplyees;
            var tags = await _context.Tags.ToListAsync();
            ViewBag.Tags = tags;
        }
    }
}
