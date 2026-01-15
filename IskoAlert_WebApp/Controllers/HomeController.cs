using IskoAlert_WebApp.Data;
using IskoAlert_WebApp.Models.ViewModels.Dashboard;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        // 1. Get logged-in user's email from claims
        var email = User.FindFirstValue(ClaimTypes.Email);
        if (email == null)
            return RedirectToAction("Login", "Account");

        // 2. Get user info
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Webmail == email);
        if (user == null)
            return RedirectToAction("Login", "Account");
        // 3. Count Reports
        //var totalReports = await _context.IncidentReports
        //    .CountAsync(r => r.UserId == user.UserId);

        //var pendingReports = await _context.IncidentReports
        //    .CountAsync(r => r.UserId == user.UserId && r.Status == Models.Domain.Enums.ReportStatus.Pending);

        //var inProgressReports = await _context.IncidentReports
        //    .CountAsync(r => r.UserId == user.UserId && r.Status == Models.Domain.Enums.ReportStatus.InProgress);

        //var lostAndFoundCount = await _context.LostAndFounds
        //    .CountAsync(lf => lf.UserId == user.UserId);

        // 4. Build ViewModel with placeholder numbers
        var dashboardVM = new DashboardViewModel
        {
            FullName = user.Name,
            Role = user.Role,
            TotalReports = 5,       // placeholder
            PendingReports = 2,     // placeholder
            InProgressReports = 1,  // placeholder
            LostAndFoundCount = 3   // placeholder
        };

        return View(dashboardVM);
    }
}
