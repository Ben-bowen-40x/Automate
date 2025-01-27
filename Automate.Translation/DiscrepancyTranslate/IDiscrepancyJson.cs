using Automate.Translation.PhoneNumTranslate;

namespace Automate.Translation.DiscrepancyTranslate;

public interface IDiscrepancyJson
{
    IPhoneNumberTranslate? Number { get; set; }
    bool Billable { get; set; }
    DateTime Date { get; set; }
    TimeSpan Duration { get; set; }
    string? Notes { get; set; }
}
