using CsvHelper.Configuration.Attributes;
using Automate.Translation.ValueObjectsTranslations;
using Automate.Translation.MessageTranslate;

namespace Automate.Infrastructure.MessageLeadsService.CsvMaps;

public class UnifiedDateUnchangedOffset_SeparateGclid_SourceCantBeEmpty_MsgCol : IMsgDTOStrNonEmptySource
{
    [Name("Prospect Cellphone", "Phone Number", "Number")]
    public string? NumberStr { get; set; }
    [Name("Creation", "Message Creation", "Date")]
    public string? DateTimeOffsetStr { get; set; }
    [Name("Message", "Contents")]
    public string? Contents { get; set; }
    [Name("Message Source", "Source")]
    public string? Source { get; set; }
    public SourceComponent Separator => SourceComponent.Gclid;
    public IMessage Convert<IMsgDTOStrIsolateSource, IMessage>()
    {
        return (IMessage)this.Convert();
    }
}

