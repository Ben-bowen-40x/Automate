namespace Automate.Translation.InfrastructureInterfaces.Discrepancy;

public interface IDiscrepancyEntity : IDiscrepancyNotes
{
    public long Number { get; set; }
    public DateTime? Date { get; set; }
    public string? Billable { get; set; }
    public int? Duration { get; set; }
}
