using IskoAlert_WebApp.Data;
using IskoAlert_WebApp.Models.ViewModels.Dashboard;
using IskoAlert_WebApp.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

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
        // 1. Get logged-in user's email from claims
        var email = User.FindFirstValue(ClaimTypes.Email);
        if (email == null)
            return RedirectToAction("Login", "Account");

        // 2. Get user info
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Webmail == email);
        if (user == null)
            return RedirectToAction("Login", "Account");

        // 3. Count Reports
        var totalReports = await _context.IncidentReports
            .CountAsync(r => r.UserId == user.UserId);

        var pendingReports = await _context.IncidentReports
            .CountAsync(r => r.UserId == user.UserId && r.Status == IskoAlert_WebApp.Models.Domain.Enums.ReportStatus.Pending);

        var inProgressReports = await _context.IncidentReports
            .CountAsync(r => r.UserId == user.UserId && r.Status == IskoAlert_WebApp.Models.Domain.Enums.ReportStatus.InProgress);

        var lostAndFoundCount = await _context.LostFoundItems
            .CountAsync(l => l.Status != IskoAlert_WebApp.Models.Domain.Enums.ItemStatus.Archived);

        // 4. Get recent notifications (last 3)
        var allNotifications = await _notificationService.GetUserNotificationsAsync(user.UserId);
        var recentNotifications = allNotifications.Take(3).ToList();
        var unreadCount = await _notificationService.GetUnreadCountAsync(user.UserId);

        // 5. Build ViewModel
        var dashboardVM = new DashboardViewModel
        {
            FullName = user.Name,
            Role = user.Role,
            TotalReports = totalReports,
            PendingReports = pendingReports,
            InProgressReports = inProgressReports,
            LostAndFoundCount = lostAndFoundCount,
            RecentNotifications = recentNotifications,
            UnreadNotificationCount = unreadCount
        };

        return View(dashboardVM);
    }
}