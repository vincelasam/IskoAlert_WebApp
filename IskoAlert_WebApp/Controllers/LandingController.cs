using Microsoft.AspNetCore.Mvc;

namespace IskolarAlert.Controllers
{
    public class LandingController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
