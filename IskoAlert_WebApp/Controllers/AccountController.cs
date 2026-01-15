    
using IskoAlert_WebApp.Models.Domain;
using IskoAlert_WebApp.Models.Domain.Enums;
using IskoAlert_WebApp.Models.ViewModels.Account;
using Microsoft.AspNetCore.Mvc;
using IskoAlert_WebApp.Models.ViewModels.Account;
using IskoAlert_WebApp.Services.Interfaces;

namespace IskolarAlert.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService _userService; 
        
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
                return View(model); 
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
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var user = await _userService.ValidateCredentialsAsync(model.Email, model.Password, Enum.Parse<UserRole>(model.Role));

                if (user == null)
                {
                    ViewData["ErrorMessage"] = "Invalid credentials. Please try again.";
                    return View();
                }

                if (user.Role == UserRole.Student)
                    return RedirectToAction("Index", "Home");
                else
                    return RedirectToAction("Index", "Admin");
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Message;
                return View();
            }
        }

    }
}