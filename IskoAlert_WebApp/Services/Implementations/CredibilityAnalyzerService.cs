using IskoAlert_WebApp.Models.Domain;
using IskoAlert_WebApp.Models.Domain.Enums;
using IskoAlert_WebApp.Services.Interfaces;

namespace IskoAlert_WebApp.Services.Implementations
{
    /// <summary>
    /// Result of credibility analysis for an incident report
    /// </summary>
    public class CredibilityAnalysisResult
    {
        public int CredibilityScore { get; set; }
        public bool RequiresManualReview { get; set; }
        public string AnalysisReason { get; set; } = string.Empty;
        public List<string> RedFlags { get; set; } = new();
        public List<string> PositiveSignals { get; set; } = new();
        public ReportStatus RecommendedAction { get; set; }
    }

    /// <summary>
    /// Service that analyzes incident reports for credibility
    /// Uses rule-based scoring to determine if reports should be auto-accepted, rejected, or manually reviewed
    /// </summary>
    public class CredibilityAnalyzerService : ICredibilityAnalyzerService
    {
        // Scoring thresholds
        private const int AUTO_ACCEPT_THRESHOLD = 75;
        private const int AUTO_REJECT_THRESHOLD = 30;

        public CredibilityAnalysisResult AnalyzeReport(IncidentReport report)
        {
            var result = new CredibilityAnalysisResult
            {
                CredibilityScore = 50, // Start at neutral score
                RequiresManualReview = false,
                RedFlags = new List<string>(),
                PositiveSignals = new List<string>()
            };

            // === ANALYSIS RULES ===

            // 1. Description Length Analysis
            AnalyzeDescriptionLength(report, result);

            // 2. Description Quality Analysis
            AnalyzeDescriptionQuality(report, result);

            // 3. Photo Evidence Analysis
            AnalyzePhotoEvidence(report, result);

            // 4. Location Specificity Analysis
            AnalyzeLocationSpecificity(report, result);

            // 5. Incident Type Consistency
            AnalyzeIncidentType(report, result);

            // === DETERMINE FINAL SCORE AND ACTION ===

            // Ensure score is between 0 and 100
            result.CredibilityScore = Math.Max(0, Math.Min(100, result.CredibilityScore));

            // Determine recommended action based on score
            if (result.CredibilityScore >= AUTO_ACCEPT_THRESHOLD)
            {
                result.RecommendedAction = ReportStatus.Accepted;
                result.RequiresManualReview = false;
                result.AnalysisReason = $"AUTOMATED ACCEPTANCE (Score: {result.CredibilityScore}/100)\n\n" +
                    "This report has been automatically accepted based on high credibility indicators:\n" +
                    "- Detailed and specific description\n" +
                    "- Clear location information\n" +
                    "- Evidence provided (if applicable)\n\n" +
                    "The report has been forwarded to campus security for immediate action.";
            }
            else if (result.CredibilityScore < AUTO_REJECT_THRESHOLD)
            {
                result.RecommendedAction = ReportStatus.Rejected;
                result.RequiresManualReview = false;
                result.AnalysisReason = $"AUTOMATED REJECTION (Score: {result.CredibilityScore}/100)\n\n" +
                    "This report lacks sufficient detail or credibility indicators:\n" +
                    string.Join("\n", result.RedFlags.Select(f => $"- {f}")) + "\n\n" +
                    "Please provide more specific information and resubmit, or contact campus security directly " +
                    "if this is an urgent matter.";
            }
            else
            {
                result.RecommendedAction = ReportStatus.Pending;
                result.RequiresManualReview = true;
                result.AnalysisReason = $"MANUAL REVIEW REQUIRED (Score: {result.CredibilityScore}/100)\n\n" +
                    "This report requires administrator evaluation due to mixed credibility signals.\n\n" +
                    "Positive Indicators:\n" +
                    (result.PositiveSignals.Any()
                        ? string.Join("\n", result.PositiveSignals.Select(s => $"- {s}"))
                        : "- None identified\n") + "\n\n" +
                    "Concerns:\n" +
                    (result.RedFlags.Any()
                        ? string.Join("\n", result.RedFlags.Select(f => $"- {f}"))
                        : "- None identified\n") + "\n\n" +
                    "An administrator will review this report within 24 hours.";
            }

            return result;
        }


        /// Analyzes the length of the description

        private void AnalyzeDescriptionLength(IncidentReport report, CredibilityAnalysisResult result)
        {
            var descriptionLength = report.Description?.Length ?? 0;

            if (descriptionLength < 20)
            {
                result.CredibilityScore -= 25;
                result.RedFlags.Add("Very brief description (less than 20 characters)");
            }
            else if (descriptionLength < 50)
            {
                result.CredibilityScore -= 10;
                result.RedFlags.Add("Short description (less than 50 characters)");
            }
            else if (descriptionLength >= 100)
            {
                result.CredibilityScore += 15;
                result.PositiveSignals.Add("Detailed description provided");
            }
            else if (descriptionLength >= 50)
            {
                result.CredibilityScore += 5;
                result.PositiveSignals.Add("Adequate description length");
            }
        }


