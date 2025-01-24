using Automate.Translation.InfrastructureInterfaces.Message;
using Automate.Translation.ValueObjectsTranslations;

namespace Automate.Infrastructure.MessageLeadsService.CsvMaps;

public class MessageClass : IMsgStrDateTimeOffset
{
    public string? NumberStr { get; set; }
    public DateTimeOffset Date { get; set; }
    public string? Contents { get; set; }
    public string? Source { get; set; }

    public IMessage Convert<IMsgStrDateTimeOffset, IMessage>()
    {
        return (IMessage)this.Convert();
    }
}