using IskoAlert_WebApp.Models.Domain.Enums;

namespace IskoAlert_WebApp.Models.ViewModels.Admin
{
    public class AdminDashboardViewModel
    {
        public int TotalReports { get; set; }
        public int PendingReports { get; set; }
        public int InProgressReports { get; set; }
        public int ResolvedReports { get; set; }
        public int AutoAcceptedReports { get; set; }
        public int AutoRejectedReports { get; set; }
        public int ManualReviewRequired { get; set; }
    }

    public class AdminIncidentReportViewModel
    {
        public int ReportId { get; set; }
        public IncidentType IncidentType { get; set; }
        public string Title { get; set; } = string.Empty;
        public string ReporterName { get; set; } = string.Empty;
        public string CampusLocation { get; set; } = string.Empty;
        public ReportStatus Status { get; set; }
        public int CredibilityScore { get; set; }
        public bool IsAutoProcessed { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class AdminReviewIncidentViewModel
    {
        public int ReportId { get; set; }
        public IncidentType IncidentType { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string CampusLocation { get; set; } = string.Empty;
        public string? ImagePath { get; set; }
        public ReportStatus Status { get; set; }
        public int CredibilityScore { get; set; }
        public bool IsAutoProcessed { get; set; }
        public string? AnalysisReason { get; set; }
        public List<string> RedFlags { get; set; } = new();
        public List<string> PositiveSignals { get; set; } = new();
        public DateTime CreatedAt { get; set; }
        public string ReporterName { get; set; } = string.Empty;
        public string ReporterWebmail { get; set; } = string.Empty;

        public string GetCredibilityLevel()
        {
            if (CredibilityScore >= 75) return "High";
            if (CredibilityScore >= 30) return "Medium";
            return "Low";
        }

        public string GetCredibilityColor()
        {
            if (CredibilityScore >= 75) return "green";
            if (CredibilityScore >= 30) return "yellow";
            return "red";
        }
    }
}