using Automate.Domain.ValueObjects;

namespace Automate.Translation.MessageTranslate;

public interface IMsgDTONumberLong : IConvert
{
    long Number { get; set; }
    DateTimeOffset Date { get; set; }
    string? Contents { get; set; }
    string? Source { get; set; }
}
