using IskoAlert_WebApp.Models.Domain.Enums;
using IskoAlert_WebApp.Models.ViewModels.Notification;

namespace IskoAlert_WebApp.Services.Interfaces
{
    public interface INotificationService
    {
        Task<List<NotificationViewModel>> GetUserNotificationsAsync(int userId);
        Task<int> GetUnreadCountAsync(int userId);
        Task MarkAsReadAsync(int notificationId, int userId);
        Task MarkAllAsReadAsync(int userId);
        Task CreateNotificationAsync(
            int userId,
            string title,
            string message,
            NotificationType Type,
            int? relatedReportId = null,
            int? relatedItemId = null);
    }
}