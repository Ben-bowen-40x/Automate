using Automate.Application.InfrastructureInterfaces;
using Automate.Domain.DiscrepancyAnalysis;
using Automate.Domain.ValueObjects;
using CSharpFunctionalExtensions;

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
    public Result<FileInfo> ManageDiscrepancyAnalysis(string sourceCsv, string reportLoc, string comparisonQuery)
    {
        // Retrieve billed leads via the infrastructure layer
        List<IDiscrepancyCall> billedCalls = _discrepancyService.GetBillableSourceCalls(sourceCsv);

        // Retrieve comparison leads via the infrastructure layer
        List<IDiscrepancyCall> comparisonCalls = _discrepancyService.GetComparisonSourceCalls(comparisonQuery);

        // Match up the calls
        List<MatchingLeads> matches = MatchDiscrepancyCalls.MatchLeads(billedCalls, comparisonCalls);

        // Analyze the matches
        List<DiscrepancyMatch> analyzed = AnalyzeDiscrepancyWithNotePatterns.FindReasoning(matches);

        // Put the matches into a report
        Result<FileInfo> result = _reportService.GenerateDiscrepancyReport(analyzed, reportLoc);

        // Return result
        return result;
    }
}
