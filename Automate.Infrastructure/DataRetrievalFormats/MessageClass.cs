using Automate.Translation.MessageTranslate;
using CsvHelper.Configuration.Attributes;

namespace Automate.Infrastructure.DataRetrievalFormats;

public class MessageClass : IMsgStrDateTimeOffset
{
    [Name("Number","Phone Number")]
    public string? Number { get; set; }
    [Name("Date", "Date of Message")]
    public DateTimeOffset Date { get; set; }
    [Name("Contents","Message Contents")]
    public string? Contents { get; set; }
    [Name("Source", "Message Source")]
    public string? Source { get; set; }

    public IMessage Convert<IMsgStrDateTimeOffset, IMessage>()
    {
        return (IMessage)this.Translate();
    }
}
