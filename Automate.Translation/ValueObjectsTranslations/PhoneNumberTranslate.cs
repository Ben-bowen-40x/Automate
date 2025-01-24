using Automate.Domain.ValueObjects;

namespace Automate.Translation.ValueObjectsTranslations;

public static class PhoneNumberTranslate
{
    // Returns a default PhoneNumber type
    internal static PhoneNumber Default => new(PhoneNumber.Default);

    // From NumberTypeJson (Infrastructure Object)
    public static PhoneNumber Convert(this IPhoneNumberTranslate entity)
    {
        return Convert(entity.Number.ToString());
    }
    // From nullable string
    public static PhoneNumber Convert(string? phone)
    {
        return !PhoneNumber.TryParse(phone, out PhoneNumber phoneResult)
            ? Default
            : phoneResult;
    }
    // From long
    public static PhoneNumber Convert(long phone)
    {
        return Convert(phone.ToString());
    }
}
