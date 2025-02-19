using Automate.Translation.CallTranslate;

namespace Automate.Translation.DiscrepancyTranslate;

public interface ICallDateTime
{
    NumberType? Number { get; set; }
    string? Billable { get; set; }
    DateTime Date { get; set; }
    int? Duration { get; set; }
    string? Notes { get; set; }
    string? Source { get; set; }
}
