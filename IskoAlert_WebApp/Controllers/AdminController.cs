using IskoAlert_WebApp.Data;
using IskoAlert_WebApp.Models.Domain.Enums;
using IskoAlert_WebApp.Models.ViewModels.Admin;
using IskoAlert_WebApp.Models.ViewModels.LostFound;
using IskoAlert_WebApp.Services;
using IskoAlert_WebApp.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace IskoAlert_WebApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILostFoundService _lostFoundService;
        private readonly INotificationService _notificationService;

        public AdminController(
            ApplicationDbContext context,
            ILostFoundService lostFoundService,
            INotificationService notificationService)
        {
            _context = context;
            _lostFoundService = lostFoundService;
            _notificationService = notificationService;
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
        public async Task<IActionResult> ManageIncidents(string? keyword, string? credibilityFilter)
        {
            // Base query
            var query = _context.IncidentReports
                .Include(r => r.User)
                .Where(r => r.Status == ReportStatus.Pending && !r.IsAutoProcessed)
                .AsQueryable();

            // Filter by credibility score first (still in database)
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

            // Execute DB query first, then do keyword search in memory
            var reports = (await query
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync()) // fetch filtered reports from DB
                .Where(r =>
                {
                    if (string.IsNullOrWhiteSpace(keyword)) return true;

                    var searchTerm = keyword.ToLower();

                    // Safe client-side search
                    return r.ReportId.ToString().Contains(searchTerm) ||
                           (r.User.Name != null && r.User.Name.ToLower().Contains(searchTerm)) ||
                           r.CampusLocation.ToString().ToLower().Contains(searchTerm) ||
                           r.IncidentType.ToString().ToLower().Contains(searchTerm) ||
                           (r.Title != null && r.Title.ToLower().Contains(searchTerm));
                })
                .ToList();

            // Map to ViewModel
            var viewModel = reports.Select(r => new AdminIncidentReportViewModel
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
            }).ToList();

            ViewBag.CredibilityFilter = credibilityFilter;
            ViewBag.Search = keyword;
            return View(viewModel);
        }



        // ============================================
        // AUTO-PROCESSED REPORTS - Audit trail
        // Shows: Reports that were auto-accepted or auto-rejected
        // Allows admins to review automated decisions
        // ============================================
        public async Task<IActionResult> AutoProcessedReports(string? statusFilter, string? keyword)
        {
            var query = _context.IncidentReports
                .Include(r => r.User)
                .AsQueryable();

            // Filter by status
            if (!string.IsNullOrEmpty(statusFilter) && statusFilter != "All")
            {
                if (Enum.TryParse<ReportStatus>(statusFilter, out var status))
                {
                    query = query.Where(r => r.Status == status);
                }
            }

            // Keyword search
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var searchTerm = keyword.ToLower();
                query = query.Where(r =>
                    r.ReportId.ToString().Contains(searchTerm) ||
                    (r.User.Name != null && r.User.Name.ToLower().Contains(searchTerm)) ||
                    r.CampusLocation.ToString().ToLower().Contains(searchTerm) ||
                    r.IncidentType.ToString().ToLower().Contains(searchTerm) ||
                    (r.Title != null && r.Title.ToLower().Contains(searchTerm))
                );
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
            ViewBag.Search = keyword;
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
            var report = await _context.IncidentReports
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.ReportId == reportId);

            if (report == null)
                return NotFound();

            var oldStatus = report.Status;
            report.UpdateStatus(newStatus);

            await _context.SaveChangesAsync();
            var notificationTitle = $"Incident Report #{reportId} Update";

            var notificationMessage = newStatus switch
            {
                ReportStatus.Rejected => $"Your incident report #{reportId} has been rejected. " +
                                          (!string.IsNullOrEmpty(remarks) ? $"Reason: {remarks}" : "Please review and resubmit with more details."),
                ReportStatus.Accepted => $"Your incident report #{reportId} has been accepted and is now being processed.",
                ReportStatus.InProgress => $"Your incident report #{reportId} status has been updated to \"In-Progress\". We are actively working on this.",
                ReportStatus.Resolved => $"Your incident report #{reportId} has been resolved.",
                _ => $"Your incident report #{reportId} status has been updated from {oldStatus} to {newStatus}."
            };

            // FIX: Just use NotificationType directly (not Models.Domain.Enums.NotificationType)
            var notificationType = newStatus switch
            {
                ReportStatus.Resolved => NotificationType.IncidentResolved,
                ReportStatus.Rejected => NotificationType.IncidentStatusUpdate,
                _ => NotificationType.IncidentStatusUpdate
            };

            await _notificationService.CreateNotificationAsync(
                report.UserId,
                notificationTitle,
                notificationMessage,
                notificationType,
                relatedReportId: reportId
            );

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

        public IActionResult ManageLostFound(string? keyword, string? statusFilter, string? categoryFilter)
        {
            // Start query
            var query = _context.LostFoundItems
                .Include(lf => lf.User)
                .AsQueryable();

            // Bring into memory to safely use .ToString() and string methods
            var data = query.AsEnumerable();

            // Keyword search
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var searchTerm = keyword.ToLower();

                data = data.Where(lf =>
                    (lf.Title != null && lf.Title.ToLower().Contains(searchTerm)) ||
                    (lf.User?.Webmail != null && lf.User.Webmail.ToLower().Contains(searchTerm)) ||
                    lf.LocationFound.ToString().ToLower().Contains(searchTerm)
                );
            }

            // Status filter
            if (!string.IsNullOrEmpty(statusFilter) && statusFilter != "All Status")
            {
                data = data.Where(lf => lf.Status.ToString() == statusFilter);
            }

            // Category filter
            if (!string.IsNullOrEmpty(categoryFilter) && categoryFilter != "All Categories")
            {
                data = data.Where(lf => lf.Category.ToString() == categoryFilter);
            }

            // Project to view model
            var items = data
                .OrderByDescending(lf => lf.DatePosted)
                .Select(lf => new LostFoundItemDisplayViewModel
                {
                    Id = lf.ItemId,
                    Title = lf.Title,
                    Status = lf.Status.ToString(),
                    Category = lf.Category.ToString(),
                    CampusLocation = lf.LocationFound.ToString(),
                    DatePosted = lf.DatePosted,
                    Email = lf.User?.Webmail ?? "N/A",
                    ImagePath = lf.ImagePath
                })
                .ToList();

            // Preserve filters in ViewBag
            ViewBag.Search = keyword;
            ViewBag.StatusFilter = statusFilter;
            ViewBag.CategoryFilter = categoryFilter;

            return View(items);
        }

        // GET: Admin/EditLostFound/5
        public async Task<IActionResult> EditLostFound(int id)
        {
            // Get the item including the reporting user
            var item = await _context.LostFoundItems
                .Include(lf => lf.User)
                .FirstOrDefaultAsync(lf => lf.ItemId == id);

            if (item == null)
                return NotFound();

            // Map entity to EditItem ViewModel
            var model = new EditItem
            {
                ItemId = item.ItemId,
                UserId = item.UserId,
                Title = item.Title,
                Description = item.Description,
                LostOrFound = item.Status,
                SelectedCategory = item.Category,
                SelectedCampusLocation = item.LocationFound,
                Email = item.Email  // Changed from item.User.Webmail to item.Email
            };

            // Current image path for display
            ViewBag.CurrentImagePath = item.ImagePath;

            // Populate dropdowns (if using a helper)
            DropdownHelper.PopulateLostFoundDropdowns(model);

            return View(model);
        }

        // POST: Admin/EditLostFound/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditLostFound(EditItem model)
        {
            if (!ModelState.IsValid)
            {
                // repopulate dropdowns if validation fails
                DropdownHelper.PopulateLostFoundDropdowns(model);
                return View(model);
            }

            try
            {
                int adminUserId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
                string roleUser = User.FindFirstValue(ClaimTypes.Role);

                // Admins can edit any item; non-admins can only edit their own
                if (roleUser != "Admin" && model.UserId != adminUserId)
                    return Forbid();

                // For admins editing other users' items, pass the item's owner ID (model.UserId)
                // For regular users, pass their own ID (adminUserId)
                // The service checks item.UserId == userId, so we pass the owner's ID
                int userIdToPass = roleUser == "Admin" ? model.UserId : adminUserId;

                // Call service to update the item
                await _lostFoundService.UpdateItemAsync(model, userIdToPass);

                TempData["SuccessMessage"] = "Item updated successfully!";
                return RedirectToAction(nameof(ManageLostFound));
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                DropdownHelper.PopulateLostFoundDropdowns(model);
                return View(model);
            }
        }

        // ARCHIVE LOST & FOUND ITEM
        // Allows: Admin to archive any lost & found item
        // ============================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ArchiveLostFound(int id)
        {
            try
            {
                // Get the item to find its owner
                var item = await _context.LostFoundItems
                    .FirstOrDefaultAsync(lf => lf.ItemId == id);

                if (item == null)
                    return NotFound();

                // For admins, pass the item owner's userId (admins can archive any item)
                // The service will call item.Archive(userId) which requires the owner's userId
                await _lostFoundService.ArchiveItemAsync(id, item.UserId);

                TempData["SuccessMessage"] = "Item archived successfully!";
            }
            catch (KeyNotFoundException)
            {
                TempData["ErrorMessage"] = "Item not found.";
            }
            catch (UnauthorizedAccessException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
            }

            return RedirectToAction(nameof(ManageLostFound));
        }

        public IActionResult Notifications()
        {
            return View("~/Views/Admin/Notifications.cshtml");
        }
    }
}