using IskoAlert_WebApp.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IskoAlert_WebApp.Controllers
{
    public class NotificationController : Controller
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        // ===== GET: /Notification =====
        public async Task<IActionResult> Notifications()
        {
            int userId = GetCurrentUserId();
            var notifications = await _notificationService.GetNotificationsAsync(userId);
            return View(notifications);
        }

        // ===== POST: Mark single notification as read =====
        [HttpPost]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            await _notificationService.MarkAsReadAsync(id);
            return Ok(); // Can return JSON for AJAX
        }

        [HttpPost]
        [ValidateAntiForgeryToken] // recommended for form POSTs
        public async Task<IActionResult> MarkAllAsRead()
        {
            int userId = GetCurrentUserId(); // get logged-in user's ID

            // Call service to mark all notifications as read
            await _notificationService.MarkAllAsReadAsync(userId);

            // Redirect back to the same page (Index)
            return RedirectToAction("Notifications");
        }


        // ===== GET: Get total unread count (for badge) =====
        [HttpGet]
        public async Task<IActionResult> UnreadCount()
        {
            int userId = GetCurrentUserId();
            var count = await _notificationService.GetUnreadCountAsync(userId);
            return Json(count);
        }

        // View All page
        public async Task<IActionResult> Notifications()
        {
            int userId = int.Parse(User.FindFirst("UserId")!.Value);

            var notifications = await _notificationService
                .GetRecentNotificationsAsync(userId, 3); // 

            return View(notifications);
        }


        [HttpGet]
        public async Task<IActionResult> UnreadCount()
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            int count = await _notificationService.GetUnreadCountAsync(userId);
            return Json(count); // returns { "value": 3 } kind of response
        }

        // ===== Helper: Get current logged-in user ID =====
        private int GetCurrentUserId()
        {
            // Replace with your actual user ID retrieval logic
            // For example, from claims:
            // return int.Parse(User.FindFirst("UserId")?.Value ?? "0");

            return 1; // placeholder for testing
        }
    }
}
