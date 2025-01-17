using Automate.Translation.ValueObjectsTranslations;

namespace Automate.Translation.DiscrepancyTranslations;

public interface IDiscrepancyJson
{
    public INumberTypeJson? Number { get; set; }
    public bool Billable { get; set; }
    public DateTime Date { get; set; }
    public TimeSpan Duration { get; set; }
    public string? Notes { get; set; }
}
