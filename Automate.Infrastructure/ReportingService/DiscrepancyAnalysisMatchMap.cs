using CsvHelper.Configuration;
using Automate.Domain.ValueObjects;

namespace Automate.Infrastructure.ReportingService;

public class DiscrepancyAnalysisMatchMap : ClassMap<DiscrepancyMatch>
{
    public DiscrepancyAnalysisMatchMap()
    {
        int index = 0;
        // Billed Lead
        Map(m => m.MatchingLeads.BilledLead.Number.Number).Index(index++).Name("Lead Phone");
        Map(m => m.MatchingLeads.BilledLead.Date).Index(index++).Name("Lead Date");
        Map(m => m.MatchingLeads.BilledLead.Duration).Index(index++).Name("Lead Duration");
        Map(m => m.MatchingLeads.BilledLead.Notes).Index(index++).Name("Lead Note");

        // Comparison lead
        Map(m => m.MatchingLeads.ComparisonLead.Number.Number).Index(index++).Name("Record Number");
        Map(m => m.MatchingLeads.ComparisonLead.Date).Index(index++).Name("Record Date");
        Map(m => m.MatchingLeads.ComparisonLead.Duration).Index(index++).Name("Record Duration");
        Map(m => m.MatchingLeads.ComparisonLead.Billable).Index(index++).Name("Record Billability");
        Map(m => m.MatchingLeads.ComparisonLead.Notes).Index(index++).Name("Record Note");

        // Reasoning
        Map(m => m.ReasoningStr).Index(index++).Name("Reasoning");
    }
}
