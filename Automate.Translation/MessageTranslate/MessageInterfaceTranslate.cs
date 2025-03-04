using Automate.Domain.ValueObjects;
using Automate.Translation.PhoneNumTranslate;
using Automate.Translation.ValueObjectsTranslations;
using Automate.Translation.ValueObjectTranslate;
using System.Text.RegularExpressions;

namespace Automate.Translation.MessageTranslate;
public static partial class MessageInterfaceTranslate
{
    #region Public
    public static IMessage Translate(this IMsgStrsSpecialExtras entity)
    {
        PhoneNumber num = PhoneNumberTranslate.Translate(entity.PhoneNumStr);

        // Date Translation
        string cleanDate = string.IsNullOrWhiteSpace(entity.Date)
            ? string.Empty
            : AtRgx().Replace(entity.Date, string.Empty);
        DateTime d = DateTime.TryParse(cleanDate, out DateTime dtResult) ? dtResult : DateTime.MaxValue;
        TimeZoneEnum z = !string.IsNullOrWhiteSpace(entity.TimeZoneInfo)
            ? FindTimeZone(entity)
            : TimeZoneEnum.Eastern;
        DateTimeOffset date = ConvertDateTimeOffset.Convert(d, z);
        
        // Source
        string source = VerifySource(entity.Source);
        
        // Determine lead status
        bool islead = !string.IsNullOrWhiteSpace(entity.Lead) && entity.Lead.Contains("yes", StringComparison.CurrentCultureIgnoreCase);
        IMessage result = islead
            ? new Message(num, date, string.Empty, source)
            : new Message(PhoneNumberTranslate.Default, DateTimeOffset.MaxValue, string.Empty, string.Empty);

        return result;
    }

    /// <summary>
    /// Extension Method Translates <paramref name="entity"/> from <see cref="IMsgDTONumberLong"/> to <see cref="IMessage"/>
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    // No need to test; all components are tested elsewhere
    public static IMessage Translate(this IMsgDTONumberLong entity)
    {
        PhoneNumber number = PhoneNumberTranslate.Translate(entity.Number);
        string contents = VerifyContents(entity.Contents);
        string source = VerifySource(entity.Source);
        IMessage result = new Message(number, entity.Date, contents, source);

        return result;
    }

    /// <summary>
    /// Extension Method Translates <paramref name="entity"/> from <see cref="IMsgStrDateTimeOffset"/> to <see cref="IMessage"/>
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    // No need to test; all components are tested elsewhere
    public static IMessage Translate(this IMsgStrDateTimeOffset entity)
    {
        PhoneNumber num = PhoneNumberTranslate.Translate(entity.Number);
        string contents = VerifyContents(entity.Contents);
        string source = VerifySource(entity.Source);
        IMessage rMsg = new Message(num, entity.Date, contents, source);

        return rMsg;
    }

    /// <summary>
    /// Extension Method Translates <paramref name="entity"/> from <see cref="IMsgNoTimeStrUtc"/> to <see cref="IMessage"/>
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    // No need to test; all components are tested elsewhere
    public static IMessage Translate(this IMsgNoTimeStrUtc entity)
    {
        PhoneNumber number = PhoneNumberTranslate.Translate(entity.NumberStr);
        DateTime interDate = ConvertPrimitive.ConvertDate(entity.DateTimeStr, null, DateDefault.Min);
        DateTimeOffset date = ConvertPrimitive.ConvertDateTimeOffset(interDate, TimeSpan.FromTicks(0)); // This type is already in UTC
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
    // No need to test; all components are tested elsewhere
    public static IMessage Translate(this IMsgDTOStr entity)
    {
        PhoneNumber number = PhoneNumberTranslate.Translate(entity.Number);
        DateTimeOffset date = ConvertPrimitive.ConvertDateTimeOffset(entity.DateTimeOffsetStr, DateDefault.Min);
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
    // No need to test; all components are tested elsewhere
    public static IMessage Translate(this IMsgDTOStrIsolateSource entity)
    {
        PhoneNumber number = PhoneNumberTranslate.Translate(entity.NumberStr);
        DateTimeOffset date = ConvertPrimitive.ConvertDateTimeOffset(entity.DateTimeOffsetStr, DateDefault.Min);
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
    // No need to test; all components are tested elsewhere
    public static IMessage Translate(this IMsgDTOStrNonEmptySource entity)
    {
        PhoneNumber number = VerifyNumber(entity.Source, entity.NumberStr);
        DateTimeOffset date = ConvertPrimitive.ConvertDateTimeOffset(entity.DateTimeOffsetStr, DateDefault.Min);
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
    // No need to test; all components are tested elsewhere
    public static IMessage Translate(this IMsgZoneEnumStr entity)
    {
        DateTimeOffset start = ConvertPrimitive.ConvertDateTimeOffset(entity.Date, entity.TimeZone, DateDefault.Min);
        string source = VerifySource(entity.Source);
        PhoneNumber number = PhoneNumberTranslate.Translate(entity.Number);
        string message = VerifyContents(entity.Contents);
        IMessage result = new Message(number, start, message, source);

        return result;
    }
    #endregion

    #region Verifications
    internal static PhoneNumber VerifyNumber(string? source, string? stringNum)
    {
        return string.IsNullOrWhiteSpace(source) ? PhoneNumberTranslate.Default : PhoneNumberTranslate.Translate(stringNum);
    }
    internal static string VerifyContents(string? contents)
    {
        return string.IsNullOrWhiteSpace(contents)
            ? string.Empty
            : TSH.ContentsJoined(contents);
    }
    internal static string VerifySource(string? source)
    {
        string sourcer = string.IsNullOrWhiteSpace(source)
            ? string.Empty
            : source;
        string removal = "z:";
        string replaced = sourcer.Replace(removal, null);
        string result = TSH.ReplaceCsvAwkward(replaced, string.Empty);
        return result;
    }
    internal static string VerifySource(string? source, SourceComponent component)
    {
        string separator = component.ToString().ToLower() + "=";
        string verified = VerifySource(source);
        bool hasComponent = verified.Contains(separator);
        string isolation = hasComponent ? verified.Split(separator)[1] : verified;
        string result = hasComponent ? isolation.Split('/')[0] : isolation;
        return result;
    }
    private static TimeZoneEnum FindTimeZone(IMsgStrsSpecialExtras entity) => entity.TimeZoneInfo!.ToLower() switch
    {
        string t when t.Contains("est") || t.Contains("edt") => TimeZoneEnum.Eastern,
        string t when t.Contains("cst") || t.Contains("cdt") => TimeZoneEnum.Central,
        string t when t.Contains("mst") || t.Contains("mdt") => TimeZoneEnum.Mountain,
        string t when t.Contains("pst") || t.Contains("pdt") => TimeZoneEnum.Pacific,
        _ => TimeZoneEnum.Eastern
    };
    #endregion

    #region GeneratedRegex
    [GeneratedRegex("at", RegexOptions.Compiled)]
    private static partial Regex AtRgx();
    #endregion
}
