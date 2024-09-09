namespace Automate.Domain.ValueObjects;

public record MessageCallRecord(PhoneNumber Number, DateTimeOffset Date, bool Billable) : ICallRecord
{
    public DateTimeOffset Date { get; set; } = Date;
    public override string ToString()
    {
        return $"{nameof(MessageCallRecord)}: {nameof(Number)}: {Number.Number}, {nameof(Date)}: {Date.ToString(DateTimeStrings.InternalDateTimeOffset)}, {nameof(Billable)}: {Billable}";
    }
}
