using IskoAlert_WebApp.Data;
using IskoAlert_WebApp.Models.Domain.Enums;
using IskoAlert_WebApp.Models.ViewModels.Dashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

[Authorize]
public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly INotificationService _notificationService;

    public HomeController(ApplicationDbContext context, INotificationService notificationService)
    {
        _context = context;
        _notificationService = notificationService;
    }

    public async Task<IActionResult> Index()
    {
        
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
        {
            return RedirectToAction("Login", "Account");
        }

        
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return RedirectToAction("Login", "Account");

       
        var totalReports = await _context.IncidentReports.CountAsync(r => r.UserId == userId);
        var pendingReports = await _context.IncidentReports
            .CountAsync(r => r.UserId == userId && r.Status == ReportStatus.Pending);
        var inProgressReports = await _context.IncidentReports
            .CountAsync(r => r.UserId == userId && r.Status == ReportStatus.InProgress);
        var lostAndFoundCount = await _context.LostFoundItems.CountAsync(lf => lf.ArchivedAt == null);

  
        var dashboardVM = new DashboardViewModel
        {
            FullName = user.Name,
            Role = user.Role,
            TotalReports = totalReports,
            PendingReports = pendingReports,
            InProgressReports = inProgressReports,
            LostAndFoundCount = lostAndFoundCount,
            // Huwag kalimutan itong i-populate!
            RecentNotifications = await _notificationService.GetRecentNotificationsViewModelAsync(userId, 5)
        };

        return View(dashboardVM);
    }
}