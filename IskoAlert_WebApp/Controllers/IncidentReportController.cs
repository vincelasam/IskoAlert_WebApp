using Microsoft.AspNetCore.Mvc;

namespace IskolarAlert.Controllers  
{
    public class IncidentReportController : Controller
    {
        
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
    }
}
