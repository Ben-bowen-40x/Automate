using Automate.Domain.ValueObjects;

namespace Automate.Infrastructure.MessageLeadsService.JsonMaps;

public class NumberTypeJson
{
    public bool IsDefault { get; set; }
    public long Number { get; set; }
    public PhoneNumber Convert()
    {
        return new(Number);
    }
}
