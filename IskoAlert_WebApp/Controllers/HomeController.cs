using System.Diagnostics;
using IskolarAlert.Models;
using Microsoft.AspNetCore.Mvc;

namespace IskolarAlert.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Notifications()
        {
            return View();
        }

        public IActionResult ReportIncident()
        {
            return View();
        }

        public IActionResult MyReports()
        {
            return View();
        }

        public IActionResult ReportDetails()
        {
            return View();
        }

        public IActionResult LostAndFound()
        {
            return View();
        }

        public IActionResult ReportLostItem()
        {
            return View();
        }    
    
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
