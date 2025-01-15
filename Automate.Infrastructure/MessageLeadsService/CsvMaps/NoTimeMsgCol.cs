using Automate.Domain.ValueObjects;
using CsvHelper.Configuration.Attributes;

namespace Automate.Infrastructure.MessageLeadsService.CsvMaps;

public class NoTimeMsgCol : IConvert
{
    [Name("Phone")]
    public string? PhoneNumber { get; set; }
    [Name("Date")]
    public string? StartDate { get; set; }
    [Name("Problem")]
    public string? Contents { get; set; }
    [Name("Referring URL")]
    public string? Source { get; set; }
    public IMessage Convert<NoTimeMsgCol, IMessage>()
    {
        // Convert Phone Number
        PhoneNumber number = PhoneNumber is null || PhoneNumber == string.Empty ? new(0) : new(PhoneNumber);

        // Convert date
        // Date does not have a time, so the time is assumed to be midnight
        DateTime interDate = StartDate is null || StartDate == string.Empty || !DateTime.TryParse(StartDate, out DateTime interResult) ? DateTime.MaxValue : interResult;
        DateTimeOffset date = new(interDate, TimeSpan.FromTicks(0));

        // Convert Contents
        string contents =
        Contents is null || Contents == string.Empty
            ? string.Empty
            : CsvMapsHelper.ContentsJoined(Contents);

        // Convert Source
        string source = Source is null || Source == string.Empty ? string.Empty : Source;

        // Cast new message into IMessage
        IMessage resultMsg = (IMessage)(Domain.ValueObjects.IMessage)new Message(number, date, contents, source);

        return resultMsg;
    }
}
