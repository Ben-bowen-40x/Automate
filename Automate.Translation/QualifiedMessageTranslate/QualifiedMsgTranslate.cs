using Automate.Domain.ValueObjects;
using Automate.Translation.MessageTranslate;
using Automate.Translation.CustomerTranslate;

namespace Automate.Translation.QualifiedMessageTranslate;

public static class QualifiedMsgTranslate
{
    /// <summary>
    /// <para>Converts <see cref="IQualifiedMessageTranslate"/> into <see cref="QualifiedMessageRecord"/></para>
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public static QualifiedMessageRecord Translate(this IQualifiedMessageTranslate entity, MessageType type)
    {
        IMessage message = MessageInterfaceTranslate.Translate(entity);
        ICustomerSubscription customer = CustomerSubscriptionTranslate.Translate(entity);

        return new QualifiedMessageRecord(message, customer, entity.ImLead, entity.SalesLead, type);
    }
}
