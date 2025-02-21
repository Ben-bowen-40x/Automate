using Automate.Domain.ValueObjects;

namespace Automate.Translation.MessageTranslate;

public interface IMsgDTOStrNonEmptySource : IConvert
{
    public string? NumberStr { get; set; }
    public string? DateTimeOffsetStr { get; set; }
    public string? Contents { get; set; }
    public string? Source { get; set; }
    public SourceComponent Separator { get; }
}
