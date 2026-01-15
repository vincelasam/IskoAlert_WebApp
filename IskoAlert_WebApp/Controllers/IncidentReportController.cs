using IskoAlert_WebApp.Data;
using IskoAlert_WebApp.Models.Domain;
using IskoAlert_WebApp.Models.ViewModels.IncidentReport;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace IskoAlert_WebApp.Controllers
{
    [Authorize] // Restrict access to users with valid university webmail credentials
    public class IncidentReportController : Controller
    {
        private readonly ApplicationDbContext _context;

        public IncidentReportController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /IncidentReport/Index
        // Displays a list of reports submitted by the logged-in user
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();

            int userId = int.Parse(userIdClaim);

            var reports = await _context.IncidentReports
                .Where(ir => ir.UserId == userId)
                .OrderByDescending(ir => ir.CreatedAt)
                .Select(ir => new IncidentReportListViewModel
                {
                    ReportId = ir.ReportId,
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
        // Processes the submission of a new incident report
        [HttpPost]
        [ValidateAntiForgeryToken] // Security: Protect against CSRF attacks
        public async Task<IActionResult> ReportIncident(IncidentReportViewModel model, IFormFile? imageFile)
        {
            if (!ModelState.IsValid) return View(model);

            try
            {
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized();
                int userId = int.Parse(userIdClaim);

                string? savedPath = await HandleImageUpload(imageFile);

                // Domain Model creation using Constructor due to private setters
                var incident = new IncidentReport(
                    userId,
                    model.CampusLocation,
                    model.Title,
                    model.Description,
                    savedPath
                );

                _context.IncidentReports.Add(incident);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Incident report submitted successfully.";
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
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized();

            int userId = int.Parse(userIdClaim);

            var incident = await _context.IncidentReports
                .Include(ir => ir.User)
                .Where(ir => ir.ReportId == id && ir.UserId == userId)
                .Select(ir => new IncidentReportDetailsViewModel
                {
                    ReportId = ir.ReportId,
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