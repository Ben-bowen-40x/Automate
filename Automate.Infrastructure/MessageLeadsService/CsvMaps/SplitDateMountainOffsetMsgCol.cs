using CsvHelper.Configuration.Attributes;
using Automate.Translation.InfrastructureInterfaces.Message;
using Automate.Translation.ValueObjectsTranslations;
using Automate.Translation.DateTimeConvertService;

namespace Automate.Infrastructure.MessageLeadsService.CsvMaps;

public class SplitDateMountainOffsetMsgCol : IMsgZoneEnumStr
{
    [Name("Customer #")]
    public string? NumberStr { get; set; }
    [Name("Date")]
    public string? DateStr { get; set; }
    [Name("Time")]
    public string? TimeStr { get; set; }
    [Name("FormCustomFields")]
    public string? Contents { get; set; }
    [Name("Account Name")]
    public string? Source { get; set; }

    public TimeZoneEnum TimeZone => TimeZoneEnum.Mountain;

    public IMessage Convert<IMsgTimeStr, IMessage>()
    {
        return (IMessage)this.Convert();
    }
}
