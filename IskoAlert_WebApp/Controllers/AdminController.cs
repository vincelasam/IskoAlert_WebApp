using IskoAlert_WebApp.Data;
using IskoAlert_WebApp.Models.Domain.Enums;
using IskoAlert_WebApp.Models.ViewModels.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IskolarAlert.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Admin/Index
        // Dashboard showing automation metrics
        public async Task<IActionResult> Index()
        {
            var dashboard = new AdminDashboardViewModel
            {
                TotalReports = await _context.IncidentReports.CountAsync(),
                PendingReports = await _context.IncidentReports
                    .CountAsync(r => r.Status == ReportStatus.Pending),
                InProgressReports = await _context.IncidentReports
                    .CountAsync(r => r.Status == ReportStatus.InProgress),
                ResolvedReports = await _context.IncidentReports
                    .CountAsync(r => r.Status == ReportStatus.Resolved),

                // SEMI-AUTOMATION metrics
                AutoAcceptedReports = await _context.IncidentReports
                    .CountAsync(r => r.Status == ReportStatus.Accepted && r.IsAutoProcessed),
                AutoRejectedReports = await _context.IncidentReports
                    .CountAsync(r => r.Status == ReportStatus.Rejected && r.IsAutoProcessed),
                ManualReviewRequired = await _context.IncidentReports
                    .CountAsync(r => r.Status == ReportStatus.Pending && !r.IsAutoProcessed)
            };

            return View(dashboard);
        }

        // ============================================
        // MANUAL REVIEW QUEUE - Reports that need admin attention
        // Shows: Pending reports with score 30-74 (not auto-processed)
        // ============================================
        public async Task<IActionResult> ManageIncidents(string? credibilityFilter)
        {
            var query = _context.IncidentReports
                .Include(r => r.User)
                .Where(r => r.Status == ReportStatus.Pending && !r.IsAutoProcessed)
                .AsQueryable();

            // Filter by credibility score range
            if (!string.IsNullOrEmpty(credibilityFilter))
            {
                switch (credibilityFilter)
                {
                    case "High":
                        query = query.Where(r => r.CredibilityScore >= 75);
                        break;
                    case "Medium":
                        query = query.Where(r => r.CredibilityScore >= 30 && r.CredibilityScore < 75);
                        break;
                    case "Low":
                        query = query.Where(r => r.CredibilityScore < 30);
                        break;
                }
            }

            var reports = await query
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => new AdminIncidentReportViewModel
                {
                    ReportId = r.ReportId,
                    IncidentType = r.IncidentType,
                    Title = r.Title,
                    ReporterName = r.User.Name,
                    CampusLocation = r.CampusLocation,
                    Status = r.Status,
                    CredibilityScore = r.CredibilityScore,
                    IsAutoProcessed = r.IsAutoProcessed,
                    CreatedAt = r.CreatedAt
                })
                .ToListAsync();

            ViewBag.CredibilityFilter = credibilityFilter;
            return View(reports);
        }

        // ============================================
        // AUTO-PROCESSED REPORTS - Audit trail
        // Shows: Reports that were auto-accepted or auto-rejected
        // Allows admins to review automated decisions
        // ============================================
        public async Task<IActionResult> AutoProcessedReports(string? statusFilter)
        {
            var query = _context.IncidentReports
                .Include(r => r.User)
                .Where(r => r.IsAutoProcessed)
                .AsQueryable();

            // Filter by status (Accepted or Rejected)
            if (!string.IsNullOrEmpty(statusFilter))
            {
                if (statusFilter == "Accepted")
                    query = query.Where(r => r.Status == ReportStatus.Accepted);
                else if (statusFilter == "Rejected")
                    query = query.Where(r => r.Status == ReportStatus.Rejected);
            }

            var reports = await query
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => new AdminIncidentReportViewModel
                {
                    ReportId = r.ReportId,
                    IncidentType = r.IncidentType,
                    Title = r.Title,
                    ReporterName = r.User.Name,
                    CampusLocation = r.CampusLocation,
                    Status = r.Status,
                    CredibilityScore = r.CredibilityScore,
                    IsAutoProcessed = r.IsAutoProcessed,
                    CreatedAt = r.CreatedAt
                })
                .ToListAsync();

            ViewBag.StatusFilter = statusFilter;
            return View(reports);
        }

        // ============================================
        // REVIEW INDIVIDUAL REPORT - Detailed view
        // Shows: Full credibility analysis with red flags, positive signals
        // Allows: Admin to change status manually
        // ============================================
        [HttpGet]
        public async Task<IActionResult> ReviewIncident(int id)
        {
            var incident = await _context.IncidentReports
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.ReportId == id);

            if (incident == null)
                return NotFound();

            var viewModel = new AdminReviewIncidentViewModel
            {
                ReportId = incident.ReportId,
                IncidentType = incident.IncidentType,
                Title = incident.Title,
                Description = incident.Description,
                CampusLocation = incident.CampusLocation,
                ImagePath = incident.ImagePath,
                Status = incident.Status,
                CredibilityScore = incident.CredibilityScore,
                IsAutoProcessed = incident.IsAutoProcessed,
                AnalysisReason = incident.AnalysisReason,
                RedFlags = incident.GetRedFlagsList(),
                PositiveSignals = incident.GetPositiveSignalsList(),
                CreatedAt = incident.CreatedAt,
                ReporterName = incident.User.Name,
                ReporterWebmail = incident.User.Webmail
            };

            return View(viewModel);
        }

        // ============================================
        // UPDATE REPORT STATUS - Manual override
        // Allows: Admin to manually change report status
        // Used for: Overriding automation or processing manual reviews
        // ============================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateReportStatus(int reportId, ReportStatus newStatus, string? remarks)
        {
            var report = await _context.IncidentReports.FindAsync(reportId);
            if (report == null)
                return NotFound();

            report.UpdateStatus(newStatus);

            // TODO: If you want to save remarks, you can add them to AnalysisReason or create a separate AdminRemarks field

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Report #{reportId} status updated to {newStatus}";
            return RedirectToAction(nameof(ReviewIncident), new { id = reportId });
        }

        // ============================================
        // BULK ACCEPT PENDING - Quick action
        // Accepts: All pending reports in manual review queue
        // Use case: Admin reviews the queue and decides to accept all
        // ============================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BulkAcceptPending()
        {
            var pendingReports = await _context.IncidentReports
                .Where(r => r.Status == ReportStatus.Pending && !r.IsAutoProcessed)
                .ToListAsync();

            foreach (var report in pendingReports)
            {
                report.UpdateStatus(ReportStatus.Accepted);
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"{pendingReports.Count} reports accepted!";
            return RedirectToAction(nameof(ManageIncidents));
        }

        public IActionResult ManageLostFound()
        {
            return View();
        }

        public IActionResult Notifications()
        {
            return View("~/Views/Admin/Notifications.cshtml");
        }
    }
}