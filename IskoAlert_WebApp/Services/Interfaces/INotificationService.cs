using IskoAlert_WebApp.Models.ViewModels;
using IskoAlert_WebApp.Models.Domain;

public interface INotificationService
{
    Task<List<Notification>> GetNotificationsAsync(int userId); // all
    Task<List<Notification>> GetRecentNotificationsAsync(int userId, int take); // latest N
    Task<List<NotificationViewModel>> GetRecentNotificationsViewModelAsync(int userId, int take); // new!
    Task MarkAllAsReadAsync(int userId);
    Task<int> GetUnreadCountAsync(int userId);
    Task MarkAsReadAsync(int notificationId);
}
