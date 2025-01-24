using Automate.Translation.ValueObjectsTranslations;

namespace Automate.Infrastructure.MessageLeadsService.JsonMaps;

public class NumberTypeJson : IPhoneNumberTranslate
{
    public bool IsDefault { get; set; }
    public long Number { get; set; }
}
