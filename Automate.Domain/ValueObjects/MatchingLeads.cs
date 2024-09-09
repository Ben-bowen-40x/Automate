namespace Automate.Domain.ValueObjects;

public class MatchingLeads(DiscrepancyCall billedLead, DiscrepancyCall comparisonLead, bool billableBeforeComparison)
{
    public bool BillableBefore { get; set; } = billableBeforeComparison;
    public DiscrepancyCall BilledLead { get; set; } = billedLead;
    public DiscrepancyCall ComparisonLead { get; set; } = comparisonLead;
    public bool BothBillable => ComparisonLead.Billable;
}
