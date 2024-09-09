namespace Automate.Domain.ValueObjects;

public record Message(PhoneNumber Number, DateTimeOffset Date, string Contents, string Source) : IMessage
{
    public DateTimeOffset Date { get; set; } = Date;
    public string? Source { get; set; } = Source;
    public override string ToString()
    {
        return $"{nameof(Message)}: {nameof(Number)}: {Number.Number}, {nameof(Date)}: {Date.ToString(DateTimeStrings.InternalDateTimeOffset)}, {nameof(Contents)}: {string.Join('|', Contents.Split(',', '\n', '\r'))}, {nameof(Source)}: {Source}";
    }
}
