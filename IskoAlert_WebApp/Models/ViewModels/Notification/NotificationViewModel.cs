using IskoAlert_WebApp.Models.Domain;
using IskoAlert_WebApp.Models.Domain.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace IskoAlert_WebApp.Models.ViewModels
{
    public class NotificationViewModel
{
    public int NotificationId { get; set; }
    public string Message { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }

    public string? IncidentReportTitle { get; set; }
    public string? LostFoundItemTitle { get; set; }

    // NEW: total unread notifications (for badge)
    public int TotalUnread { get; set; } = 0;
    }
}