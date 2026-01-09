using Microsoft.AspNetCore.Mvc;

namespace IskolarAlert.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

 
        // GET: /Account/Login
        // This is what runs when you first open the page
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
