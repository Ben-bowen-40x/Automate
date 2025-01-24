using CsvHelper.Configuration.Attributes;
using Automate.Translation.ValueObjectsTranslations;
using Automate.Translation.InfrastructureInterfaces.MessageTranslate;

namespace Automate.Infrastructure.MessageLeadsService.CsvMaps;

public class UnifiedDateUtc_SplitPhone : IMsgDTOStr
{
    [Name("phone_number")]
    public string? Number { get; set; }
    [Name("created_time")]
    public string? DateTimeOffsetStr { get; set; }
    [Name("what_bugs_are_you_having_trouble_with?")]
    public string? Contents { get; set; }
    [Name("zip_code")]
    public string? Source { get; set; }
    public IMessage Convert<IMsgDTOStr, IMessage>()
    {
        return (IMessage)this.Convert();
    }
}

