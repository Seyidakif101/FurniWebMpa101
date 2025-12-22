using Microsoft.AspNetCore.Mvc;

namespace FurniMpa101.App.Controllers
{
    public class ServicesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
