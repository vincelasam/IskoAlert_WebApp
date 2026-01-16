using IskoAlert_WebApp.Models;
using IskoAlert_WebApp.Models.ViewModels.Account;
using IskoAlert_WebApp.Models.ViewModels.LostFound;
using IskoAlert_WebApp.Services.Implementations;
using IskoAlert_WebApp.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace IskolarAlert.Controllers
{
    public class LostFoundController : Controller
    {

        private readonly ILostFoundService _lostFoundService;

        public LostFoundController(ILostFoundService lostFoundService)
        {
            _lostFoundService = lostFoundService;
        }

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

        [HttpGet]
        public IActionResult ReportLostItem()
        {
            return View();
        }

        //Reporting of Lost Item
        [HttpPost]
        public async Task<IActionResult> CreateLostItemAsync(CreateItem model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                await _lostFoundService.CreateLostItemAsync(model);

                TempData["SuccessMessage"] = "Reporting an Item is successful!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)  // Handle business logic errors
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
