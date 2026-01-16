using IskoAlert_WebApp.Models.Domain.Enums;

namespace IskoAlert_WebApp.Models.Domain
{
    public class IncidentReport
    {
        public int ReportId { get; private set; }

        // Foreign Key pointing to User.UserId
        public int UserId { get; private set; }

        // Navigation Property
        public User User { get; private set; }

        public IncidentType IncidentType { get; private set; }
        public string CampusLocation { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }
        public string? ImagePath { get; private set; }

        public ReportStatus Status { get; private set; }

        // SEMI-AUTOMATION: Credibility Analysis Fields
        public int CredibilityScore { get; private set; }
        public bool IsAutoProcessed { get; private set; }
        public string? AnalysisReason { get; private set; }
        public string? RedFlags { get; private set; }
        public string? PositiveSignals { get; private set; }

        public DateTime CreatedAt { get; private set; }

        // EF Core needs this empty constructor for materialization
        private IncidentReport() { }

        // Constructor to ensure a VALID Incident Report is created
        public IncidentReport(
            int userId,
            IncidentType incidentType,
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
            IncidentType = incidentType;
            CampusLocation = campusLocation;
            Title = title;
            Description = description;
            ImagePath = imagePath;

            // Initial status - will be updated after credibility analysis
            Status = ReportStatus.Pending;

            // Credibility fields initialized to defaults
            // These will be set by ApplyCredibilityAnalysis method
            CredibilityScore = 0;
            IsAutoProcessed = false;

            CreatedAt = DateTime.UtcNow;
        }

        // ===== Domain Behaviors =====


        /// Applies the results of credibility analysis to this report
        /// This is called by the controller after analyzing the report

        public void ApplyCredibilityAnalysis(
            int credibilityScore,
            bool isAutoProcessed,
            ReportStatus recommendedStatus,
            string analysisReason,
            List<string> redFlags,
            List<string> positiveSignals)
        {
            CredibilityScore = credibilityScore;
            IsAutoProcessed = isAutoProcessed;
            Status = recommendedStatus;
            AnalysisReason = analysisReason;

            // Store lists as JSON for easy parsing later
            RedFlags = redFlags.Any()
                ? System.Text.Json.JsonSerializer.Serialize(redFlags)
                : null;
            PositiveSignals = positiveSignals.Any()
                ? System.Text.Json.JsonSerializer.Serialize(positiveSignals)
                : null;
        }

        /// Gets red flags as a list (deserializes from JSON)

        public List<string> GetRedFlagsList()
        {
            if (string.IsNullOrEmpty(RedFlags))
                return new List<string>();

            try
            {
                return System.Text.Json.JsonSerializer.Deserialize<List<string>>(RedFlags)
                       ?? new List<string>();
            }
            catch
            {
                return new List<string>();
            }
        }


        /// Gets positive signals as a list (deserializes from JSON)

        public List<string> GetPositiveSignalsList()
        {
            if (string.IsNullOrEmpty(PositiveSignals))
                return new List<string>();

            try
            {
                return System.Text.Json.JsonSerializer.Deserialize<List<string>>(PositiveSignals)
                       ?? new List<string>();
            }
            catch
            {
                return new List<string>();
            }
        }

        public void UpdateStatus(ReportStatus newStatus)
        {
            Status = newStatus;
        }

        public void UpdateImage(string imagePath)
        {
            ImagePath = imagePath;
        }


        /// Checks if this report requires manual admin review

        public bool RequiresManualReview()
        {
            return Status == ReportStatus.Pending && !IsAutoProcessed;
        }


        /// Checks if this report was auto-accepted

        public bool WasAutoAccepted()
        {
            return Status == ReportStatus.Accepted && IsAutoProcessed;
        }


        /// Checks if this report was auto-rejected

        public bool WasAutoRejected()
        {
            return Status == ReportStatus.Rejected && IsAutoProcessed;
        }
    }
}