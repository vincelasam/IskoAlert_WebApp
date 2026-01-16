using IskoAlert_WebApp.Models.Domain;
using IskoAlert_WebApp.Services.Implementations;

namespace IskoAlert_WebApp.Services.Interfaces
{
    public interface ICredibilityAnalyzerService
    {

        /// Analyzes an incident report and returns credibility assessment

        CredibilityAnalysisResult AnalyzeReport(IncidentReport report);
    }
}