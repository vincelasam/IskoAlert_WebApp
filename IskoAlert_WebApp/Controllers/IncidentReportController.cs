using IskoAlert_WebApp.Data;
using IskoAlert_WebApp.Models.Domain;
using IskoAlert_WebApp.Models.ViewModels.IncidentReport;
using IskoAlert_WebApp.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json;

namespace IskoAlert_WebApp.Controllers
{
    [Authorize] // Restrict access to users with valid university webmail credentials
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
        // Displays a list of reports submitted by the logged-in user
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (userEmail == null) return Unauthorized();

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Webmail == userEmail);
            if (user == null) return Unauthorized();

            var reports = await _context.IncidentReports
                .Where(ir => ir.UserId == user.UserId)
                .OrderByDescending(ir => ir.CreatedAt)
                .Select(ir => new IncidentReportListViewModel
                {
                    ReportId = ir.ReportId,
                    Title = ir.Title,
                    CampusLocation = ir.CampusLocation,
                    Status = ir.Status,
                    CreatedAt = ir.CreatedAt,
                    CredibilityScore = ir.CredibilityScore,
                    IsAutoProcessed = ir.IsAutoProcessed
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
        // Processes the submission of a new incident report with automated credibility analysis
        [HttpPost]
        [ValidateAntiForgeryToken] // Security: Protect against CSRF attacks
        public async Task<IActionResult> ReportIncident(IncidentReportViewModel model, IFormFile? imageFile)
        {
            if (!ModelState.IsValid) return View(model);

            try
            {
                var userEmail = User.FindFirstValue(ClaimTypes.Email);
                if (string.IsNullOrEmpty(userEmail)) return Unauthorized();

                var user = await _context.Users.FirstOrDefaultAsync(u => u.Webmail == userEmail);
                if (user == null) return Unauthorized();

                string? savedPath = await HandleImageUpload(imageFile);

                // Domain Model creation using Constructor due to private setters
                var incident = new IncidentReport(
                    user.UserId,
                    model.CampusLocation,
                    model.Title,
                    model.Description,
                    savedPath
                );

                // ===== AUTOMATED CREDIBILITY ANALYSIS =====
                var analysisResult = _credibilityAnalyzer.AnalyzeReport(incident);

                // Set the credibility analysis on the incident
                incident.SetCredibilityAnalysis(
                    credibilityScore: analysisResult.CredibilityScore,
                    isAutoProcessed: !analysisResult.RequiresManualReview,
                    analysisReason: analysisResult.AnalysisReason,
                    redFlags: JsonSerializer.Serialize(analysisResult.RedFlags),
                    positiveSignals: JsonSerializer.Serialize(analysisResult.PositiveSignals)
                );

                // Update status based on analysis
                incident.UpdateStatus(analysisResult.RecommendedAction);

                _context.IncidentReports.Add(incident);
                await _context.SaveChangesAsync();

                // Provide feedback to user based on analysis result
                if (analysisResult.RecommendedAction == Models.Domain.Enums.ReportStatus.Accepted)
                {
                    TempData["SuccessMessage"] = "? Your incident report has been automatically accepted and forwarded to campus security. You will receive updates via email.";
                    TempData["SuccessDetails"] = $"Credibility Score: {analysisResult.CredibilityScore}/100";
                }
                else if (analysisResult.RecommendedAction == Models.Domain.Enums.ReportStatus.Rejected)
                {
                    TempData["WarningMessage"] = "?? Your report could not be automatically processed. It appears to lack sufficient detail or credibility. Please ensure all information is accurate and detailed.";
                    TempData["WarningDetails"] = "If you believe this is an error, please contact campus security directly.";
                }
                else
                {
                    TempData["InfoMessage"] = "?? Your report is pending review. An administrator will evaluate it shortly.";
                    TempData["InfoDetails"] = $"Credibility Score: {analysisResult.CredibilityScore}/100 - Manual review required.";
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
        // Displays full details and status of a specific report
        [HttpGet]
        public async Task<IActionResult> ReportDetails(int id)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmail)) return Unauthorized();

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Webmail == userEmail);
            if (user == null) return Unauthorized();

            var incident = await _context.IncidentReports
                .Include(ir => ir.User)
                .Where(ir => ir.ReportId == id && ir.UserId == user.UserId)
                .FirstOrDefaultAsync();

            if (incident == null) return NotFound();

            // Ensure User is loaded
            if (incident.User == null) return NotFound("User information not available");

            var viewModel = new IncidentReportDetailsViewModel
            {
                ReportId = incident.ReportId,
                Title = incident.Title,
                Description = incident.Description,
                CampusLocation = incident.CampusLocation,
                ImagePath = incident.ImagePath,
                Status = incident.Status,
                CreatedAt = incident.CreatedAt,
                ReporterName = incident.User.Name,
                ReporterWebmail = incident.User.Webmail,
                CredibilityScore = incident.CredibilityScore,
                IsAutoProcessed = incident.IsAutoProcessed,
                AnalysisReason = incident.AnalysisReason,
                RedFlags = string.IsNullOrEmpty(incident.RedFlags)
                    ? new List<string>()
                    : JsonSerializer.Deserialize<List<string>>(incident.RedFlags) ?? new List<string>(),
                PositiveSignals = string.IsNullOrEmpty(incident.PositiveSignals)
                    ? new List<string>()
                    : JsonSerializer.Deserialize<List<string>>(incident.PositiveSignals) ?? new List<string>()
            };

            return View(viewModel);
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