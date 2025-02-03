using Automate.Domain.ValueObjects;
using Automate.Translation.PhoneNumTranslate;
using Automate.Translation.ValueObjectsTranslations;
using Automate.Translation.ValueObjectTranslate;

namespace Automate.Translation.DiscrepancyTranslate;

public static class DiscrepancyCallTranslate
{
    /// <summary>
    /// Translates <see cref="ICallBoolStringDateTime"/> to <see cref="DiscrepancyCall"/>
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public static DiscrepancyCall Translate(this ICallBoolStringDateTime entity)
    {
        PhoneNumber number = PhoneNumberTranslate.Translate(entity.Number);
        bool billable = ConvertPrimitive.ConvertBool(entity.Billable);
        DateTime date = ConvertPrimitive.ConvertDate(entity.Date, DateDefault.Min);
        string notes = VerifyNotes(entity.Notes);
        TimeSpan duration = GetDuration(entity.Duration);
        DiscrepancyCall result = new(number, billable, date, duration, notes);

        return result;
    }

    /// <summary>
    /// Translates <see cref="ICallDateTime"/> to <see cref="DiscrepancyCall"/>
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public static DiscrepancyCall Translate(this ICallDateTime entity)
    {
        PhoneNumber number = PhoneNumberTranslate.Translate(entity.Number);
        string notes = VerifyNotes(entity.Notes);
        DiscrepancyCall result = new(number, entity.Billable, entity.Date, entity.Duration, notes);
        return result;
    }

    /// <summary>
    /// Translates <see cref="IDiscrepancyBillable"/> to <see cref="DiscrepancyCall"/>
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public static DiscrepancyCall Translate(this IDiscrepancyBillable entity)
    {
        string notes = VerifyNotes(entity.Notes);
        DateTime startDate = ConvertPrimitive.ConvertDate(entity.Date, DateDefault.Min);
        TimeSpan duration = GetDuration(entity.Duration);
        PhoneNumber number = PhoneNumberTranslate.Translate(entity.Number);
        DiscrepancyCall result = new(number, true, startDate, duration, notes); // Note that source leads are always billable

        return result;
    }

    #region Internal
    internal static string VerifyNotes(string? note)
    {
        return note is not null
            ? TSH.ContentsJoined(note)
            : string.Empty;
    }

    internal static TimeSpan GetDuration(string? duration)
    {
        int seconds = int.TryParse(duration, out int secondsValue) ? secondsValue : 0;
        return duration is null || !TimeSpan.TryParse(duration, out TimeSpan durationResult) || seconds > 0
            ? GetDuration(seconds)
            : durationResult;
    }

    internal static TimeSpan GetDuration(int? duration)
    {
        return duration is null
            ? new(0)
            : TimeSpan.FromSeconds((double)duration!);
    }
    #endregion
}
