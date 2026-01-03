using Microsoft.AspNetCore.Mvc;

namespace IskolarAlert.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
