using IskoAlert_WebApp.Models.Domain;
using IskoAlert_WebApp.Models.Domain.Enums;
using IskoAlert_WebApp.Services.Interfaces;
using System.Text.RegularExpressions;

namespace IskoAlert_WebApp.Services.Implementations
{

    /// Analyzes incident reports to determine credibility and recommend automated actions

    public class CredibilityAnalyzerService : ICredibilityAnalyzerService
    {
        // Credibility thresholds
        private const int AUTO_ACCEPT_THRESHOLD = 75;
        private const int AUTO_REJECT_THRESHOLD = 30;
        // Scores between 30-75 require manual review

        public CredibilityAnalysisResult AnalyzeReport(IncidentReport report)
        {
            int totalScore = 0;
            var flags = new List<string>();
            var positiveSignals = new List<string>();

            // 1. Description Quality Analysis (30 points max)
            var descriptionScore = AnalyzeDescription(report.Description, out var descFlags, out var descPositive);
            totalScore += descriptionScore;
            flags.AddRange(descFlags);
            positiveSignals.AddRange(descPositive);

            // 2. Location Specificity (15 points max)
            var locationScore = AnalyzeLocation(report.CampusLocation, out var locFlags, out var locPositive);
            totalScore += locationScore;
            flags.AddRange(locFlags);
            positiveSignals.AddRange(locPositive);

            // 3. Title Quality (10 points max)
            var titleScore = AnalyzeTitle(report.Title, out var titleFlags, out var titlePositive);
            totalScore += titleScore;
            flags.AddRange(titleFlags);
            positiveSignals.AddRange(titlePositive);

            // 4. Photo Evidence (20 points max)
            var photoScore = AnalyzePhotoEvidence(report.ImagePath, out var photoFlags, out var photoPositive);
            totalScore += photoScore;
            flags.AddRange(photoFlags);
            positiveSignals.AddRange(photoPositive);

            // 5. Report Completeness (15 points max)
            var completenessScore = AnalyzeCompleteness(report, out var compFlags, out var compPositive);
            totalScore += completenessScore;
            flags.AddRange(compFlags);
            positiveSignals.AddRange(compPositive);

            // 6. Spam/Abuse Detection (10 points deduction if detected)
            var spamScore = DetectSpamOrAbuse(report, out var spamFlags);
            totalScore -= spamScore;
            flags.AddRange(spamFlags);

            // Ensure score is within 0-100 range
            totalScore = Math.Max(0, Math.Min(100, totalScore));

            // Determine recommended action
            var recommendedAction = DetermineRecommendedAction(totalScore);
            var requiresReview = totalScore >= AUTO_REJECT_THRESHOLD && totalScore < AUTO_ACCEPT_THRESHOLD;

            return new CredibilityAnalysisResult
            {
                CredibilityScore = totalScore,
                RecommendedAction = recommendedAction,
                RequiresManualReview = requiresReview,
                RedFlags = flags,
                PositiveSignals = positiveSignals,
                AnalysisReason = GenerateAnalysisReason(totalScore, flags, positiveSignals)
            };
        }

