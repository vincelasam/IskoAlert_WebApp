using IskoAlert_WebApp.Models;
using IskoAlert_WebApp.Models.ViewModels.Account;
using IskoAlert_WebApp.Models.ViewModels.LostFound;
using IskoAlert_WebApp.Services.Implementations;
using IskoAlert_WebApp.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace IskolarAlert.Controllers
{
    [Authorize]
    public class LostFoundController : Controller
    {

        private readonly ILostFoundService _lostFoundService;

        public LostFoundController(ILostFoundService lostFoundService)
        {
            _lostFoundService = lostFoundService;
        }

        private int CurrentUserId => int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id) // details ng mga reports
        {
            var item = await _lostFoundService.GetItemByIdAsync(id);
            if (item == null)
            {
                return NotFound(); // item not found
            }

            var model = new LostFoundItemDisplayViewModel
            {
                Id = item.ItemId,
                Title = item.Title,
                DescriptionPreview = item.Description,
                Category = item.Category.ToString(),
                CampusLocation = item.LocationFound.ToString(),
                Status = item.Status.ToString(),
                DatePosted = item.DatePosted,
                ImagePath = item.ImagePath,
                Email = item.Email
            };

            return View(model);
        }


        [HttpGet]   
         public async Task<IActionResult> MyListings()
        {
            var items = await _lostFoundService.GetUserItemsAsync(CurrentUserId);

            var model = new LostFoundListViewModel
            {
                Items = items.Select(x => new LostFoundItemDisplayViewModel
                {
                    Id = x.ItemId,
                    Title = x.Title,
                    DescriptionPreview = x.Description, // or short version
                    Category = x.Category.ToString(),
                    CampusLocation = x.LocationFound.ToString(),
                    Status = x.Status.ToString(),
                    DatePosted = x.DatePosted,
                    ImagePath = x.ImagePath
                }).ToList()
            };

            return View(model);
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
                await _lostFoundService.CreateLostItemAsync(model, CurrentUserId);

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
