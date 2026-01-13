using IskoAlert_WebApp.Data; 
using IskoAlert_WebApp.Models.Domain;
using IskoAlert_WebApp.Models.Domain.Enums;
using IskoAlert_WebApp.Models.ViewModels.Account;
using Microsoft.AspNetCore.Mvc;

namespace IskolarAlert.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        // Constructor: Database Access
        public AccountController(ApplicationDbContext context)
        {
            _context = context;
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
            if (ModelState.IsValid)
            {
                // MAPPING: Convert ViewModel to Domain Model
                var newUser = new User(
                    model.IdNumber,
                    model.Webmail,
                    model.Password, 
                    model.FullName,
                    UserRole.Student // Default role for registration
                );

                // SAVE TO DB
                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();

                return RedirectToAction("Login");
            } 
            return View(model);

        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        // This runs when you click the "Sign In" button
        [HttpPost]
        public IActionResult Login(string email, string password, string role)
        {
            // Simple validation: Ensure fields are not empty
            if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password))
            {
                // LOGIC: If the user selects "Student", go to the Dashboard
                if (role == "Student")
                {
                    // Redirects to HomeController -> Index Action (Your Dashboard)
                    return RedirectToAction("Index", "Home");
                }
                else if (role == "Admin")
                {
                    // Redirect to the Admin Controller, Index Action
                    return RedirectToAction("Index", "Admin");
                }
            }

            // If we get here, something was empty or wrong
            ViewData["ErrorMessage"] = "Invalid credentials. Please try again.";
            return View();
        }
        
    }
}