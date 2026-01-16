using IskoAlert_WebApp.Data;
using IskoAlert_WebApp.Models.Domain;
using IskoAlert_WebApp.Models.Domain.Enums;
using IskoAlert_WebApp.Models.ViewModels.IncidentReport;
using IskoAlert_WebApp.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace IskoAlert_WebApp.Controllers
{
    [Authorize]
    public class IncidentReportController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ICredibilityAnalyzerService _credibilityAnalyzer;

        public IncidentReportController(
            ApplicationDbContext context,
            ICredibilityAnalyzerService credibilityAnalyzer)
        {
            _context = context;
            _credibilityAnalyzer = credibilityAnalyzer;
        }

        // GET: /IncidentReport/Index
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return RedirectToAction("Login", "Account");

            int userId = int.Parse(userIdClaim);

            var reports = await _context.IncidentReports
                .Where(ir => ir.UserId == userId)
                .OrderByDescending(ir => ir.CreatedAt)
                .Select(ir => new IncidentReportListViewModel
                {
                    ReportId = ir.ReportId,
                    IncidentType = ir.IncidentType,
                    Title = ir.Title,
                    CampusLocation = ir.CampusLocation,
                    Status = ir.Status,
                    CreatedAt = ir.CreatedAt
                })
                .ToListAsync();

            return View(reports);
        }

        // GET: /IncidentReport/ReportIncident
        [HttpGet]
        public IActionResult ReportIncident()
        {
            return View(new IncidentReportViewModel());
        }

        // POST: /IncidentReport/ReportIncident
        // WITH SEMI-AUTOMATION: Analyzes credibility and auto-accepts/rejects
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReportIncident(IncidentReportViewModel model, IFormFile? imageFile)
        {
            if (!ModelState.IsValid) return View(model);

            try
            {
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userIdClaim)) return RedirectToAction("Login", "Account");
                int userId = int.Parse(userIdClaim);

                string? savedPath = await HandleImageUpload(imageFile);

                // Create incident report
                var incident = new IncidentReport(
                    userId,
                    model.IncidentType,
                    model.CampusLocation,
                    model.Title,
                    model.Description,
                    savedPath
                );

                // SEMI-AUTOMATION: Analyze credibility
                var analysisResult = _credibilityAnalyzer.AnalyzeReport(incident);

                // Apply analysis results to the report
                incident.ApplyCredibilityAnalysis(
                    credibilityScore: analysisResult.CredibilityScore,
                    isAutoProcessed: !analysisResult.RequiresManualReview,
                    recommendedStatus: analysisResult.RecommendedAction,
                    analysisReason: analysisResult.AnalysisReason,
                    redFlags: analysisResult.RedFlags,
                    positiveSignals: analysisResult.PositiveSignals
                );

                // Save to database
                _context.IncidentReports.Add(incident);
                await _context.SaveChangesAsync();

                // USER EXPERIENCE: User sees standard success message regardless of automation
                // They are NOT aware if it was auto-accepted, auto-rejected, or needs review

                if (analysisResult.CredibilityScore < 30)
                {
                    // Score < 30 (auto-rejected): Success message with helpful tip
                    TempData["SuccessMessage"] = "Report submitted for review. Tip: Adding more details and photos helps us respond faster to your report.";
                }
                else
                {
                    // Score >= 30: Standard success message
                    TempData["SuccessMessage"] = "Incident report submitted successfully. You can track its status in 'My Reports'.";
                }

                return RedirectToAction(nameof(Index));
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }

        // GET: /IncidentReport/ReportDetails/{id}
        [HttpGet]
        public async Task<IActionResult> ReportDetails(int id)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim)) return RedirectToAction("Login", "Account");

            int userId = int.Parse(userIdClaim);

            var incident = await _context.IncidentReports
                .Include(ir => ir.User)
                .Where(ir => ir.ReportId == id && ir.UserId == userId)
                .Select(ir => new IncidentReportDetailsViewModel
                {
                    ReportId = ir.ReportId,
                    IncidentType = ir.IncidentType,
                    Title = ir.Title,
                    Description = ir.Description,
                    CampusLocation = ir.CampusLocation,
                    ImagePath = ir.ImagePath,
                    Status = ir.Status,
                    CreatedAt = ir.CreatedAt,
                    ReporterName = ir.User.Name,
                    ReporterWebmail = ir.User.Webmail
                })
                .FirstOrDefaultAsync();

            if (incident == null) return NotFound();

            return View(incident);
        }

        // Helper Method: Handles Physical Storage and Validation
        private async Task<string?> HandleImageUpload(IFormFile? imageFile)
        {
            if (imageFile == null || imageFile.Length == 0) return null;

            // Format validation: JPG and PNG only
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            var extension = Path.GetExtension(imageFile.FileName).ToLower();
            if (!allowedExtensions.Contains(extension))
                throw new ArgumentException("Only JPG and PNG formats are allowed.");

            // Size validation: Max 2MB
            if (imageFile.Length > 2 * 1024 * 1024)
                throw new ArgumentException("Image size must not exceed 2MB.");

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "incidents");
            if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            return "/uploads/incidents/" + fileName;
        }
    }
}