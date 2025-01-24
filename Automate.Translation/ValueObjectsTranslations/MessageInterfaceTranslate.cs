using Automate.Domain.ValueObjects;
using Automate.Translation.DateTimeConvertService;
using Automate.Translation.InfrastructureInterfaces.Message;

namespace Automate.Translation.ValueObjectsTranslations;

public static class CallInterfaceTranslate
{
    /// <summary>
    /// Extension Method Translates <paramref name="entity"/> from <see cref="IMsgZoneStr"/> to <see cref="IMessage"/>
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public static ICallRecord Convert(this IMsgZoneStr entity)
    {
        bool billable = MessageInterfaceTranslate.ConvertBool(entity.BillableStr);
        TimeSpan timeZone = MessageInterfaceTranslate.ConvertTimeSpan(entity.TimeZoneStr);
        DateTimeOffset date = MessageInterfaceTranslate.ConvertDateTimeOffset(entity.Date, timeZone);
        ICallRecord record = new MessageCallRecord(entity.Number, date, billable);
        return record;
    }
}
public static class MessageInterfaceTranslate
{
    #region Public
    /// <summary>
    /// Extension Method Translates <paramref name="entity"/> from <see cref="IMsgStrDateTimeOffset"/> to <see cref="IMessage"/>
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public static IMessage Convert(this IMsgStrDateTimeOffset entity)
    {
        PhoneNumber num = PhoneNumberTranslate.Convert(entity.NumberStr);
        string contents = VerifyContents(entity.Contents);
        string source = VerifySource(entity.Source);
        IMessage rMsg = new Message(num, entity.Date, contents, source);

        return rMsg;
    }

    /// <summary>
    /// Extension Method Translates <paramref name="entity"/> from <see cref="IMsgNoTimeStr"/> to <see cref="IMessage"/>
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public static IMessage Convert(this IMsgNoTimeStr entity)
    {
        PhoneNumber number = PhoneNumberTranslate.Convert(entity.NumberStr);
        DateTime interDate = ConvertDateTime(entity); // Date does not have a time, so the time is assumed to be midnight
        DateTimeOffset date = ConvertDateTimeOffset(interDate, TimeSpan.FromTicks(0));
        string contents = VerifyContents(entity.Contents);
        string source = VerifySource(entity.Source);
        IMessage resultMsg = new Message(number, date, contents, source);

        return resultMsg;
    }

    /// <summary>
    /// Extension Method Translates <paramref name="entity"/> from <see cref="IMsgDTOStr"/> to <see cref="IMessage"/>
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public static IMessage Convert(this IMsgDTOStr entity)
    {
        PhoneNumber number = PhoneNumberTranslate.Convert(entity.NumberStr);
        DateTimeOffset date = ConvertDateTimeOffset(entity.DateStr);
        string contents = VerifyContents(entity.Contents);
        string source = VerifySource(entity.Source);
        IMessage resultMsg = new Message(number, date, contents, source);
        return resultMsg;
    }

    /// <summary>
    /// Extension Method Translates <paramref name="entity"/> from <see cref="IMsgDTOStrIsolateSource"/> to <see cref="IMessage"/>
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public static IMessage Convert(this IMsgDTOStrIsolateSource entity)
    {
        PhoneNumber number = PhoneNumberTranslate.Convert(entity.NumberStr);
        DateTimeOffset date = ConvertDateTimeOffset(entity.DateStr);
        string contents = VerifyContents(entity.Contents);
        string source = VerifySource(entity.Source, entity.Separator);
        IMessage resultMsg = new Message(number, date, contents, source);
        return resultMsg;
    }

    /// <summary>
    /// Extension Method Translates <paramref name="entity"/> from <see cref="IMsgDTOStrNonEmptySource"/> to <see cref="IMessage"/>
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public static IMessage Convert(this IMsgDTOStrNonEmptySource entity)
    {
        // This particular execution is uninterested in records without a source, hence the additional calculation here
        PhoneNumber number = string.IsNullOrWhiteSpace(entity.Source) ? PhoneNumberTranslate.Default : PhoneNumberTranslate.Convert(entity.NumberStr);
        DateTimeOffset date = ConvertDateTimeOffset(entity.DateStr);
        string contents = VerifyContents(entity.Contents);
        string source = VerifySource(entity.Source, entity.Separator);
        IMessage resultMsg = new Message(number, date, contents, source);
        return resultMsg;
    }

    /// <summary>
    /// Extension Method Translates <paramref name="entity"/> from <see cref="IMsgZoneEnumStr"/> to <see cref="IMessage"/>
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public static IMessage Convert(this IMsgZoneEnumStr entity)
    {
        DateTimeOffset start = ConvertDateTimeOffset(entity, entity.TimeZone);
        string source = VerifySource(entity.Source);
        PhoneNumber number = PhoneNumberTranslate.Convert(entity.NumberStr);
        string message = VerifyContents(entity.Contents);
        IMessage result = new Message(number, start, message, source);

        return result;
    }


    #endregion

    #region Internal Verifications
    internal static string VerifyContents(string? contents)
    {
        return contents is null || contents == string.Empty
            ? string.Empty
            : TSH.ContentsJoined(contents);
    }
    internal static string VerifySource(string? source)
    {
        var sourcer = source is null
            ? string.Empty
            : source;
        var removal = "z:"; // If this gets to be a lot, it might be worth using REGEX instead of .Contains() and .Replace()
        return sourcer.Contains(removal, StringComparison.InvariantCultureIgnoreCase)
                ? sourcer.ToLower().Replace(removal, string.Empty)
                : sourcer;
    }
    internal static string VerifySource(string? source, SourceComponent component)
    {
        var separator = component switch
        {
            SourceComponent.Gclid => "gclid=",
            SourceComponent.Msclid => "msclid=",
            _ => "gclid="
        };
        var verified = VerifySource(source);
        var result = verified.Contains(separator)
            ? verified.Split(separator)[1].Split('/')[0]
            : verified;
        return result;
    }
    #endregion

