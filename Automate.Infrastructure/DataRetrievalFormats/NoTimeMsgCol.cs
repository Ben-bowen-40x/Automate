using Automate.Translation.MessageTranslate;
using CsvHelper.Configuration.Attributes;

namespace Automate.Infrastructure.DataRetrievalFormats;

public class NoTimeMsgCol : IMsgNoTimeStrUtc
{
    [Name("Phone")]
    public string? NumberStr { get; set; }
    [Name("Date")]
    public string? DateTimeStr { get; set; }
    [Name("Problem")]
    public string? Contents { get; set; }
    [Name("Referring URL")]
    public string? Source { get; set; }

    public IMessage Convert<IMessageNoTimeStr, IMessage>()
    {
        return (IMessage)this.Translate();
    }
}
