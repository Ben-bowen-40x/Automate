using CsvHelper.Configuration.Attributes;
using Automate.Translation.MessageTranslate;
using Automate.Translation.ValueObjectTranslate;

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
    [Name("Account Name", "Email")]
    public string? Source { get; set; }

    public TimeZoneEnum TimeZone => TimeZoneEnum.Eastern;

    public IMessage Convert<IMsgTimeStr, IMessage>()
    {
        return (IMessage)this.Translate();
    }
}
