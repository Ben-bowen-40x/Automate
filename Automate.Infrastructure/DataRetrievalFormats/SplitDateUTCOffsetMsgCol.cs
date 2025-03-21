using CsvHelper.Configuration.Attributes;
using Automate.Translation.MessageTranslate;
using Automate.Translation.ValueObjectTranslate;

namespace Automate.Infrastructure.DataRetrievalFormats;

public class SplitDateUTCOffsetMsgCol : IMsgZoneEnumStr
{
    [Name("phone_number", "Phone Number")]
    public string? Number { get; set; }
    [Name("date_submitted", "Date")]
    public string? Date { get; set; }
    [Name("time_submitted", "Time")]
    public string? Time { get; set; }
    [Name("how_can_we_help", "Message")]
    public string? Contents { get; set; }
    [Name("page_name", "Commercial?")]
    public string? Source { get; set; }

    public TimeZoneEnum TimeZone => TimeZoneEnum.Utc;

    public IMessage Convert<IMsgZoneEnumStr, IMessage>()
    {
        return (IMessage)this.Translate();
    }
}
