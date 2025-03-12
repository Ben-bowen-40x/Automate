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
    public static IDiscrepancyCall Translate(this ICallBoolStringDateTime entity)
    {
        PhoneNumber number = PhoneNumberTranslate.Translate(entity.NumberLong);
        bool billable = ConvertPrimitive.ConvertBool(entity.Billable);
        DateTimeOffset date = entity.Date is not null ? entity.Date.Value : DateTimeOffset.MinValue;
        string notes = VerifyNotes(entity.Notes);
        DiscrepancySource source = VerifySource(entity.Source);
        TimeSpan duration = GetDuration(entity.Duration);
        DiscrepancyCall result = new(number, billable, date, duration, source, notes);

        return result;
    }

    /// <summary>
    /// Translates <see cref="ICallDateTime"/> to <see cref="DiscrepancyCall"/>
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public static IDiscrepancyCall Translate(this ICallDateTime entity)
    {
        PhoneNumber number = PhoneNumberTranslate.Translate(entity.Number);
        string notes = VerifyNotes(entity.Notes);
        DiscrepancySource source = VerifySource(entity.Source);
        bool billable = !string.IsNullOrWhiteSpace(entity.Billable) && !entity.Billable.Contains("non", StringComparison.InvariantCultureIgnoreCase);
        TimeSpan duration = entity.Duration is not null ? TimeSpan.FromSeconds((double)entity.Duration!) : TimeSpan.FromSeconds(0);
        DiscrepancyCall result = new(number, billable, entity.Date, duration, source, notes);

        return result;
    }

    /// <summary>
    /// Translates <see cref="IDiscrepancyBillable"/> to <see cref="DiscrepancyCall"/>
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public static IDiscrepancyCall Translate(this IDiscrepancyBillable entity)
    {
        string notes = VerifyNotes(entity.Notes);
        DiscrepancySource source = VerifySource(entity.Source);
        DateTimeOffset startDate = ConvertPrimitive.ConvertDateTimeOffset(entity.Date, DateDefault.Min);
        TimeSpan duration = GetDuration(entity.Duration);
        PhoneNumber number = PhoneNumberTranslate.Translate(entity.Number);
        DiscrepancyCall result = new(number, true, startDate, duration, source, notes); // Note that source leads are always billable

        return result;
    }

    #region Internal
    internal static string VerifyNotes(string? note)
    {
        return note is not null
            ? TSH.ContentsJoined(note)
            : string.Empty;
    }

    internal static DiscrepancySource VerifySource(string? source) => source switch
    {
        // This case must be first and it must execute, because this expression will test every condition, and if this condition is true, then the other conditions will throw
        string s when string.IsNullOrWhiteSpace(s) => DiscrepancySource.Null,
        string s when s!.Contains(DiscrepancySource.Libacion.ToString(), StringComparison.InvariantCultureIgnoreCase) => DiscrepancySource.Libacion,
        string s when s!.Contains(DiscrepancySource.Guliagar.ToString(), StringComparison.InvariantCultureIgnoreCase) => DiscrepancySource.Guliagar,
        string s when s!.Contains(DiscrepancySource.ElkHall.ToString(), StringComparison.InvariantCultureIgnoreCase) => DiscrepancySource.ElkHall,
        _ => DiscrepancySource.Null,
    };

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
