using CsvHelper.Configuration.Attributes;
using Automate.Domain.ValueObjects;

namespace Automate.Infrastructure.MessageLeadsService.CsvMaps;

public class SplitDateUTCOffsetMsgCol : IConvert
{
    [Name("phone_number")]
    public string? PhoneNumber { get; set; }
    [Name("date_submitted")]
    public string? StartDate { get; set; }
    [Name("time_submitted")]
    public string? StartTime { get; set; }
    [Name("how_can_we_help")]
    public string? Contents { get; set; }
    [Name("page_name")]
    public string? Source { get; set; }
    public IMessage Convert<SplitDateUTCOffsetMsgCol, IMessage>()
    {
        // Convert UTC string to DateTimeOffset
        string startTim = StartTime is not null
            ? StartTime
            : string.Empty;
        string startTime = startTim.Contains("utc", StringComparison.CurrentCultureIgnoreCase) 
            ? startTim.Replace("utc", string.Empty)
            : startTim;
        DateTimeOffset start =
            StartDate is not null && DateTimeOffset.TryParse($"{StartDate} {startTime}", out DateTimeOffset resultDate)
            ? resultDate
            : DateTimeOffset.MinValue;
        // Source
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
        IMessage result = (IMessage)(Domain.ValueObjects.IMessage)new Message(number, start, message, source);

        return result;
    }
}
