using Automate.Domain.ValueObjects;
using Automate.Translation.CallTranslate;
using Automate.Translation.DiscrepancyTranslate;

namespace Automate.Infrastructure.Retrieval;

public class DiscrepancyJson : ICallDateTime, IConvert
{
    public NumberType? Number { get; set; }
    public string? Billable { get; set; }
    public DateTime Date { get; set; }
    public int? Duration { get; set; }
    public string? Source { get; set; }
    public string? Notes { get; set; }

    public IDiscrepancyCall Convert<ICallDateTime, IDiscrepancyCall>()
    {
        return (IDiscrepancyCall)this.Translate();
    }
}
