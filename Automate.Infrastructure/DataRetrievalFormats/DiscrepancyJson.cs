using Automate.Translation.CallTranslate;
using Automate.Translation.DiscrepancyTranslate;

namespace Automate.Infrastructure.Retrieval;

internal class DiscrepancyJson : ICallDateTime
{
    public NumberType? Number { get; set; }
    public bool Billable { get; set; }
    public DateTime Date { get; set; }
    public TimeSpan Duration { get; set; }
    public string? Notes { get; set; }
}