        private int AnalyzeDescription(string description, out List<string> flags, out List<string> positive)
        {
            flags = new List<string>();
            positive = new List<string>();
            int score = 0;

            if (string.IsNullOrWhiteSpace(description))
            {
                flags.Add("No description provided");
                return 0;
            }

            // Length check (5 points)
            if (description.Length >= 50 && description.Length <= 500)
            {
                score += 5;
                positive.Add("Appropriate description length");
            }
            else if (description.Length < 20)
            {
                flags.Add("Description too brief (less than 20 characters)");
            }

            // Word count (5 points)
            var wordCount = description.Split(new[] { ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).Length;
            if (wordCount >= 10 && wordCount <= 150)
            {
                score += 5;
                positive.Add($"Good word count ({wordCount} words)");
            }
            else if (wordCount < 5)
            {
                flags.Add($"Very few words ({wordCount})");
            }

            // Specificity indicators (10 points)
            var specificityPatterns = new[]
            {
                @"\d{1,2}:\d{2}",  // Time mentions
                @"(today|yesterday|this morning|this afternoon|last night)",  // Time references
                @"(floor|room|building|area|section)",  // Location details
                @"(person|people|individual|suspect|witness)",  // People mentions
                @"(phone|laptop|wallet|bag|watch|id)",  // Specific items
            };

            int specificityMatches = specificityPatterns.Count(pattern =>
                Regex.IsMatch(description, pattern, RegexOptions.IgnoreCase));

            if (specificityMatches >= 2)
            {
                score += 10;
                positive.Add($"Contains specific details ({specificityMatches} indicators)");
            }
            else if (specificityMatches == 0)
            {
                flags.Add("Lacks specific details (time, location, items)");
            }

            // Check for emotional/exaggerated language (5 points deduction)
            var exaggerationPatterns = new[]
            {
                @"!!!+",
                @"\b(HELP|URGENT|EMERGENCY|DISASTER|TERRIBLE)\b",
                @"(.)\1{4,}",  // Repeated characters
            };

            if (exaggerationPatterns.Any(pattern =>
                Regex.IsMatch(description, pattern, RegexOptions.IgnoreCase)))
            {
                flags.Add("Contains excessive capitalization or punctuation");
                score -= 5;
            }

            // Grammar and coherence (10 points)
            if (HasBasicGrammar(description))
            {
                score += 10;
                positive.Add("Well-structured description");
            }

            return Math.Max(0, score);
        }

        private int AnalyzeLocation(string location, out List<string> flags, out List<string> positive)
        {
            flags = new List<string>();
            positive = new List<string>();
            int score = 0;

            if (string.IsNullOrWhiteSpace(location))
            {
                flags.Add("No location specified");
                return 0;
            }

            // Valid campus locations get full points
            var validLocations = new[]
            {
                "Main Building - West Wing",
                "Main Building - East Wing",
                "Main Building - South Wing",
                "Main Building - North Wing",
                "Library",
                "Gymnasium",
                "Canteen",
                "Lagoon Area",
                "Track Oval"
            };

            if (validLocations.Contains(location, StringComparer.OrdinalIgnoreCase))
            {
                score += 15;
                positive.Add($"Valid specific location: {location}");
            }
            else if (location.Length > 5)
            {
                score += 8;
                positive.Add("Location provided");
            }
            else
            {
                flags.Add("Vague or unclear location");
                score += 3;
            }

            return score;
        }

        private int AnalyzeTitle(string title, out List<string> flags, out List<string> positive)
        {
            flags = new List<string>();
            positive = new List<string>();
            int score = 0;

            if (string.IsNullOrWhiteSpace(title))
            {
                flags.Add("No title provided");
                return 0;
            }

            // Appropriate length (5 points)
            if (title.Length >= 10 && title.Length <= 150)
            {
                score += 5;
                positive.Add("Appropriate title length");
            }
            else if (title.Length < 5)
            {
                flags.Add("Title too short");
            }

            // Not all caps (5 points)
            if (!title.Equals(title.ToUpper()) || title.Length < 10)
            {
                score += 5;
            }
            else
            {
                flags.Add("Title in all caps");
            }

            return score;
        }

        private int AnalyzePhotoEvidence(string? imagePath, out List<string> flags, out List<string> positive)
        {
            flags = new List<string>();
            positive = new List<string>();
            int score = 0;

            if (!string.IsNullOrWhiteSpace(imagePath))
            {
                score += 20;
                positive.Add("Photo evidence provided");
            }
            else
            {
                // No penalty, but note absence
                flags.Add("No photo evidence (optional)");
            }

            return score;
        }

        private int AnalyzeCompleteness(IncidentReport report, out List<string> flags, out List<string> positive)
        {
            flags = new List<string>();
            positive = new List<string>();
            int score = 0;

            // All required fields present (15 points)
            if (!string.IsNullOrWhiteSpace(report.Title) &&
                !string.IsNullOrWhiteSpace(report.Description) &&
                !string.IsNullOrWhiteSpace(report.CampusLocation))
            {
                score += 15;
                positive.Add("All required fields completed");
            }

            return score;
        }

        private int DetectSpamOrAbuse(IncidentReport report, out List<string> flags)
        {
            flags = new List<string>();
            int penaltyPoints = 0;

            // Check for spam indicators
            var spamPatterns = new[]
            {
                @"(test|testing|asdf|qwerty|zzz)",
                @"(click here|buy now|visit|website\.com)",
                @"(.)\1{10,}",  // Excessive character repetition
            };

            var combinedText = $"{report.Title} {report.Description}".ToLower();

            foreach (var pattern in spamPatterns)
            {
                if (Regex.IsMatch(combinedText, pattern, RegexOptions.IgnoreCase))
                {
                    flags.Add("Potential spam/test content detected");
                    penaltyPoints += 15;
                    break;
                }
            }

            // Check for offensive language (basic check)
            var offensivePatterns = new[]
            {
                @"\b(stupid|dumb|idiot)\b"
            };

            if (offensivePatterns.Any(pattern =>
                Regex.IsMatch(combinedText, pattern, RegexOptions.IgnoreCase)))
            {
                flags.Add("Potentially inappropriate language");
                penaltyPoints += 10;
            }

            return penaltyPoints;
        }

        private bool HasBasicGrammar(string text)
        {
            // Basic checks for sentence structure
            var sentences = text.Split(new[] { '.', '!', '?' }, StringSplitOptions.RemoveEmptyEntries);

            if (sentences.Length == 0) return false;

            // Check if most sentences start with capital letter
            int properSentences = sentences.Count(s =>
                s.Trim().Length > 0 && char.IsUpper(s.Trim()[0]));

            return (double)properSentences / sentences.Length >= 0.5;
        }

        private ReportStatus DetermineRecommendedAction(int credibilityScore)
        {
            if (credibilityScore >= AUTO_ACCEPT_THRESHOLD)
                return ReportStatus.Accepted;
            else if (credibilityScore < AUTO_REJECT_THRESHOLD)
                return ReportStatus.Rejected;
            else
                return ReportStatus.Pending;
        }

        private string GenerateAnalysisReason(int score, List<string> flags, List<string> positive)
        {
            var reason = $"Credibility Score: {score}/100\n\n";

            if (score >= AUTO_ACCEPT_THRESHOLD)
            {
                reason += "? AUTO-ACCEPTED: Report appears credible and detailed.\n\n";
            }
            else if (score < AUTO_REJECT_THRESHOLD)
            {
                reason += "? AUTO-REJECTED: Report lacks credibility or appears to be spam/test.\n\n";
            }
            else
            {
                reason += "?? MANUAL REVIEW REQUIRED: Report needs admin evaluation.\n\n";
            }

            if (positive.Any())
            {
                reason += "Positive Indicators:\n";
                reason += string.Join("\n", positive.Select(p => $"• {p}"));
                reason += "\n\n";
            }

            if (flags.Any())
            {
                reason += "Concerns:\n";
                reason += string.Join("\n", flags.Select(f => $"• {f}"));
            }

            return reason;
        }
    }

    public class CredibilityAnalysisResult
    {
        public int CredibilityScore { get; set; }
        public ReportStatus RecommendedAction { get; set; }
        public bool RequiresManualReview { get; set; }
        public List<string> RedFlags { get; set; } = new();
        public List<string> PositiveSignals { get; set; } = new();
        public string AnalysisReason { get; set; } = string.Empty;
    }
}