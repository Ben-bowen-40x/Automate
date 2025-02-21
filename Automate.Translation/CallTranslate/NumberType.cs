using Automate.Translation.PhoneNumTranslate;

namespace Automate.Translation.CallTranslate;

public class NumberType : IPhoneNumberTranslate
{
    public bool IsDefault { get; set; }
    public long Number { get; set; }
}
