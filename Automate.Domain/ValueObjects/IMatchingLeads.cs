namespace Automate.Domain.ValueObjects;

public interface IMatchingLeads
{
    bool BillableBefore { get; set; }
    IDiscrepancyCall BilledLead { get; set; }
    bool BothBillable { get; }
    IDiscrepancyCall ComparisonLead { get; set; }
}