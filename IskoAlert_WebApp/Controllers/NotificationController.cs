using System.Diagnostics;
using IskoAlert_WebApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace IskolarAlert.Controllers
{
    public class NotificationController : Controller
    {
        public IActionResult Notifications()
        {
            return View();
        }

    }
}
