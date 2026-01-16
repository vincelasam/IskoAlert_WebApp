using System.ComponentModel.DataAnnotations;
using IskoAlert_WebApp.Models.Domain.Enums;

namespace IskoAlert_WebApp.Models.ViewModels.IncidentReport
{
    public class IncidentReportViewModel
    {
        [Required(ErrorMessage = "Incident type is required")]
        public IncidentType IncidentType { get; set; }

        [Required(ErrorMessage = "Campus location is required")]
        [StringLength(150)]
        public string CampusLocation { get; set; } = string.Empty;

        [Required(ErrorMessage = "Title is required")]
        [StringLength(150)]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description is required")]
        [StringLength(500, ErrorMessage = "Maximum of 500 characters only")]
        public string Description { get; set; } = string.Empty;

        // Image PATH only (e.g. /uploads/incidents/img123.png)
        [StringLength(255)]
        public string? ImagePath { get; set; }
    }
}