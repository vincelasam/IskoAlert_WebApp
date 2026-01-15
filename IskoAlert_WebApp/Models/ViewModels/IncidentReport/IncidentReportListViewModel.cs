using IskoAlert_WebApp.Models.Domain.Enums;

namespace IskoAlert_WebApp.Models.ViewModels.IncidentReport
{
    public class IncidentReportListViewModel
    {
        public int ReportId { get; set; }

        public string Title { get; set; } = string.Empty;

        public string CampusLocation { get; set; } = string.Empty;

        public ReportStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
