using CsvHelper.Configuration.Attributes;
using Automate.Application.InfrastructureInterfaces;
using Automate.Domain.ValueObjects;
using Automate.Infrastructure.DateTimeConversion;

namespace Automate.Infrastructure.MessageLeadsService.CsvMaps;

public class SplitDateMountainOffsetMsgCol : IMessageConvert
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
    public IMessage ConvertToMessage()
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

        return new Message(number, start, message, source);
    }
}
