using Automate.Translation.CallTranslate;

namespace Automate.Translation.DiscrepancyTranslate;

public interface ICallDateTime
{
    NumberType? Number { get; set; }
    bool Billable { get; set; }
    DateTime Date { get; set; }
    TimeSpan Duration { get; set; }
    string? Notes { get; set; }
    string? Source { get; set; }
}
