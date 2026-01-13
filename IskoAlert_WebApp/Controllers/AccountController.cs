using IskoAlert_WebApp.Data; 
using IskoAlert_WebApp.Models.Domain;
using IskoAlert_WebApp.Models.Domain.Enums;
using IskoAlert_WebApp.Models.ViewModels.Account;
using Microsoft.AspNetCore.Mvc;
using IskoAlert_WebApp.Models.ViewModels.Account; // Import your ViewModel namespace

namespace IskolarAlert.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService _userService;  // ← Inject service
        
        public AccountController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);  // Return with validation errors
            }
            
            try
            {
                await _userService.RegisterAsync(model);
                
                TempData["SuccessMessage"] = "Registration successful!";
                return RedirectToAction("Login");
            }
            catch (Exception ex)  // Handle business logic errors
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        public IActionResult Login(string email, string password, string role)
        {
            if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password))
            {
                if (role == "Student")
                {
                    return RedirectToAction("Index", "Home");
                }
                else if (role == "Admin")
                {
                    return RedirectToAction("Index", "Admin");
                }
            }

            ViewData["ErrorMessage"] = "Invalid credentials. Please try again.";
            return View();
        }
        
    }
}