using Microsoft.AspNetCore.Mvc;

namespace FurniMpa101.App.Controllers
{
    public class BlogController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
