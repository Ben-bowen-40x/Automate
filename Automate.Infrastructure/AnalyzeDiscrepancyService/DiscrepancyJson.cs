using Automate.Translation.InfrastructureInterfaces.Discrepancy;
using Automate.Translation.ValueObjectsTranslations;

namespace Automate.Infrastructure.AnalyzeDiscrepancyService;

internal class DiscrepancyJson : IDiscrepancyJson
{
    public IPhoneNumberTranslate? Number { get; set; }
    public bool Billable { get; set; }
    public DateTime Date { get; set; }
    public TimeSpan Duration { get; set; }
    public string? Notes { get; set; }
}
