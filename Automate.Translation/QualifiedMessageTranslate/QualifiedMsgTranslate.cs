using Automate.Domain.ValueObjects;
using Automate.Translation.MessageTranslate;
using Automate.Translation.CustomerTranslate;

namespace Automate.Translation.QualifiedMessageTranslate;

public static class QualifiedMsgTranslate
{
    /// <summary>
    /// <para>Converts <see cref="IQualifiedMessageTranslate"/> into <see cref="QualifiedMessageRecord"/></para>
    /// <para>DO NOT change the name of this method, as <see cref="IQualifiedMessageTranslate"/> already contains a method called <see cref="IQualifiedMessageTranslate.Convert()"/> which cannot be renamed</para>
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public static QualifiedMessageRecord Converter(this IQualifiedMessageTranslate entity)
    {
        IMessage message = MessageInterfaceTranslate.Convert(entity);
        ICustomerSubscription customer = CustomerSubscriptionTranslate.Convert(entity);

        return new QualifiedMessageRecord(message, customer, entity.ImLead, entity.SalesLead);
    }
}
