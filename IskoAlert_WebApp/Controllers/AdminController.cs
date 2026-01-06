using Microsoft.AspNetCore.Mvc;

namespace IskolarAlert.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Notifications()
        {
            return View();
        }

        public IActionResult ManageIncidents()
        {
            return View();
        }

        public IActionResult ReviewIncident()
        {
            return View();
        }

        public IActionResult ManageLostFound()
        {
            return View();
        }
    }
}
