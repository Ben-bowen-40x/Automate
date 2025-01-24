using Automate.Domain.ValueObjects;

namespace Automate.Translation.InfrastructureInterfaces.MessageTranslate;

public interface IMsgStrDateTimeOffset : IConvert
{
    string? NumberStr { get; set; }
    DateTimeOffset Date { get; set; }
    string? Contents { get; set; }
    string? Source { get; set; }
}
