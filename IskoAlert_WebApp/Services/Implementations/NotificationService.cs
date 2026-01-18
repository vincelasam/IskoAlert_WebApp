using IskoAlert_WebApp.Data;
using IskoAlert_WebApp.Models.Domain;
using IskoAlert_WebApp.Models.Domain.Enums;
using IskoAlert_WebApp.Models.ViewModels.Notification;
using IskoAlert_WebApp.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IskoAlert_WebApp.Services.Implementations
{
    public class NotificationService : INotificationService
    {
        private readonly ApplicationDbContext _context;

        public NotificationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<NotificationViewModel>> GetUserNotificationsAsync(int userId)
        {
            var notifications = await _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .Select(n => new NotificationViewModel
                {
                    NotificationId = n.NotificationId,
                    Title = n.Title,
                    Message = n.Message,
                    Type = n.Type,
                    IsRead = n.IsRead,
                    CreatedAt = n.CreatedAt,
                    RelatedReportId = n.RelatedReportId,
                    RelatedItemId = n.RelatedItemId
                })
                .ToListAsync();

            return notifications;
        }

        public async Task<int> GetUnreadCountAsync(int userId)
        {
            return await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .CountAsync();
        }

        public async Task MarkAsReadAsync(int notificationId, int userId)
        {
            var notification = await _context.Notifications
                .FirstOrDefaultAsync(n => n.NotificationId == notificationId && n.UserId == userId);

            if (notification == null)
                throw new KeyNotFoundException("Notification not found.");

            notification.MarkAsRead();
            await _context.SaveChangesAsync();
        }

        public async Task MarkAllAsReadAsync(int userId)
        {
            var notifications = await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .ToListAsync();

            foreach (var notification in notifications)
            {
                notification.MarkAsRead();
            }

            await _context.SaveChangesAsync();
        }

        public async Task CreateNotificationAsync(
            int userId,
            string title,
            string message,
            NotificationType type,
            int? relatedReportId = null,
            int? relatedItemId = null)
        {
            var notification = new Notification(
                userId: userId,
                title: title,
                message: message,
                type: type,
                relatedReportId: relatedReportId,
                relatedItemId: relatedItemId);

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
        }
    }
}