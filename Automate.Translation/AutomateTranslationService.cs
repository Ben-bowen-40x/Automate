using Automate.Domain.ValueObjects;

namespace Automate.Translation;

public static class AutomateTranslationService
{

    public static DiscrepancyCall Convert(this IDiscrepancyEntity entity)
    {
        PhoneNumber number = new(entity.Number);
        bool billable = entity.Billable is not null && entity.Billable != string.Empty && entity.Billable == "billable";
        DateTime date = entity.Date is not null ? (DateTime)entity.Date! : DateTime.MinValue;
        string notes = entity.Notes is not null ? ContentsJoined(entity.Notes) : string.Empty;
        TimeSpan duration = entity.Duration is null ? new(0) : TimeSpan.FromSeconds((double)entity.Duration!);

        return new(number, billable, date, duration, notes);
    }

    private static string ContentsJoined(string contents)
    {
        string str = string.Join('|', contents.Split(',', '"'));
        return string.Join(" | ", str.Split('\n', '\r'));
    }
}