using Automate.Domain.ValueObjects;
using Automate.Translation.PhoneNumTranslate;

namespace Automate.Translation.ContactTranslate;

public static class ContactsTranslate
{
    /// <summary>
    /// <para>Accepts <paramref name="entity"/> of type <see cref="IContactsEntity"/> and converts it into <see cref="Contact"/></para>
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public static Contact Translate(this IContactsEntity entity)
    {
        PhoneNumber number = PhoneNumberTranslate.Translate(entity.Phone1);
        PhoneNumber number2 = PhoneNumberTranslate.Translate(entity.Phone2);
        return new(number, number2);
    }
}
