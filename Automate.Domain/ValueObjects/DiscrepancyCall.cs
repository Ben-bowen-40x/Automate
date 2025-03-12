namespace Automate.Domain.ValueObjects;

public record DiscrepancyCall(PhoneNumber Number, bool Billable, DateTimeOffset Date, TimeSpan Duration, DiscrepancySource Source, string Notes) : IDiscrepancyCall
{
    public DateTimeOffset Date { get; set; } = Date;

    public override string ToString()
    {
        return $"{nameof(PhoneNumber)}: {Number}, {nameof(Billable)}: {Billable}, {nameof(Date)}: {Date}, {nameof(Duration)}: {Duration}, {nameof(Source)}: {Source}, {nameof(Notes)}: {Notes}";
    }
}
