using Automate.Domain.ValueObjects;
using Automate.Translation.ValueObjectTranslate;

namespace Automate.Translation.MessageTranslate;

public interface IMsgZoneEnumStr : IConvert
{
    public string? Number { get; set; }
    public string? Date { get; set; }
    public string? Time { get; set; }
    public string? Contents { get; set; }
    public string? Source { get; set; }
    public TimeZoneEnum TimeZone { get; }
}
