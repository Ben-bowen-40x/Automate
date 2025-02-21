using Automate.Domain.ValueObjects;

namespace Automate.Translation.MessageTranslate;

public interface IMsgNoTimeStrUtc : IConvert
{
    string? NumberStr { get; set; }
    string? DateTimeStr { get; set; }
    string? Contents { get; set; }
    string? Source { get; set; }
}
