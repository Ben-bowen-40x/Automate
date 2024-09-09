namespace Automate.Domain.ValueObjects;

public interface IDatedRecord
{
    DateTimeOffset Date { get; set; }
}
