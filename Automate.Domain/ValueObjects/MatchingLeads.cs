namespace Automate.Domain.ValueObjects;

public class MatchingLeads(IDiscrepancyCall billedLead, IDiscrepancyCall comparisonLead, bool billableBeforeComparison)
{
    public bool BillableBefore { get; set; } = billableBeforeComparison;
    public IDiscrepancyCall BilledLead { get; set; } = billedLead;
    public IDiscrepancyCall ComparisonLead { get; set; } = comparisonLead;
    public bool BothBillable => ComparisonLead.Billable;
}
