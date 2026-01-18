// IskoAlert_WebApp/Controllers/NotificationController.cs
using IskoAlert_WebApp.Models.ViewModels.Notification;
using IskoAlert_WebApp.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IskoAlert_WebApp.Data;
using System.Security.Claims;

namespace IskolarAlert.Controllers
{
    [Authorize]
    public class NotificationController : Controller
    {
        private readonly INotificationService _notificationService;
        private readonly ApplicationDbContext _context;

        public NotificationController(INotificationService notificationService, ApplicationDbContext context)
        {
            _notificationService = notificationService;
            _context = context;
        }

        public async Task<IActionResult> Notifications()
        {
            var userId = await GetCurrentUserIdAsync();
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var notifications = await _notificationService.GetUserNotificationsAsync(userId.Value);
            var unreadCount = await _notificationService.GetUnreadCountAsync(userId.Value);

            var viewModel = new NotificationListViewModel
            {
                Notifications = notifications,
                UnreadCount = unreadCount
            };

            return View(viewModel);
        }

        [Authorize(Roles = "Admin")]  // Changed from [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> AdminNotifications()
        {
            var userId = await GetCurrentUserIdAsync();
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var notifications = await _notificationService.GetUserNotificationsAsync(userId.Value);
            var unreadCount = await _notificationService.GetUnreadCountAsync(userId.Value);

            var viewModel = new NotificationListViewModel
            {
                Notifications = notifications,
                UnreadCount = unreadCount
            };

            return View("~/Views/Admin/Notifications.cshtml", viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> MarkAsRead(int notificationId)
        {
            var userId = await GetCurrentUserIdAsync();
            if (userId == null)
                return Unauthorized();

            try
            {
                await _notificationService.MarkAsReadAsync(notificationId, userId.Value);
                return Ok();
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPost]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var userId = await GetCurrentUserIdAsync();
            if (userId == null)
                return Unauthorized();

            await _notificationService.MarkAllAsReadAsync(userId.Value);
            return RedirectToAction("Notifications");
        }

        private async Task<int?> GetCurrentUserIdAsync()
        {
            // Get email from claims (set during login)
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(email))
                return null;

            // Get user from database using email
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Webmail == email);

            return user?.UserId;
        }
    }
}