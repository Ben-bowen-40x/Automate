
namespace Automate.Domain.ValueObjects;

public interface IDiscrepancyCall
{
    bool Billable { get; init; }
    DateTime Date { get; set; }
    TimeSpan Duration { get; init; }
    string Notes { get; init; }
    PhoneNumber Number { get; init; }
    string ToString();
}