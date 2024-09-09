namespace Automate.Domain.ValueObjects;

public interface IMessage : IDatedRecord
{
    PhoneNumber Number { get; init; }
    string Contents { get; init; }
    string? Source { get; set; }
    string ToString();
}