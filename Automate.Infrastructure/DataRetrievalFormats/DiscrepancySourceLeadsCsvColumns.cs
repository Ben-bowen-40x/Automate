using CsvHelper.Configuration.Attributes;
using Automate.Translation.DiscrepancyTranslate;

namespace Automate.Infrastructure.AnalyzeDiscrepancyService;

public class DiscrepancySourceLeadsCsvColumns : IDiscrepancyCallTranslate
{
    // This class is used to retrieve discrepancy leads from a csv file
    // All leads from the discrepancy list MUST be billable because those are the only leads that are charged
    [Name("Caller Number", "Number")]
    public string? Number { get; set; }
    [Name("Call Date & Time", "Most Recent Call")]
    public string? Date { get; set; }
    [Name("Call Duration", "Longest Call (seconds)")]
    public string? Duration { get; set; }
    [Name("Notes", "Name")]
    public string? Notes { get; set; }
    public string? Billable { get; set; }
}