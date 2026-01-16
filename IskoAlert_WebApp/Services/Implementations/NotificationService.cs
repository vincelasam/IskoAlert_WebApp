using IskoAlert_WebApp.Data;
using IskoAlert_WebApp.Models.Domain;
using IskoAlert_WebApp.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

public class NotificationService : INotificationService
{
    private readonly ApplicationDbContext _context;
    public NotificationService(ApplicationDbContext context) => _context = context;

    public async Task<List<Notification>> GetNotificationsAsync(int userId)
        => await _context.Notifications
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();

    public async Task<List<Notification>> GetRecentNotificationsAsync(int userId, int take)
        => await _context.Notifications
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .Take(take)
            .ToListAsync();

    // NEW: Returns NotificationViewModel with total unread
    public async Task<List<NotificationViewModel>> GetRecentNotificationsViewModelAsync(int userId, int take)
    {
        var notifications = await GetRecentNotificationsAsync(userId, take);
        var totalUnread = await GetUnreadCountAsync(userId);

        return notifications.Select(n => new NotificationViewModel
        {
            NotificationId = n.NotificationId,
            Message = n.Message,
            IsRead = n.IsRead,
            CreatedAt = n.CreatedAt,
            IncidentReportTitle = n.IncidentReport?.Title,
            LostFoundItemTitle = n.LostFoundItem?.Title,
            TotalUnread = totalUnread
        }).ToList();
    }

    public async Task MarkAllAsReadAsync(int userId)
    {
        var unread = await _context.Notifications
            .Where(n => n.UserId == userId && !n.IsRead)
            .ToListAsync();

        foreach (var n in unread)
            n.IsRead = true;

        await _context.SaveChangesAsync();
    }

    public async Task MarkAsReadAsync(int notificationId)
    {
        var notif = await _context.Notifications.FindAsync(notificationId);
        if (notif != null)
        {
            notif.IsRead = true;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<int> GetUnreadCountAsync(int userId)
        => await _context.Notifications.CountAsync(n => n.UserId == userId && !n.IsRead);
}
