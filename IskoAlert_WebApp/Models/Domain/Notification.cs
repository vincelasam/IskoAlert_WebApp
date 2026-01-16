using System;

namespace IskoAlert_WebApp.Models.Domain
{
    public class Notification
    {
        public int NotificationId { get; set; }

        // Foreign Keys
        public int UserId { get; set; }
        public virtual User User { get; set; }

        public int? IncidentReportId { get; set; }
        public virtual IncidentReport IncidentReport { get; set; }

        public int? LostFoundItemId { get; set; }
        public virtual LostFoundItem LostFoundItem { get; set; }

        // Content
        public string Message { get; set; }

        // UI Logic
        public bool IsRead { get; set; } = false;

        // Timestamp
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
