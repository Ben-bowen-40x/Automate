using Automate.Domain.ValueObjects;

namespace Automate.Translation.MessageTranslate;

public interface IMsgDTOStr : IConvert
{
    string? Number { get; set; }
    string? DateTimeOffsetStr { get; set; }
    string? Contents { get; set; }
    string? Source { get; set; }

}
