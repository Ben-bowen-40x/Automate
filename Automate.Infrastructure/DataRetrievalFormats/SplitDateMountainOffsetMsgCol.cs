using CsvHelper.Configuration.Attributes;
using Automate.Translation.ValueObjectsTranslations;
using Automate.Translation.MessageTranslate;

namespace Automate.Infrastructure.DataRetrievalFormats;

public class SplitDateMountainOffsetMsgCol : IMsgZoneEnumStr
{
    [Name("Customer #")]
    public string? Number { get; set; }
    [Name("Date")]
    public string? Date { get; set; }
    [Name("Time")]
    public string? Time { get; set; }
    [Name("FormCustomFields")]
    public string? Contents { get; set; }
    [Name("Account Name")]
    public string? Source { get; set; }

    public TimeZoneEnum TimeZone => TimeZoneEnum.Mountain;

    public IMessage Convert<IMsgTimeStr, IMessage>()
    {
        return (IMessage)this.Translate();
    }
}
