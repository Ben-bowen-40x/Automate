using Automate.Domain.ValueObjects;
using Automate.Translation.DiscrepancyTranslate;

namespace Automate.Translation.ValueObjectsTranslations;

public static class DiscrepancyCallTranslate
{
    /// <summary>
    /// Translates <see cref="IDiscrepancyEntity"/> to <see cref="DiscrepancyCall"/>
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public static DiscrepancyCall Convert(this IDiscrepancyEntity entity)
    {
        PhoneNumber number = new(entity.Number);
        bool billable = entity.Billable is not null && entity.Billable != string.Empty && entity.Billable == "billable";
        DateTime date = entity.Date is not null ? (DateTime)entity.Date! : DateTime.MinValue;
        string notes = entity.Notes is not null ? TSH.ContentsJoined(entity.Notes) : string.Empty;
        TimeSpan duration = entity.Duration is null ? new(0) : TimeSpan.FromSeconds((double)entity.Duration!);

        return new(number, billable, date, duration, notes);
    }

    /// <summary>
    /// Translates <see cref="IDiscrepancyJson"/> to <see cref="DiscrepancyCall"/>
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public static DiscrepancyCall Convert(this IDiscrepancyJson entity)
    {
        PhoneNumber number = entity.Number is null ? new(0) : new(entity.Number.Number);
        string notes = entity.Notes is not null ? TSH.ContentsJoined(entity.Notes) : string.Empty;
        return new DiscrepancyCall(number, entity.Billable, entity.Date, entity.Duration, notes);
    }

    /// <summary>
    /// Translates <see cref="IDiscrepancyCallTranslate"/> to <see cref="DiscrepancyCall"/>
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public static DiscrepancyCall Convert(this IDiscrepancyCallTranslate entity)
    {
        string notes = entity.Notes is null
            ? string.Empty
            : TSH.ContentsJoined(entity.Notes);
        DateTime startDate = entity.Date is null | !DateTime.TryParse(entity.Date, out DateTime startResult) ? DateTime.MinValue : startResult;
        TimeSpan duration = entity.Duration is null | !TimeSpan.TryParse(entity.Duration, out TimeSpan durationResult) ? new(0) : durationResult;
        PhoneNumber number = entity.Number is null ? new(0) : new(entity.Number);

        return new(number, true, startDate, duration, notes); // Note that source leads are always billable
    }
}
