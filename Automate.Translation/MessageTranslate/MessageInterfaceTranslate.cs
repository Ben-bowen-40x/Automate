using Automate.Domain.ValueObjects;
using Automate.Translation.PhoneNumTranslate;
using Automate.Translation.ValueObjectsTranslations;
using Automate.Translation.ValueObjectTranslate;

namespace Automate.Translation.MessageTranslate;
public static class MessageInterfaceTranslate
{
    #region Public
    /// <summary>
    /// Extension Method Translates <paramref name="entity"/> from <see cref="IMsgDTONumberLong"/> to <see cref="IMessage"/>
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
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
    public static IMessage Translate(this IMsgDTOStrNonEmptySource entity)
    {
        // This particular execution is uninterested in records without a source, hence the additional calculation here
        PhoneNumber number = string.IsNullOrWhiteSpace(entity.Source) ? PhoneNumberTranslate.Default : PhoneNumberTranslate.Translate(entity.NumberStr);
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
    internal static string VerifyContents(string? contents)
    {
        return string.IsNullOrWhiteSpace(contents)
            ? string.Empty
            : TSH.ContentsJoined(contents);
    }
    internal static string VerifySource(string? source)
    {
        var sourcer = string.IsNullOrWhiteSpace(source)
            ? string.Empty
            : source;
        var removal = "z:"; // If this gets to be a lot, it might be worth using REGEX instead 
        return sourcer.ToLower().Replace(removal, string.Empty);
    }
    private static string VerifySource(string? source, SourceComponent component)
    {
        var separator = component switch
        {
            SourceComponent.Gclid => "gclid=",
            SourceComponent.Msclid => "msclid=",
            _ => throw new ArgumentException($"The {nameof(SourceComponent)} attribute has not been created for the following component separator:\n{component}")
        };
        var verified = VerifySource(source);
        var result = verified.Contains(separator)
            ? verified.Split(separator)[1].Split('/')[0]
            : verified;
        return result;
    }
    #endregion
}
