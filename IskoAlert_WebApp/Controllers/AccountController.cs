using Microsoft.AspNetCore.Mvc;

namespace IskoAlert_WebApp.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account/Login
        // Displays the Login Page
        public IActionResult Login()
        {
            return View();
        }

        // POST: Account/Login
        // Handles the actual login logic (checking username/password)
        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            // TODO: Connect this to the database later.
            // For now, we manually check the roles for your prototype.

            if (username == "admin" && password == "admin")
            {
                // Redirect to Admin Dashboard
                return RedirectToAction("Index", "Admin");
            }
            else
            {
                // Default: Redirect to Student Dashboard
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: Account/Register
        // Displays the Registration Page
        public IActionResult Register()
        {
            return View();
        }

        // POST: Account/Register
        // Handles creating a new user
        [HttpPost]
        public IActionResult Register(string email, string password, string confirmPassword)
        {
            // TODO: Save the new user to the database later.

            // After registration, redirect to Login
            return RedirectToAction("Login");
        }

        // POST: Account/Logout
        // Logs the user out
        public IActionResult Logout()
        {
            // TODO: Clear session/cookies later.

            return RedirectToAction("Login");
        }
    }
}
