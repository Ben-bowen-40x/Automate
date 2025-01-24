using CsvHelper.Configuration.Attributes;
using Automate.Translation.InfrastructureInterfaces.Message;
using Automate.Translation.ValueObjectsTranslations;

namespace Automate.Infrastructure.MessageLeadsService.CsvMaps;

public class UnifiedDateUtc_SplitPhone : IMsgDTOStr
{
    [Name("phone_number")]
    public string? NumberStr { get; set; }
    [Name("created_time")]
    public string? DateStr { get; set; }
    [Name("what_bugs_are_you_having_trouble_with?")]
    public string? Contents { get; set; }
    [Name("zip_code")]
    public string? Source { get; set; }
    public IMessage Convert<IMsgDTOStr, IMessage>()
    {
        return (IMessage)this.Convert();
    }
}

