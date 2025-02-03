namespace Automate.Translation.DiscrepancyTranslate;

public interface IDiscrepancyBillable
{
    public string? Number { get; set; }
    public string? Date { get; set; }
    public string? Duration { get; set; }
    public string? Notes { get; set; }
}
