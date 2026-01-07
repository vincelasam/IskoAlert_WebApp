using Microsoft.AspNetCore.Mvc;

namespace IskoAlert_WebApp.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: Account/Login
        // UPDATED: Now accepts 'email' instead of 'username' to match your View
        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            // Simple check for the prototype
            // We check if the email contains "admin" to simulate an admin login
            if (!string.IsNullOrEmpty(email) && email.ToLower().Contains("admin"))
            {
                return RedirectToAction("Index", "Admin");
            }
            else
            {
                // Everyone else goes to Student Dashboard
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: Account/Register
        public IActionResult Register()
        {
            return View();
        }

        public IActionResult Logout()
        {
            return RedirectToAction("Login");
        }
    }
}