    #region Internal Primitive Conversions
    /// <summary>
    /// Converts a nullable <see cref="string"/> into a <see cref="bool"/>
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    internal static bool ConvertBool(string? value)
    {
        return value is not null && value.Contains("billable") && !value.Contains("non");
    }

    /// <summary>
    /// Converts a <paramref name="value"/> from nullable <see cref="string"/> to <see cref="TimeSpan"/>
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    internal static TimeSpan ConvertTimeSpan(string? value)
    {
        return value is not null && TimeSpan.TryParse(value, out TimeSpan tzResult) ? tzResult : new(0);
    }

    /// <summary>
    /// Converts a nullable <see cref="DateTime"/> <paramref name="value"/> and <see cref="TimeSpan"/> <paramref name="timeZone"/> into <see cref="DateTimeOffset"/>
    /// </summary>
    /// <param name="value"></param>
    /// <param name="timeZone"></param>
    /// <returns></returns>
    internal static DateTimeOffset ConvertDateTimeOffset(DateTime? value, TimeSpan timeZone)
    {
        DateTimeOffset date;
        DateTime dateInter = value is null ? DateTime.MinValue : (DateTime)value;
        date = new(dateInter, timeZone);
        return date;
    }

    /// <summary>
    /// <para>This conversion receives a nullable <see cref="string"/> formatted like a <see cref="DateTimeOffset"/>, with offsets etc.</para>
    /// <para>For example: yyyy-mm-ddTHH:MM:SS-hh:00</para>
    /// <para>You will note that <see cref="DateTimeOffset"/> does not easily parse such strings because of the extra "T" character</para>
    /// </summary>
    /// <param name="dtOffsetStr"></param>
    /// <returns></returns>
    internal static DateTimeOffset ConvertDateTimeOffset(string? dtOffsetStr)
    {
        // If the string to get rid of becomes tedious, You may want to use REGEX instead of .Contains() and .Replace()
        string? cleanedDate = dtOffsetStr is not null && dtOffsetStr.Contains('T', StringComparison.InvariantCultureIgnoreCase)
            ? dtOffsetStr.Replace('T', ' ') // DO NOT use string.Empty
            : dtOffsetStr;
        return cleanedDate is null || !DateTimeOffset.TryParse
            (
                cleanedDate,
                out DateTimeOffset result
            )
            ? DateTimeOffset.MinValue // This translation absolutely must have the min value on these particular primitives because of the way they will be used on the Domain Layer
            : result;
    }

    /// <summary>
    /// This conversion uses a <see cref="TimeZoneEnum"/> to convert a <see cref="DateTime"/> to <see cref="DateTimeOffset"/>
    /// </summary>
    /// <param name="startDate"></param>
    /// <param name="zone"></param>
    /// <returns></returns>
    internal static DateTimeOffset ConvertDateTimeOffset(DateTime startDate, TimeZoneEnum zone)
    {
        return DateTimeOffsetTranslate.ConvertLocalToDTOffset(startDate, zone, out DateTimeOffset resultOffset)
            ? resultOffset
            : DateTimeOffset.MinValue; // This translation absolutely must have the min value on these particular primitives because of the way they will be used on the Domain Layer
    }

    /// <summary>
    /// Converts a <paramref name="startdate"/> and <see cref="TimeSpan"/> to <see cref="DateTimeOffset"/>
    /// </summary>
    /// <param name="startdate"></param>
    /// <param name="zone"></param>
    /// <returns></returns>
    internal static DateTimeOffset ConvertDateTimeOffset(DateTime startdate, TimeSpan zone)
    {
        return new(startdate, zone);
    }

    /// <summary>
    /// Converts 
    /// </summary>
    /// <param name="date"></param>
    /// <param name="time"></param>
    /// <returns></returns>
    internal static DateTime ConvertDate(string? date, string? time)
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

        return DateTime.TryParse(combined, out DateTime resultDate)
            ? resultDate
            : DateTime.MinValue;
    }
    #endregion

    #region Private Primitive Conversions
    private static DateTime ConvertDateTime(IMessage_Date_Str entity)
    {
        return ConvertDateTime(entity, null);
    }
    private static DateTime ConvertDateTime(IMessage_DateTime_Str entity) // Break glass in case of emergency
    {
        return ConvertDateTime(entity, entity.TimeStr);
    }
    private static DateTimeOffset ConvertDateTimeOffset(IMessage_DateTime_Str entity, TimeZoneEnum zone)
    {
        DateTime startDate = ConvertDateTime(entity, entity.TimeStr);
        DateTimeOffset start = ConvertDateTimeOffset(startDate, zone);
        return start;
    }
    private static DateTimeOffset ConvertDateTimeOffset(IMessage_Date_Str entity, TimeZoneEnum zone) // Break glass in case of emergency
    {
        DateTime startDate = ConvertDateTime(entity, null);
        DateTimeOffset start = ConvertDateTimeOffset(startDate, zone);
        return start;
    }
    private static DateTime ConvertDateTime(IMessage_Date_Str entity, string? time)
    {
        return ConvertDate(entity.DateStr, time);
    }
    #endregion

}
