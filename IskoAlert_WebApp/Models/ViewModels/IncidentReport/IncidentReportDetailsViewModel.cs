using IskoAlert_WebApp.Models.Domain.Enums;

namespace IskoAlert_WebApp.Models.ViewModels.IncidentReport
{
    public class IncidentReportDetailsViewModel
    {
        public int ReportId { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string CampusLocation { get; set; } = string.Empty;

        public string? ImagePath { get; set; }

        public ReportStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }

        // Reporter info (from User)
        public string ReporterName { get; set; } = string.Empty;
        public string ReporterWebmail { get; set; } = string.Empty;
    }
}
