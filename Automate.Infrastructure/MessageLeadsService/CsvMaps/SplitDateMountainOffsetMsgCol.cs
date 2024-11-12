using CsvHelper.Configuration.Attributes;
using Automate.Domain.ValueObjects;
using Automate.Infrastructure.DateTimeConversion;

namespace Automate.Infrastructure.MessageLeadsService.CsvMaps;

public class SplitDateMountainOffsetMsgCol : IConvert
{
    [Name("Customer #")]
    public string? PhoneNumber { get; set; }
    [Name("Date")]
    public string? StartDate { get; set; }
    [Name("Time")]
    public string? StartTime { get; set; }
    [Name("FormCustomFields")]
    public string? Contents { get; set; }
    [Name("Account Name")]
    public string? Source { get; set; }
    public IMessage Convert<SplitDateMountainOffsetMsgCl, IMessage>()
    {
        // Convert local to DateTimeOffset
        DateTime startDate =
            StartDate is not null && DateTime.TryParse($"{StartDate} {StartTime}", out DateTime resultDate)
            ? resultDate
            : DateTime.MinValue;
        DateTimeOffset start =
            DateTimeConversions.ConvertLocalToDTOffset(startDate, TimeZoneEnum.Mountain, out DateTimeOffset resultOffset)
            ? resultOffset
            : DateTimeOffset.MinValue;

        // Source Url
        string source = Source is null ? string.Empty : Source!;

        // Phone Number
        PhoneNumber number =
            PhoneNumber is null || PhoneNumber == string.Empty || PhoneNumber.Length < 10
            ? new(0)
            : new(PhoneNumber);

        // Message
        string message =
            Contents is null
            ? string.Empty
            : CsvMapsHelper.ContentsJoined(Contents!);

        // Cast new message into IMessage
        List<Message> rlist = [new Message(number, start, message, source)];
        List<IMessage> mlist = (List<IMessage>)rlist.Cast<IMessage>();
        IMessage result = mlist[0];

        return result;
    }
}
