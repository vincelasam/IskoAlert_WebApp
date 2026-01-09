using System.Diagnostics;
using IskoAlert_WebApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace IskolarAlert.Controllers
{
    public class IncidentReportController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ReportIncident()
        {
            return View();
        }

        public IActionResult ReportDetails()
        {
            return View();
        }

        public IActionResult Feedback()
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
