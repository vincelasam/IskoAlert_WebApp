using IskoAlert_WebApp.Models.Domain;
using IskoAlert_WebApp.Models.Domain.Enums;
using IskoAlert_WebApp.Services.Interfaces;
using System.Text.RegularExpressions;

namespace IskoAlert_WebApp.Services.Implementations
{

    /// Result of credibility analysis for an incident report

    public class CredibilityAnalysisResult
    {
        public int CredibilityScore { get; set; }
        public bool RequiresManualReview { get; set; }
        public string AnalysisReason { get; set; } = string.Empty;
        public List<string> RedFlags { get; set; } = new();
        public List<string> PositiveSignals { get; set; } = new();
        public ReportStatus RecommendedAction { get; set; }
    }


    /// Service that analyzes incident reports for credibility
    ///  Better spam detection, quality checks, and meaningful content validation

    public class CredibilityAnalyzerService : ICredibilityAnalyzerService
    {
        // Scoring thresholds
        private const int AUTO_ACCEPT_THRESHOLD = 90;
        private const int AUTO_REJECT_THRESHOLD = 20;

        public CredibilityAnalysisResult AnalyzeReport(IncidentReport report)
        {
            var result = new CredibilityAnalysisResult
            {
                CredibilityScore = 40, // Start at neutral score
                RequiresManualReview = false,
                RedFlags = new List<string>(),
                PositiveSignals = new List<string>()
            };

            // === CRITICAL SPAM CHECKS (Run First) ===
            // These can immediately disqualify a report
            if (DetectSpam(report, result))
            {
                // Spam detected - force low score regardless of other factors
                result.CredibilityScore = Math.Min(result.CredibilityScore, 25);
            }

            // === ANALYSIS RULES ===

            // 1. Description Length Analysis
            AnalyzeDescriptionLength(report, result);

            // 2. Description Quality Analysis (IMPROVED)
            AnalyzeDescriptionQuality(report, result);

            // 3. Photo Evidence Analysis
            AnalyzePhotoEvidence(report, result);


            // 4. Incident Type Consistency
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

        
        /// NEW: Detects spam and low-quality submissions
        /// Returns true if spam is detected
       
        private bool DetectSpam(IncidentReport report, CredibilityAnalysisResult result)
        {
            var description = report.Description ?? string.Empty;
            bool isSpam = false;

            // 1. Repeated character spam (like "1111111..." or "aaaaaaa...")
            if (HasRepeatedCharacters(description, 10))
            {
                result.CredibilityScore -= 40;
                result.RedFlags.Add("Description contains repeated characters (possible spam)");
                isSpam = true;
            }

            // 2. Single word repeated (like "test test test test...")
            if (HasRepeatedWords(description, 5))
            {
                result.CredibilityScore -= 35;
                result.RedFlags.Add("Description contains repeated words (possible spam)");
                isSpam = true;
            }

            // 3. No spaces or punctuation (like "thisisnotarealsentence")
            if (description.Length > 30 && !description.Contains(" "))
            {
                result.CredibilityScore -= 30;
                result.RedFlags.Add("Description lacks proper spacing");
                isSpam = true;
            }

            // 4. Too many special characters
            var specialCharCount = description.Count(c => !char.IsLetterOrDigit(c) && !char.IsWhiteSpace(c));
            if (description.Length > 20 && specialCharCount > description.Length * 0.3)
            {
                result.CredibilityScore -= 25;
                result.RedFlags.Add("Excessive special characters detected");
                isSpam = true;
            }

            // 5. Common test/spam phrases
            var spamPhrases = new[] { "test", "asdf", "qwerty", "lorem ipsum", "asdfjkl" };
            if (spamPhrases.Any(phrase => description.ToLower().Contains(phrase)))
            {
                result.CredibilityScore -= 20;
                result.RedFlags.Add("Contains common test/spam phrases");
                isSpam = true;
            }

            // 6. No vowels (like "bcdfghjkl...")
            var vowelCount = description.Count(c => "aeiouAEIOU".Contains(c));
            if (description.Length > 20 && vowelCount < description.Length * 0.1)
            {
                result.CredibilityScore -= 25;
                result.RedFlags.Add("Unusual character distribution (possible gibberish)");
                isSpam = true;
            }

            return isSpam;
        }

        
        /// NEW: Checks if text has many repeated characters
       
        private bool HasRepeatedCharacters(string text, int threshold)
        {
            if (string.IsNullOrEmpty(text) || text.Length < threshold) return false;

            int maxConsecutive = 1;
            int currentConsecutive = 1;

            for (int i = 1; i < text.Length; i++)
            {
                if (text[i] == text[i - 1])
                {
                    currentConsecutive++;
                    maxConsecutive = Math.Max(maxConsecutive, currentConsecutive);
                }
                else
                {
                    currentConsecutive = 1;
                }
            }

            return maxConsecutive >= threshold;
        }

        
        /// NEW: Checks if text has many repeated words
       
        private bool HasRepeatedWords(string text, int threshold)
        {
            if (string.IsNullOrEmpty(text)) return false;

            var words = text.ToLower().Split(new[] { ' ', '.', ',', '!', '?' }, StringSplitOptions.RemoveEmptyEntries);
            if (words.Length < 2) return false;

            var wordGroups = words.GroupBy(w => w);
            int maxRepeats = wordGroups.Max(g => g.Count());

            return maxRepeats >= threshold;
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

        /// IMPROVED: Analyzes the quality and specificity of the description
        /// Now checks for meaningful content, not just keywords

        private void AnalyzeDescriptionQuality(IncidentReport report, CredibilityAnalysisResult result)
        {
            var description = report.Description?.ToLower() ?? string.Empty;

            // NEW: Check for minimum word count (meaningful sentences)
            var words = description.Split(new[] { ' ', '.', ',', '!', '?' }, StringSplitOptions.RemoveEmptyEntries);
            var uniqueWords = words.Distinct().Count();

            if (uniqueWords < 5)
            {
                result.CredibilityScore -= 20;
                result.RedFlags.Add("Very few unique words (lacks detail)");
            }
            else if (uniqueWords >= 15)
            {
                result.CredibilityScore += 10;
                result.PositiveSignals.Add("Good vocabulary diversity");
            }

            // Check for specific details (time, numbers, specific terms)
            bool hasTimeReference = description.Contains("am") || description.Contains("pm") ||
                                   description.Contains("morning") || description.Contains("afternoon") ||
                                   description.Contains("evening") || description.Contains("night") ||
                                   Regex.IsMatch(description, @"\d{1,2}:\d{2}"); // Time format like "2:30"

            bool hasColorDescription = description.Contains("black") || description.Contains("white") ||
                                       description.Contains("red") || description.Contains("blue") ||
                                       description.Contains("green") || description.Contains("yellow") ||
                                       description.Contains("brown") || description.Contains("gray");

            bool hasQuantityOrSize = Regex.IsMatch(description, @"\d+") ||
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

       

        /// Checks if the incident type matches common patterns in description
        /// Uses IncidentType enum instead of Title

        private void AnalyzeIncidentType(IncidentReport report, CredibilityAnalysisResult result)
        {
            var description = report.Description?.ToLower() ?? string.Empty;

            // Check for consistency between IncidentType enum and description
            bool isConsistent = false;

            switch (report.IncidentType)
            {
                case IncidentType.Theft:
                    isConsistent = description.Contains("stolen") || description.Contains("missing") ||
                                  description.Contains("took") || description.Contains("theft") ||
                                  description.Contains("stole");
                    break;

                case IncidentType.Hazard:
                    isConsistent = description.Contains("hazard") || description.Contains("danger") ||
                                  description.Contains("unsafe") || description.Contains("risk") ||
                                  description.Contains("broken") || description.Contains("slippery");
                    break;

                case IncidentType.MedicalEmergency:
                    isConsistent = description.Contains("medical") || description.Contains("emergency") ||
                                  description.Contains("injury") || description.Contains("injured") ||
                                  description.Contains("hurt") || description.Contains("sick") ||
                                  description.Contains("unconscious") || description.Contains("collapsed");
                    break;

                case IncidentType.Accident:
                    isConsistent = description.Contains("accident") || description.Contains("crash") ||
                                  description.Contains("collision") || description.Contains("fell") ||
                                  description.Contains("tripped") || description.Contains("slipped");
                    break;

                case IncidentType.Others:
                    // For "Others", we don't penalize lack of consistency since it's a catch-all
                    isConsistent = true;
                    break;
            }

            if (isConsistent)
            {
                result.CredibilityScore += 10;
                result.PositiveSignals.Add($"Description matches incident type ({report.IncidentType})");
            }
            else if (report.IncidentType != IncidentType.Others)
            {
                result.CredibilityScore -= 5;
                result.RedFlags.Add($"Description may not match selected incident type ({report.IncidentType})");
            }
        }
    }
}