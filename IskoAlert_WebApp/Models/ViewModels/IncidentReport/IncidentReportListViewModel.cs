using IskoAlert_WebApp.Models.Domain.Enums;

namespace IskoAlert_WebApp.Models.ViewModels.IncidentReport
{
    public class IncidentReportListViewModel
    {
        public int ReportId { get; set; }

        public IncidentType IncidentType { get; set; } // ADDED


        public string Title { get; set; } = string.Empty;

        public string CampusLocation { get; set; } = string.Empty;

        public ReportStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }

        // ===== CREDIBILITY FIELDS =====

        public int CredibilityScore { get; set; }

        public bool IsAutoProcessed { get; set; }

        
        /// Display badge for auto-processed reports
        
        public string ProcessingBadge => IsAutoProcessed ? "Auto" : "Manual";

        
        /// Color coding for credibility score
        
        public string CredibilityBadgeColor
        {
            get
            {
                if (CredibilityScore >= 75) return "bg-green-100 text-green-800 border-green-200";
                if (CredibilityScore >= 30) return "bg-yellow-100 text-yellow-800 border-yellow-200";
                return "bg-red-100 text-red-800 border-red-200";
            }
        }
    }
}