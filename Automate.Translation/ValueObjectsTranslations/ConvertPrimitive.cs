namespace Automate.Translation.ValueObjectsTranslations;

public static class ConvertPrimitive
{
    /// <summary>
    /// Converts a nullable <see cref="string"/> value into a <see cref="bool"/>
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    internal static bool ConvertBool(string? value)
    {
        // Unfortunately, it may be necessary to use Regex eventually
        string[] trueConditions = ["bilable", "bilabel", "billable", "billabel"];
        string[] falseConditions = ["non", "not"];
        bool result = (value is not null && trueConditions.Any(value.ToLower().Contains) && !falseConditions.Any(value.ToLower().Contains)) || (bool.TryParse(value, out bool v) && v);
        return result;
    }

    /// <summary>
    /// Converts a nullable <see cref="int"/> value into a <see cref="bool"/>
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    internal static bool ConvertBool(int? value)
    {
        return value is not null && value > 0;
    }

    /// <summary>
    /// Converts a nullable <see cref="double"/> value into a <see cref="double"/>
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    internal static double VerifyValue(double? value)
    {
        return value is null ? 0.0 : (double)value;
    }

    /// <summary>
    /// Converts a <paramref name="value"/> from nullable <see cref="string"/> to <see cref="TimeSpan"/>
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    internal static TimeSpan ConvertTimeSpan(string? value)
    {
        if (value is not null)
        {
            // Attempt a direct parse
            TimeSpan tzResult = value.Contains(':') && TimeSpan.TryParse(value, out TimeSpan t)
                ? t
                : TimeSpan.Zero;

            // Detect whether it has decimals
            TimeSpan doubleResult = value.Contains('.') && double.TryParse(value, out double d)
                ? TimeSpan.FromSeconds(d)
                : TimeSpan.Zero;

            // Detect whether it is an integer
            TimeSpan intResult = int.TryParse(value, out int i)
                ? TimeSpan.FromSeconds(i)
                : TimeSpan.Zero;

            if (tzResult != TimeSpan.Zero)
                return tzResult;
            if (doubleResult != TimeSpan.Zero)
                return doubleResult;
            if (intResult != TimeSpan.Zero)
                return intResult;
        }
        return TimeSpan.Zero;
    }

    /// <summary>
    /// Converts a nullable <see cref="DateTime"/> <paramref name="value"/> which is already in UTC and <see cref="TimeSpan"/> <paramref name="timeZone"/> into <see cref="DateTimeOffset"/>
    /// </summary>
    /// <param name="value"></param>
    /// <param name="timeZone"></param>
    /// <returns></returns>
    // Does not need tests because its components are guaranteed to work.
    internal static DateTimeOffset ConvertDateTimeOffset(DateTime? value, TimeSpan timeZone, DateTimeDefault defaultVal)
    {
        DateTime dateInter = value is null ? defaultVal.DateTimeDefault() : (DateTime)value;
        DateTimeOffset date = new(dateInter, timeZone);
        return date;
    }

    /// <summary>
    /// <para>This conversion receives a nullable <see cref="string"/> formatted like a <see cref="DateTimeOffset"/>, with offsets etc.</para>
    /// <para>For example: yyyy-mm-ddTHH:MM:SS-hh:00</para>
    /// <para>You will note that <see cref="DateTimeOffset"/> does not easily parse such strings because of the extra "T" character</para>
    /// <para>However, this implementation will not find it difficult to handle any nullale <see cref="string"/> input value, so long as it has a string formatted like a <see cref="DateTimeOffset"/>. Otherwise, please use a different overload.</para>
    /// </summary>
    /// <param name="dtOffsetStr"></param>
    /// <returns></returns>
    internal static DateTimeOffset ConvertDateTimeOffset(string? dtOffsetStr, DateTimeDefault defaultDate)
    {
        // If the string to get rid of becomes tedious, You may want to use REGEX instead of .Contains() and .Replace()
        string? cleanedDate = dtOffsetStr is not null && dtOffsetStr.Contains('T', StringComparison.InvariantCultureIgnoreCase)
            ? dtOffsetStr.Replace('T', ' ') // DO NOT use string.Empty
            : dtOffsetStr;
        DateTimeOffset result = cleanedDate is null || !DateTimeOffset.TryParse
            (
                cleanedDate,
                out DateTimeOffset d
            )
            ? defaultDate.DateTimeOffsetDefault()
            : d;
        return result;
    }

