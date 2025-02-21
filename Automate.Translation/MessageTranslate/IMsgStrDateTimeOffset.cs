using Automate.Domain.ValueObjects;

namespace Automate.Translation.MessageTranslate;

public interface IMsgStrDateTimeOffset : IConvert
{
    string? Number { get; set; }
    DateTimeOffset Date { get; set; }
    string? Contents { get; set; }
    string? Source { get; set; }
}
