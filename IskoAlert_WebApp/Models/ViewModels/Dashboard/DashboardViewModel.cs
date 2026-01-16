using IskoAlert_WebApp.Models.Domain.Enums;

namespace IskoAlert_WebApp.Models.ViewModels.Dashboard
{
    public class DashboardViewModel
    {
        public string FullName { get; set; }
        public UserRole Role { get; set; }
        public int TotalReports { get; set; }
        public int PendingReports { get; set; }
        public int InProgressReports { get; set; }
        public int LostAndFoundCount { get; set; }

        //public List<Notification> RecentNotifications { get; set; }
    }
}
