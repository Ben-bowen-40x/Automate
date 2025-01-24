using Automate.Translation.MessageTranslate;
using Automate.Translation.ValueObjectsTranslations;
using CsvHelper.Configuration.Attributes;

namespace Automate.Infrastructure.MessageLeadsService.CsvMaps;

public class MessageClass : IMsgStrDateTimeOffset
{
    [Name("Number")]
    public string? Number { get; set; }
    [Name("Date")]
    public DateTimeOffset Date { get; set; }
    [Name("Contents")]
    public string? Contents { get; set; }
    [Name("Source")]
    public string? Source { get; set; }

    public IMessage Convert<IMsgStrDateTimeOffset, IMessage>()
    {
        return (IMessage)this.Convert();
    }
}