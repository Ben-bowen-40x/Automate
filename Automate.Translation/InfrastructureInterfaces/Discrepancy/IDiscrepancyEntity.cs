namespace Automate.Translation.InfrastructureInterfaces.Discrepancy;

public interface IDiscrepancyEntity
{
    long Number { get; set; }
    DateTime? Date { get; set; }
    string? Billable { get; set; }
    int? Duration { get; set; }
    string? Notes { get; set; }
}
