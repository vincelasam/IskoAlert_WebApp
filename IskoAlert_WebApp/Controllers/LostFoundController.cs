using System.Diagnostics;
using IskoAlert_WebApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace IskolarAlert.Controllers
{
    public class LostFoundController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Details() // details ng mga reports
        {
            return View();
        }
        [HttpGet]
        public IActionResult MyListings() //Listings ng mga reports
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
