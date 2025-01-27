using CsvHelper.Configuration.Attributes;
using Automate.Translation.ValueObjectsTranslations;
using Automate.Translation.MessageTranslate;

namespace Automate.Infrastructure.DataRetrievalFormats;

public class SplitDateUTCOffsetMsgCol : IMsgZoneEnumStr
{
    [Name("phone_number")]
    public string? Number { get; set; }
    [Name("date_submitted")]
    public string? Date { get; set; }
    [Name("time_submitted")]
    public string? Time { get; set; }
    [Name("how_can_we_help")]
    public string? Contents { get; set; }
    [Name("page_name")]
    public string? Source { get; set; }

    public TimeZoneEnum TimeZone => TimeZoneEnum.Utc;

    public IMessage Convert<IMsgZoneEnumStr, IMessage>()
    {
        return (IMessage)this.Translate();
    }
}
