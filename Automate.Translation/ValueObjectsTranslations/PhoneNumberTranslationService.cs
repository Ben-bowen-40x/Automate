using Automate.Domain.ValueObjects;

namespace Automate.Translation.ValueObjectsTranslations;

public static class PhoneNumberTranslationService
{
    // From NumberTypeJson (Infrastructure Object)
    public static PhoneNumber Convert(this INumberTypeJson entity)
    {
        return new(entity.Number);
    }
    // From primitive
    public static PhoneNumber Convert(string? phone)
    {
        return phone is null || !PhoneNumber.TryParse(phone, out PhoneNumber phoneResult) ? new(0) : phoneResult;
    }
}
