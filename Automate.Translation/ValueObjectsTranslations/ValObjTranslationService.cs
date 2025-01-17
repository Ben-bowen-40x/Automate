using Automate.Domain.ValueObjects;

namespace Automate.Translation.ValueObjectsTranslations;

public static class ValObjTranslationService
{
    // From NumberTypeJson (Infrastructure Object) to PhoneNumber (Domain Value Object)
    public static PhoneNumber Convert(this INumberTypeJson entity)
    {
        return new(entity.Number);
    }
}
