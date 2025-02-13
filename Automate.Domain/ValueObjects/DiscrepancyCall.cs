namespace Automate.Domain.ValueObjects;

public record DiscrepancyCall(PhoneNumber Number, bool Billable, DateTime Date, TimeSpan Duration, string Source, string Notes) : IDiscrepancyCall
{
    public DateTime Date { get; set; } = Date;

    public override string ToString()
    {
        return $"{nameof(PhoneNumber)}: {Number}, {nameof(Billable)}: {Billable}, {nameof(Date)}: {Date}, {nameof(Duration)}: {Duration}, {nameof(Source)}: {Source}, {nameof(Notes)}: {Notes}";
    }
}
