using IskoAlert_WebApp.Models.Domain.Enums;

namespace IskoAlert_WebApp.Models.ViewModels.IncidentReport
{
    public class IncidentReportDetailsViewModel
    {
        public int ReportId { get; set; }

        public IncidentType IncidentType { get; set; } // ADDED


        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string CampusLocation { get; set; } = string.Empty;

        public string? ImagePath { get; set; }

        public ReportStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? AcceptedAt { get; set; }
        public DateTime? InProgressAt { get; set; }
        public DateTime? ResolvedAt { get; set; }
        public DateTime? RejectedAt { get; set; }

        // Reporter info (from User)
        public string ReporterName { get; set; } = string.Empty;
        public string ReporterWebmail { get; set; } = string.Empty;

        // ===== CREDIBILITY ANALYSIS FIELDS =====

        
        /// Credibility score (0-100)

        public int CredibilityScore { get; set; }

        public bool IsAutoProcessed { get; set; }

        public string? AnalysisReason { get; set; }


        /// List of red flags identified
 
        public List<string> RedFlags { get; set; } = new();


        /// List of positive signals identified
        public List<string> PositiveSignals { get; set; } = new();

        /// Helper property to determine credibility level for UI display
        public string CredibilityLevel
        {
            get
            {
                if (CredibilityScore >= 75) return "High";
                if (CredibilityScore >= 30) return "Medium";
                return "Low";
            }
        }


        /// Helper property for color coding in UI
        public string CredibilityColorClass
        {
            get
            {
                if (CredibilityScore >= 75) return "text-green-600 bg-green-50";
                if (CredibilityScore >= 30) return "text-yellow-600 bg-yellow-50";
                return "text-red-600 bg-red-50";
            }
        }
    }
}