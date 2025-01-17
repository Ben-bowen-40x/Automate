using Automate.Translation.ValueObjectsTranslations;

namespace Automate.Infrastructure.MessageLeadsService.JsonMaps;

public class NumberTypeJson : INumberTypeJson
{
    public bool IsDefault { get; set; }
    public long Number { get; set; }
}
