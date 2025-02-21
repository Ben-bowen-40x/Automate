using Automate.Application.InfrastructureInterfaces;
using Automate.Domain.DiscrepancyAnalysis;
using Automate.Domain.ValueObjects;
using CSharpFunctionalExtensions;

namespace Automate.Application.Discrepancy;

public class TypedDiscrepancyManager(ITypedDiscrepancyService discrepancyService, IReportService reportService) : ITypedDiscrepancyManager
{
    private readonly ITypedDiscrepancyService _discrepancy = discrepancyService;
    private readonly IReportService _reporting = reportService;
    public Result<FileInfo> Manage<T, TComparison>(FileInfo billedCalls, FileInfo comparisonCalls, FileInfo reportLoc) where T : IConvert where TComparison : IConvert
    {
        // Retrieve billed leads
        List<IDiscrepancyCall> billed = _discrepancy.GetCalls<T>(billedCalls);

        // Retrieve comparisons
        List<IDiscrepancyCall> comparison = _discrepancy.GetCalls<TComparison>(comparisonCalls);

        // Match up the calls
        List<IMatchingLeads> matches = MatchDiscrepancyCalls.MatchLeads(billed, comparison);

        // Analyze
        List<DiscrepancyMatch> analyzed = AnalyzeDiscrepancy.FindReasoning(matches);

        // Put the matches into a report
        Result<FileInfo> result = _reporting.GenerateDiscrepancyReport(analyzed, reportLoc.FullName);

        return result;
    }
}
