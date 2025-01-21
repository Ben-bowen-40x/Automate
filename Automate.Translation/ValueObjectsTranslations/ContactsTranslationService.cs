using Automate.Domain.ValueObjects;
using Automate.Translation.ContactsUpdateTranslations;

namespace Automate.Translation.ValueObjectsTranslations;

public static class ContactsTranslationService
{
    // Contacts Entity (Infrastructure) to Contacts (Domain Value Object)
    public static Contacts Convert(this IContactsEntity entity)
    {
        PhoneNumber number = new(entity.Phone1);
        PhoneNumber number2 = entity.Phone2 is null || entity.Phone2 == string.Empty ? new(0) : new(entity.Phone2);
        return new(number, number2);
    }
}
