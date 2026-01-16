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
        // Nullable to satisfy EF Core's requirement - will be populated when loaded from database
        public User? User { get; private set; }

        public string CampusLocation { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }
        public string? ImagePath { get; private set; }

        public ReportStatus Status { get; private set; }
        public DateTime CreatedAt { get; private set; }

        // ===== CREDIBILITY TRACKING FIELDS =====

   
        /// Credibility score from 0-100 determined by automated analysis

        public int CredibilityScore { get; private set; }

   
        /// Indicates if this report was automatically processed (accepted/rejected)

        public bool IsAutoProcessed { get; private set; }

   
        /// Detailed explanation of the credibility analysis

        public string? AnalysisReason { get; private set; }

   
        /// Red flags identified during analysis (stored as JSON array)

        public string? RedFlags { get; private set; }

   
        /// Positive signals identified during analysis (stored as JSON array)

        public string? PositiveSignals { get; private set; }

        // EF Core needs this empty constructor for materialization
        // The null-forgiving operator (!) is used because EF Core will populate these values
        private IncidentReport()
        {
            CampusLocation = null!;
            Title = null!;
            Description = null!;
        }

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

            // Initialize credibility fields
            CredibilityScore = 0;
            IsAutoProcessed = false;
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

   
        /// Sets the credibility analysis results for this report

        public void SetCredibilityAnalysis(
            int credibilityScore,
            bool isAutoProcessed,
            string analysisReason,
            string? redFlags = null,
            string? positiveSignals = null)
        {
            if (credibilityScore < 0 || credibilityScore > 100)
                throw new ArgumentException("Credibility score must be between 0 and 100.");

            CredibilityScore = credibilityScore;
            IsAutoProcessed = isAutoProcessed;
            AnalysisReason = analysisReason;
            RedFlags = redFlags;
            PositiveSignals = positiveSignals;
        }

   
        /// Marks the report as requiring manual review

        public void RequireManualReview()
        {
            IsAutoProcessed = false;
            Status = ReportStatus.Pending;
        }
    }
}