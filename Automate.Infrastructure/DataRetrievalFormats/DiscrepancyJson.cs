using Automate.Translation.DiscrepancyTranslate;
using Automate.Translation.ValueObjectsTranslations;

namespace Automate.Infrastructure.Retrieval;

internal class DiscrepancyJson : IDiscrepancyJson
{
    public IPhoneNumberTranslate? Number { get; set; }
    public bool Billable { get; set; }
    public DateTime Date { get; set; }
    public TimeSpan Duration { get; set; }
    public string? Notes { get; set; }
}
