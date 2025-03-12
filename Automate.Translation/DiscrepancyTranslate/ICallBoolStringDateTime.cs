namespace Automate.Translation.DiscrepancyTranslate;

public interface ICallBoolStringDateTime
{
    long NumberLong { get; set; }
    DateTimeOffset? Date { get; set; }
    string? Billable { get; set; }
    int? Duration { get; set; }
    string? Notes { get; set; }
    string? Source { get; set; }
}
