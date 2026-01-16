using IskoAlert_WebApp.Models;
using IskoAlert_WebApp.Models.Domain;
using IskoAlert_WebApp.Models.Domain.Enums;
using IskoAlert_WebApp.Models.ViewModels.Account;
using IskoAlert_WebApp.Models.ViewModels.LostFound;
using IskoAlert_WebApp.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics;
using System.Security.Claims;

namespace IskoAlert_WebApp.Controllers
{
    [Authorize] 
    public class LostFoundController : Controller
    {
        private readonly ILostFoundService _lostFoundService;

        public LostFoundController(ILostFoundService lostFoundService)
        {
            _lostFoundService = lostFoundService;
        }

        private int GetCurrentUserId()
        {
            var claim = User?.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null)
                throw new UnauthorizedAccessException("User is not authenticated or claim missing.");
            return int.Parse(claim.Value);
        }

        private string GetCurrentUserRole()
        {
            var claim = User?.FindFirst(ClaimTypes.Role);
            if (claim == null)
                throw new UnauthorizedAccessException("User role claim is missing.");
            return claim.Value;
        }

        private void PopulateDropdowns(dynamic model)
        {
            // Category dropdown from ItemCategory enum
            model.Category = Enum.GetValues(typeof(ItemCategory))
                                 .Cast<ItemCategory>()
                                 .Select(c => new SelectListItem
                                 {
                                     Text = c.ToString(),
                                     Value = ((int)c).ToString()
                                 }).ToList();

            // Campus location dropdown from CampusLocation enum
            model.CampusLocations = Enum.GetValues(typeof(CampusLocation))
                                        .Cast<CampusLocation>()
                                        .Select(l => new SelectListItem
                                        {
                                            Text = l.ToString(),
                                            Value = ((int)l).ToString()
                                        }).ToList();
        }

        [HttpGet] //home, kinukuha lahat ng list of items
        public async Task<IActionResult> Index(string? keyword, string? status)
        {
            int userId;
            try
            {
                userId = GetCurrentUserId();
            }
            catch
            {
                return RedirectToAction("Login", "Account");
            }

            var items = await _lostFoundService.GetAllItemsAsync(keyword,status);

            var model = new LostFoundListViewModel
            {
                SearchKeyword = keyword,
                Items = items.Select(x => new LostFoundItemDisplayViewModel
                {
                    Id = x.ItemId,
                    Title = x.Title,
                    DescriptionPreview = x.Description.Length > 100 ? x.Description.Substring(0, 100) + "..." : x.Description,
                    Category = x.Category.ToString(),
                    CampusLocation = x.LocationFound.ToString(),
                    Status = x.Status.ToString(),
                    DatePosted = x.DatePosted,
                    ImagePath = x.ImagePath,
                    Email = x.Email
                }).ToList()
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var item = await _lostFoundService.GetItemByIdAsync(id);
            if (item == null)
                return NotFound();

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

        // Update MyListings to handle archived items
        [HttpGet]
        public async Task<IActionResult> MyListings(bool? showArchived = false)
        {
            int userId;
            try
            {
                userId = GetCurrentUserId();
            }
            catch
            {
                return RedirectToAction("Login", "Account");
            }

            var items = await _lostFoundService.GetUserItemsAsync(userId, showArchived == true);

            var model = new LostFoundListViewModel
            {
                Items = items.Select(x => new LostFoundItemDisplayViewModel
                {
                    Id = x.ItemId,
                    Title = x.Title,
                    DescriptionPreview = x.Description,
                    Category = x.Category.ToString(),
                    CampusLocation = x.LocationFound.ToString(),
                    Status = x.Status.ToString(),
                    DatePosted = x.DatePosted,
                    ImagePath = x.ImagePath
                }).ToList()
            };

            ViewBag.ShowArchived = showArchived == true;
            return View(model);
        }

        [HttpGet]
        public IActionResult ReportLostItem()
        {
            var model = new EditItem // Changed from CreateItem to EditItem
            {
                ItemId = 0, // Set to 0 to indicate create mode
                UserId = 0,
                LostOrFound = ItemStatus.Found // Set default to Found
            };
            PopulateDropdowns(model);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ReportLostItem(EditItem model)
        {
            if (!ModelState.IsValid)
            {
                PopulateDropdowns(model);
                return View(model);
            }

            int userId;
            try
            {
                userId = GetCurrentUserId();
            }
            catch
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                // Convert EditItem to CreateItem for the service
                var createItem = new CreateItem
                {
                    Title = model.Title,
                    Description = model.Description,
                    LostOrFound = model.LostOrFound,
                    Email = model.Email,
                    SelectedCampusLocation = model.SelectedCampusLocation,
                    SelectedCategory = model.SelectedCategory,
                    ImageFile = model.ImageFile
                };

                await _lostFoundService.CreateLostItemAsync(createItem, userId);
                TempData["SuccessMessage"] = "Reporting an Item is successful!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                PopulateDropdowns(model);
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            int userId;
            try
            {
                userId = GetCurrentUserId();
            }
            catch
            {
                return RedirectToAction("Login", "Account");
            }

            var item = await _lostFoundService.GetItemByIdAsync(id);
            if (item == null)
                return NotFound();

            // Check if user owns the item or is admin
            if (item.UserId != userId && GetCurrentUserRole() != "Admin")
                return Forbid();

            var model = new EditItem
            {
                ItemId = item.ItemId,
                UserId = item.UserId,
                Title = item.Title,
                Description = item.Description,
                LostOrFound = item.Status,
                Email = item.Email,
                SelectedCampusLocation = item.LocationFound,
                SelectedCategory = item.Category
            };

            PopulateDropdowns(model);
            ViewBag.CurrentImagePath = item.ImagePath; 
            return View("ReportLostItem", model);// Return ReportLostItem view with EditItem model
        }

        [HttpPost]
        public async Task<IActionResult> UpdateItem(EditItem model)
        {
            if (!ModelState.IsValid)
            {
                PopulateDropdowns(model);
                return View(model);
            }

            int userId;
            string roleUser;

            // Only owner or admin can edit
            try
            {
                userId = GetCurrentUserId();
                roleUser = GetCurrentUserRole();
                if (model.UserId != userId && roleUser != "Admin")
                    return Forbid();
            }
            catch
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                await _lostFoundService.UpdateItemAsync(model, userId);
                TempData["SuccessMessage"] = "Item updated successfully!";
                return RedirectToAction("MyListings");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                PopulateDropdowns(model);
                return View(model);
            }
        }

        // Add Archive POST action
        [HttpPost]
        public async Task<IActionResult> Archive(int id)
        {
            int userId;
            try
            {
                userId = GetCurrentUserId();
            }
            catch
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                await _lostFoundService.ArchiveItemAsync(id, userId);
                TempData["SuccessMessage"] = "Item archived successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction("MyListings");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}
