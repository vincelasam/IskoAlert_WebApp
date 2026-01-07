using Microsoft.AspNetCore.Mvc;

namespace IskoAlert_WebApp.Controllers
{
    public class LostFoundController : Controller
    {
        public IActionResult LostAndFound()
        {
            return View();
        }

        public IActionResult ReportLostItem()
        {
            return View();
        }
    }
}
