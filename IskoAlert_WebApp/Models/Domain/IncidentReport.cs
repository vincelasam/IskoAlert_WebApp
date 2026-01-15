using IskoAlert_WebApp.Models.Domain.Enums;

namespace IskoAlert_WebApp.Models.Domain
{
    public class IncidentReport
    {
        public int ReportId { get; private set; }

        // Foreign Key pointing to User.UserId
        public int UserId { get; private set; }

        // Navigation Property
        // This links this report to the User entity in the database
        public User User { get; private set; }

        public string CampusLocation { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }
        public string? ImagePath { get; private set; }

        public ReportStatus Status { get; private set; }
        public DateTime CreatedAt { get; private set; }

        // EF Core needs this empty constructor for materialization
        private IncidentReport() { }

        // Constructor to ensure a VALID Incident Report is created
        public IncidentReport(
            int userId, // This must match the UserId from the User class
            string campusLocation,
            string title,
            string description,
            string? imagePath = null)
        {
            if (userId <= 0)
                throw new ArgumentException("A valid UserId is required.");

            if (string.IsNullOrWhiteSpace(campusLocation))
                throw new ArgumentException("Campus location is required.");

            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title is required.");

            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Description is required.");

            UserId = userId;
            CampusLocation = campusLocation;
            Title = title;
            Description = description;
            ImagePath = imagePath;

            Status = ReportStatus.Pending;
            CreatedAt = DateTime.UtcNow;
        }

        // ===== Domain Behaviors =====

        public void UpdateStatus(ReportStatus newStatus)
        {
            Status = newStatus;
        }

        public void UpdateImage(string imagePath)
        {
            ImagePath = imagePath;
        }
    }
}