    /// <summary>
    /// Converts a nullable <see cref="string"/> value, <paramref name="date"/> and <see cref="TimeZoneEnum"/> value, <paramref name="zone"/> into <see cref="DateTimeOffset"/>
    /// </summary>
    /// <param name="date"></param>
    /// <param name="zone"></param>
    /// <returns></returns>
    // Does not need tests because its components are already being tested
    internal static DateTimeOffset ConvertDateTimeOffset(string? date, TimeZoneEnum zone, DateTimeDefault defaultDate)
    {
        var datetime = ConvertDate(date, defaultDate);
        return ConvertDateTimeOffset(datetime, zone, defaultDate);
    }

    /// <summary>
    /// This conversion uses a <see cref="TimeZoneEnum"/> to convert a <see cref="DateTime"/> to <see cref="DateTimeOffset"/>
    /// </summary>
    /// <param name="startDate"></param>
    /// <param name="zone"></param>
    /// <returns></returns>
    internal static DateTimeOffset ConvertDateTimeOffset(DateTime startDate, TimeZoneEnum zone, DateTimeDefault defaultDate)
    {
        return ValueObjectsTranslations.ConvertDateTimeOffset.ConvertLocalToDTOffset(startDate, zone, out DateTimeOffset resultOffset)
            ? resultOffset
            : defaultDate.DateTimeOffsetDefault();
    }

    /// <summary>
    /// Converts a <paramref name="startdate"/> that is already in UTC and <see cref="TimeSpan"/>, which is the offset, to <see cref="DateTimeOffset"/> 
    /// </summary>
    /// <param name="startdate"></param>
    /// <param name="zone"></param>
    /// <returns></returns>
    // This doesn't need tests because its components are guaranteed to work
    internal static DateTimeOffset ConvertDateTimeOffset(DateTime startdate, TimeSpan zone)
    {
        return new(startdate, zone);
    }

    /// <summary>
    /// Converts two nullable <see cref="string"/> values, <paramref name="date"/> and <paramref name="time"/> into <see cref="DateTime"/>
    /// </summary>
    /// <param name="date"></param>
    /// <param name="time"></param>
    /// <returns></returns>
    internal static DateTime ConvertDate(string? date, string? time, DateTimeDefault defaultDate)
    {
        // This translation absolutely must have the correct value on these particular primitives because of the way they will be used on the Domain Layer
        string timeNotNull = string.IsNullOrWhiteSpace(time) ? string.Empty : " " + time;

        // You might want to use REGEX instead of .Contains() if this starts to become a lot of stuff
        string plainTime = timeNotNull.Contains("utc", StringComparison.InvariantCultureIgnoreCase)
            ? timeNotNull.ToLower().Replace("utc", string.Empty)
            : timeNotNull;

        string dateNotNull = string.IsNullOrWhiteSpace(date)
            ? string.Empty
            : date;

        string combined = string.IsNullOrWhiteSpace(dateNotNull)
            ? string.Empty
            : $"{dateNotNull}{plainTime}";

        return ConvertDate(combined, defaultDate);
    }

    /// <summary>
    /// Converts a nullable <see cref="string"/> value <paramref name="date"/> into a <see cref="DateTime"/>
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    internal static DateTime ConvertDate(string? date, DateTimeDefault defaultDate)
    {
        return DateTime.TryParse(date, out DateTime resultDate)
            ? resultDate
            : defaultDate.DateTimeDefault();
    }

    #region Private
    internal static DateTime DateTimeDefault(this DateTimeDefault dateTimeDefault) // This is internal because it's used in testing
    {
        return dateTimeDefault switch
        {
            ValueObjectsTranslations.DateTimeDefault.Min => DateTime.MinValue,
            ValueObjectsTranslations.DateTimeDefault.Max => DateTime.MaxValue,
            _ => DateTime.MinValue
        };
    }
    private static DateTimeOffset DateTimeOffsetDefault(this DateTimeDefault dateTimeDefault)
    {
        return dateTimeDefault switch
        {
            ValueObjectsTranslations.DateTimeDefault.Min => DateTimeOffset.MinValue,
            ValueObjectsTranslations.DateTimeDefault.Max => DateTimeOffset.MaxValue,
            _ => DateTimeOffset.MinValue
        };
    }
    #endregion
}
public enum DateTimeDefault
{
    Min,
    Max,
}

