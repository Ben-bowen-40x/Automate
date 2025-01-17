using Automate.Translation.DiscrepancyTranslations;
using Automate.Translation.ValueObjectsTranslations;

namespace Automate.Infrastructure.AnalyzeDiscrepancyService;

internal class DiscrepancyJson : IDiscrepancyJson
{
    public INumberTypeJson? Number { get; set; }
    public bool Billable { get; set; }
    public DateTime Date { get; set; }
    public TimeSpan Duration { get; set; }
    public string? Notes { get; set; }
}
