namespace Automate.Translation.InfrastructureInterfaces.Discrepancy;

public interface IDiscrepancyCall : IDiscrepancyNotes
{
    public string? Number { get; set; }
    public string? Date { get; set; }
    public string? Duration { get; set; }
}
