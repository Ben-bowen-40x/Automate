using Automate.Translation.InfrastructureInterfaces.Message;
using Automate.Translation.ValueObjectsTranslations;
using CsvHelper.Configuration.Attributes;

namespace Automate.Infrastructure.MessageLeadsService.CsvMaps;

public class NoTimeMsgCol : IMsgNoTimeStr
{
    [Name("Phone")]
    public string? Number { get; set; }
    [Name("Date")]
    public string? Date { get; set; }
    [Name("Problem")]
    public string? Contents { get; set; }
    [Name("Referring URL")]
    public string? Source { get; set; }

    public IMessage Convert<IMessageNoTimeStr, IMessage>()
    {
        return (IMessage)this.Convert();
    }
}
