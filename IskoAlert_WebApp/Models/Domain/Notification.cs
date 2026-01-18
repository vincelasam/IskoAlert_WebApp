using IskoAlert_WebApp.Models.Domain.Enums;

namespace IskoAlert_WebApp.Models.Domain
{
    public class Notification
    {
        public int NotificationId { get; private set; }
        public int UserId { get; private set; }
        public User User { get; private set; }
        public string Title { get; private set; }
        public string Message { get; private set; }
        public NotificationType Type { get; private set; }
        public bool IsRead { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public int? RelatedReportId { get; private set; }
        public int? RelatedItemId { get; private set; }

        // EF Core constructor
        private Notification() { }

        public Notification(
            int userId,
            string title,
            string message,
            NotificationType type,
            int? relatedReportId = null,
            int? relatedItemId = null)
        {
            if (userId <= 0)
                throw new ArgumentException("UserId is required.");
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title is required.");
            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentException("Message is required.");

            UserId = userId;
            Title = title;
            Message = message;
            Type = type;
            RelatedReportId = relatedReportId;
            RelatedItemId = relatedItemId;
            IsRead = false;
            CreatedAt = DateTime.UtcNow;
        }

        public void MarkAsRead()
        {
            IsRead = true;
        }
    }
}
