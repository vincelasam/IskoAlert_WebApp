
using IskoAlert_WebApp.Models.Domain;
using IskoAlert_WebApp.Models.Domain.Enums;
using IskoAlert_WebApp.Models.ViewModels.Account;
using IskoAlert_WebApp.Models.ViewModels.Account;
using IskoAlert_WebApp.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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

                if (user != null)
                {
                    // Create a list of claims representing the authenticated user's data
                    var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Webmail),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

                    // Initialize the identity using the designated cookie authentication scheme
                    var claimsIdentity = new ClaimsIdentity(claims, "CookieAuth");
                    var authProperties = new AuthenticationProperties { IsPersistent = true };

                    // Issue the authentication cookie to the user's browser
                    await HttpContext.SignInAsync("CookieAuth", new ClaimsPrincipal(claimsIdentity), authProperties);

                    if (user.Role == UserRole.Student)
                        return RedirectToAction("Index", "Home");
                    else
                        return RedirectToAction("Index", "Admin");
                }

                // Return to view with an error message if credentials do not match
                ViewData["ErrorMessage"] = "Invalid credentials. Please try again.";
                return View(model);
            }
            catch (Exception ex)
            {
                // Catch and display any business logic exceptions (e.g., account status issues)
                ViewData["ErrorMessage"] = ex.Message;
                return View(model);
            }
        }

    }
}