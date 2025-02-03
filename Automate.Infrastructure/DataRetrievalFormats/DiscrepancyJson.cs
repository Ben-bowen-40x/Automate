using Automate.Translation.DiscrepancyTranslate;
using Automate.Translation.PhoneNumTranslate;

namespace Automate.Infrastructure.Retrieval;

internal class DiscrepancyJson : ICallDateTime
{
    public IPhoneNumberTranslate? Number { get; set; }
    public bool Billable { get; set; }
    public DateTime Date { get; set; }
    public TimeSpan Duration { get; set; }
    public string? Notes { get; set; }
}
