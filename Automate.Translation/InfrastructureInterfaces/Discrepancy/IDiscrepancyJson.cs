using Automate.Translation.ValueObjectsTranslations;

namespace Automate.Translation.InfrastructureInterfaces.Discrepancy;

public interface IDiscrepancyJson
{
    IPhoneNumberTranslate? Number { get; set; }
    bool Billable { get; set; }
    DateTime Date { get; set; }
    TimeSpan Duration { get; set; }
    string? Notes { get; set; }
}
