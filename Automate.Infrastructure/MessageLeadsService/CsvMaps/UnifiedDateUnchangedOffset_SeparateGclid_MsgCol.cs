using CsvHelper.Configuration.Attributes;
using Automate.Domain.ValueObjects;

namespace Automate.Infrastructure.MessageLeadsService.CsvMaps;

public class UnifiedDateUnchangedOffset_SeparateGclid_MsgCol : IConvert
{
    [Name("Prospect Cellphone", "Phone Number", "Number")]
    public string? PhoneNumber { get; set; }
    [Name("Creation", "Message Creation", "Date")]
    public string? StartDate { get; set; }
    [Name("Message", "Contents")]
    public string? Contents { get; set; }
    [Name("Message Source", "Source")]
    public string? Source { get; set; }
    public IMessage Convert<UnifiedDateUnchangedOffset_SeparateGclid_MsgCol, IMessage>()
    {
        // Convert local to DateTimeOffset
        DateTimeOffset start =
            DateTimeOffset.TryParse(StartDate, out DateTimeOffset startResult)
            ? startResult
            : DateTimeOffset.MinValue;

        // Source Url
        string url = Source is null ? string.Empty : Gclid(Source!);

        // Phone number
        PhoneNumber number =
            PhoneNumber is null || PhoneNumber == string.Empty || PhoneNumber.Length < 10
            ? new(0)
            : new(PhoneNumber);
        string message =
            Contents is null
            ? string.Empty
            : CsvMapsHelper.ContentsJoined(Contents!);

        // Cast new message into IMessage
        IMessage result = (IMessage)(Domain.ValueObjects.IMessage)new Message(number, start, message, url);

        return result;
    }

    private static string Gclid(string str)
    {
        string g = "gclid=";
        if (str.Contains(g))
            return str.Split(g)[1].Split('/')[0];
        else return str;
    }
}

