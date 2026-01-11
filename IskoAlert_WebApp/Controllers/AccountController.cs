using Microsoft.AspNetCore.Mvc;
using IskoAlert_WebApp.Models.ViewModels.Account; // Import your ViewModel namespace

namespace IskolarAlert.Controllers
{
    public class AccountController : Controller
    {
        // GET: /Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            // Validates the fields based on your RegisterViewModel annotations (Regex, Required, etc.)
            if (!ModelState.IsValid)
            {
                return View(model); // Return the view with validation errors if any
            }

            // TODO: Add logic here to save the user to the database

            // For now, redirect to Login after "successful" registration
            TempData["SuccessMessage"] = "Registration successful! You may now login.";
            return RedirectToAction("Login");
        }

        // GET: /Account/Login
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
