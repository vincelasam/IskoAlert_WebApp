using Microsoft.AspNetCore.Mvc;

namespace IskoAlert_WebApp.Controllers
{
    public class NotificationController : Controller
    {
        // GET: Notification/Index (For Students)
        // Previously: HomeController.Notifications
        public IActionResult Index()
        {
            return View();
        }

        // GET: Notification/Admin (For Admins)
        // Previously: AdminController.Notifications
        public IActionResult Admin()
        {
            return View();
        }
    }
}
