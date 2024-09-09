namespace Automate.Domain.ValueObjects;

public interface ICallRecord : IDatedRecord
{
    PhoneNumber Number { get; init; }
    bool Billable { get; init; }
    string ToString();
}
