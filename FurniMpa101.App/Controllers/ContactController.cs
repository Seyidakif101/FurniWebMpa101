using Microsoft.AspNetCore.Mvc;

namespace FurniMpa101.App.Controllers
{
    public class ContactController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
