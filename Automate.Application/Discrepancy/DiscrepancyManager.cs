using Automate.Application.InfrastructureInterfaces;
using Automate.Domain.DiscrepancyAnalysis;
using Automate.Domain.ValueObjects;

namespace Automate.Application.Discrepancy;

public class DiscrepancyManager(IDiscrepancyService discrepancyService, IReportService reportService) : IDiscrepancyManager
{
    private readonly IDiscrepancyService _discrepancyService = discrepancyService;
    private readonly IReportService _reportService = reportService;

    /// <summary>
    /// If <paramref name="reportLoc"/> <see cref="string"/> or the <paramref name="comparisonQuery"/> <see cref="string"/> equal <see cref="string.Empty"/>, then a default will be used instead
    /// </summary>
    /// <param name="sourceCsv"></param>
    /// <param name="reportLoc"></param>
    /// <param name="comparisonQuery"></param>
    /// <returns></returns>
    public Dictionary<bool, FileInfo> ManageDiscrepancyAnalysis(string sourceCsv, string reportLoc, string comparisonQuery)
    {
        // Retrieve billed leads via the infrastructure layer
        List<DiscrepancyCall> billedCalls = _discrepancyService.GetBillableSourceCalls(sourceCsv);

        // Retrieve comparison leads via the infrastructure layer
        List<DiscrepancyCall> comparisonCalls = _discrepancyService.GetComparisonSourceCalls(comparisonQuery);

        // Match up the calls
        var matches = MatchDiscrepancyCalls.MatchLeads(billedCalls, comparisonCalls);

        // Analyze the matches
        List<DiscrepancyMatch> analyzed = AnalyzeDiscrepancyWithNotePatterns.FindReasoning(matches);

        // Put the matches into a report
        bool result = _reportService.GenerateDiscrepancyReport(analyzed, out FileInfo file, reportLoc);

        // Return result
        return new() { { result, file } };
    }
}
