using CsvHelper.Configuration.Attributes;
using Automate.Translation.InfrastructureInterfaces.Message;
using Automate.Translation.ValueObjectsTranslations;

namespace Automate.Infrastructure.MessageLeadsService.CsvMaps;

public class SplitDateMountainOffsetMsgCol : IMsgTimeStr
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

    public IMessage Convert<IMsgTimeStr, IMessage>()
    {
        return (IMessage)this.Convert();
    }
}
