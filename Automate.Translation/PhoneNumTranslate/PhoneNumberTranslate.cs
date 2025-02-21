using Automate.Domain.ValueObjects;

namespace Automate.Translation.PhoneNumTranslate;

public static class PhoneNumberTranslate
{
    // Returns a default PhoneNumber type
    internal static PhoneNumber Default => new(PhoneNumber.Default);

    // From NumberTypeJson (Infrastructure Object)
    public static PhoneNumber Translate(this IPhoneNumberTranslate? entity)
    {
        return entity is not null ? Translate(entity.Number.ToString()) : Default;
    }
    
    // From nullable string
    public static PhoneNumber Translate(string? phone)
    {
        return !PhoneNumber.TryParse(phone, out PhoneNumber phoneResult)
            ? Default
            : phoneResult;
    }
    
    // From long
    public static PhoneNumber Translate(long phone)
    {
        return Translate(phone.ToString());
    }
}