        /// Analyzes the quality and specificity of the description

        private void AnalyzeDescriptionQuality(IncidentReport report, CredibilityAnalysisResult result)
        {
            var description = report.Description?.ToLower() ?? string.Empty;

            // Check for specific details (time, numbers, specific terms)
            bool hasTimeReference = description.Contains("am") || description.Contains("pm") ||
                                   description.Contains("morning") || description.Contains("afternoon") ||
                                   description.Contains("evening") || description.Contains("night");

            bool hasColorDescription = description.Contains("black") || description.Contains("white") ||
                                       description.Contains("red") || description.Contains("blue") ||
                                       description.Contains("green") || description.Contains("yellow") ||
                                       description.Contains("brown") || description.Contains("gray");

            bool hasQuantityOrSize = System.Text.RegularExpressions.Regex.IsMatch(description, @"\d+") ||
                                     description.Contains("large") || description.Contains("small") ||
                                     description.Contains("medium");

            // Check for vague language
            bool hasVagueLanguage = description.Contains("maybe") || description.Contains("perhaps") ||
                                   description.Contains("not sure") || description.Contains("think");

            // Positive signals
            int specificityCount = 0;
            if (hasTimeReference)
            {
                specificityCount++;
                result.PositiveSignals.Add("Includes time reference");
            }
            if (hasColorDescription)
            {
                specificityCount++;
                result.PositiveSignals.Add("Includes color/visual details");
            }
            if (hasQuantityOrSize)
            {
                specificityCount++;
                result.PositiveSignals.Add("Includes specific quantities or measurements");
            }

            result.CredibilityScore += specificityCount * 5;

            // Negative signals
            if (hasVagueLanguage)
            {
                result.CredibilityScore -= 10;
                result.RedFlags.Add("Contains uncertain or vague language");
            }

            // Check for ALL CAPS (might indicate spam or low-quality report)
            if (description.Length > 20 && description == description.ToUpper())
            {
                result.CredibilityScore -= 15;
                result.RedFlags.Add("Excessive use of capital letters");
            }
        }


        /// Analyzes whether photo evidence was provided

        private void AnalyzePhotoEvidence(IncidentReport report, CredibilityAnalysisResult result)
        {
            if (!string.IsNullOrEmpty(report.ImagePath))
            {
                result.CredibilityScore += 20;
                result.PositiveSignals.Add("Photo evidence provided");
            }
            else
            {
                // No penalty for missing photo, but note it
                result.RedFlags.Add("No photo evidence provided");
            }
        }


        /// Analyzes the specificity of the location

        private void AnalyzeLocationSpecificity(IncidentReport report, CredibilityAnalysisResult result)
        {
            var location = report.CampusLocation ?? string.Empty;

            if (string.IsNullOrWhiteSpace(location))
            {
                result.CredibilityScore -= 20;
                result.RedFlags.Add("No location specified");
            }
            else if (location.Contains("-")) // e.g., "Main Building - West Wing"
            {
                result.CredibilityScore += 10;
                result.PositiveSignals.Add("Specific location provided (building and wing/area)");
            }
            else
            {
                result.CredibilityScore += 5;
                result.PositiveSignals.Add("General location provided");
            }
        }


        /// Checks if the incident type matches common patterns

        private void AnalyzeIncidentType(IncidentReport report, CredibilityAnalysisResult result)
        {
            var title = report.Title?.ToLower() ?? string.Empty;
            var description = report.Description?.ToLower() ?? string.Empty;

            // Check for consistency between title and description
            bool isConsistent = false;

            switch (title)
            {
                case "theft":
                    isConsistent = description.Contains("stolen") || description.Contains("missing") ||
                                  description.Contains("took") || description.Contains("theft");
                    break;
                case "vandalism":
                    isConsistent = description.Contains("damaged") || description.Contains("vandal") ||
                                  description.Contains("graffiti") || description.Contains("destroyed");
                    break;
                case "harassment":
                    isConsistent = description.Contains("harass") || description.Contains("threaten") ||
                                  description.Contains("bully") || description.Contains("intimidat");
                    break;
                case "property damage":
                    isConsistent = description.Contains("broken") || description.Contains("damaged") ||
                                  description.Contains("crack") || description.Contains("destroy");
                    break;
                case "safety hazard":
                    isConsistent = description.Contains("hazard") || description.Contains("danger") ||
                                  description.Contains("unsafe") || description.Contains("risk");
                    break;
            }

            if (isConsistent)
            {
                result.CredibilityScore += 10;
                result.PositiveSignals.Add("Incident type matches description content");
            }
            else if (title != "other")
            {
                result.CredibilityScore -= 5;
                result.RedFlags.Add("Incident type may not match description");
            }
        }
    }
}