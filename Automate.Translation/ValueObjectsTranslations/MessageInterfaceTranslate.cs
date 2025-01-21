using Automate.Domain.ValueObjects;
using Automate.Translation.DateTimeConvertService;
using Automate.Translation.InfrastructureInterfaces.Message;
using Microsoft.VisualBasic;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Automate.Translation.ValueObjectsTranslations;

public static class MessageInterfaceTranslate
{
    // IMsgStrDateTimeOffset
    public static IMessage Convert(this IMsgStrDateTimeOffset entity)
    {
        // Convert number
        PhoneNumber num = entity.Number is not null
            ? new(entity.Number)
            : new(0);

        // Convert Contents
        string contents = entity.Contents is not null && entity.Contents != string.Empty
            ? TSH.ContentsJoined(entity.Contents)
            : string.Empty;

        // Convert Source
        string source = entity.Source is not null && entity.Source != string.Empty
            ? entity.Source
            : string.Empty;

        // Cast new message into IMessage
        IMessage rMsg = new Message(num, entity.Date, contents, source);

        return rMsg;
    }

    // IMsgNoTimeStr
    public static IMessage Convert(this IMsgNoTimeStr entity)
    {
        // Convert Phone Number
        PhoneNumber number = entity.Number is null || entity.Number == string.Empty ? new(0) : new(entity.Number);

        // Convert date
        // Date does not have a time, so the time is assumed to be midnight
        DateTime interDate = entity.Date is null || entity.Date == string.Empty || !DateTime.TryParse(entity.Date, out DateTime interResult) ? DateTime.MaxValue : interResult;
        DateTimeOffset date = new(interDate, TimeSpan.FromTicks(0));

        // Convert Contents
        string contents =
        entity.Contents is null || entity.Contents == string.Empty
            ? string.Empty
            : TSH.ContentsJoined(entity.Contents);

        // Convert Source
        string source = entity.Source is null || entity.Source == string.Empty ? string.Empty : entity.Source;

        // Cast new message into IMessage
        IMessage resultMsg = new Message(number, date, contents, source);

        return resultMsg;
    }

    // IMsgTimeStr
    public static IMessage Convert(this IMsgTimeStr entity)
    {
        // Convert local to DateTimeOffset
        DateTime startDate =
            entity.Date is not null && DateTime.TryParse($"{entity.Date} {entity.Time}", out DateTime resultDate)
            ? resultDate
            : DateTime.MinValue;
        DateTimeOffset start =
            DateTimeConversions.ConvertLocalToDTOffset(startDate, TimeZoneEnum.Mountain, out DateTimeOffset resultOffset)
            ? resultOffset
            : DateTimeOffset.MinValue;

        // Source Url
        string source = entity.Source is null ? string.Empty : entity.Source!;

        // Phone Number
        PhoneNumber number =
            entity.Number is null || entity.Number == string.Empty || entity.Number.Length < 10
        ? new(0)
            : new(entity.Number);

        // Message
        string message =
            entity.Contents is null
            ? string.Empty
            : TSH.ContentsJoined(entity.Contents!);

        // Cast new message into IMessage
        IMessage result = new Message(number, start, message, source);

        return result;
    }
}
