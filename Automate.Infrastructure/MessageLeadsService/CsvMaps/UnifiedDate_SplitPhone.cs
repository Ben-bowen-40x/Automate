using CsvHelper.Configuration.Attributes;
using Automate.Domain.ValueObjects;

namespace Automate.Infrastructure.MessageLeadsService.CsvMaps;

public class UnifiedDate_SplitPhone : IConvert
{
    [Name("phone_number")]
    public string? PhoneNumber { get; set; }
    [Name("created_time")]
    public string? StartDate { get; set; }
    [Name("what_bugs_are_you_having_trouble_with?")]
    public string? Contents { get; set; }
    [Name("zip_code")]
    public string? Source { get; set; }
    public IMessage Convert<UnifiedDate_SplitPhone, IMessage>()
    {
        // Convert local to DateTimeOffset
        DateTimeOffset start =
            DateTimeOffset.TryParse(StartDate, out DateTimeOffset startResult)
            ? startResult
            : FormatDateAndTryAgain(StartDate);

        // Source Url
        string url = Source is null ? string.Empty : FormatSource(Source);

        // Phone number
        PhoneNumber number =
            PhoneNumber is null || PhoneNumber == string.Empty || PhoneNumber.Length < 10
            ? new(0)
            : new(PhoneNumber[^10..]);
        string message =
            Contents is null
            ? string.Empty
            : CsvMapsHelper.ContentsJoined(Contents!);

        // Cast new message into IMessage
        IMessage result = (IMessage)(Domain.ValueObjects.IMessage)new Message(number, start, message, url);

        return result;
    }

    private static string FormatSource(string source)
    {
        if (source.Contains("z:"))
            return source.Split("z:")[1];
        return source;
    }

    private static DateTimeOffset FormatDateAndTryAgain(string? startDate)
    {
        if (startDate == null)
            return DateTimeOffset.MinValue;
        return DateTimeOffset.TryParse
            (
                string.Join(' ', startDate.Split('T')),
                out DateTimeOffset result
            )
            ? result
            : DateTimeOffset.MinValue;
    }
}

