using Automate.Application.InfrastructureValueObjects;
using Automate.Domain.ValueObjects;
using Automate.Translation.MessageTranslate;
using Automate.Translation.PhoneNumTranslate;
using Automate.Translation.ValueObjectsTranslations;

namespace Automate.Translation.LeafTranslate;

public static class LeafThreadTranslate
{
    public static IMessage Translate(this ILeafThread entity)
    {
        // Get the first chronological message in the list
        List<Msg> messages = entity.Messages is not null && entity.Messages.Length > 0
            ? [.. entity.Messages!]
            : DefaultMsgList();
        Msg first = GetFirstMessage(messages);

        // Extract the Phone Number
        PhoneNumber num = entity.Prospect is not null && entity.Prospect.Cellphone is not null
            ? PhoneNumberTranslate.Translate(entity.Prospect.Cellphone)
            : PhoneNumberTranslate.Default;

        // Extract Date
        DateTimeOffset dto = new(first.Creation);

        // Extract contents
        string contents = MessageInterfaceTranslate.VerifyContents(first.Message);

        // Extract Source
        string source = MessageInterfaceTranslate.VerifySource(first.Source);

        // Cast new message into IMessage
        IMessage rMsg = new Message(num, dto, contents, source);

        return rMsg;

        static List<Msg> DefaultMsgList() => [new() { Message = "This is an empty message", Auto_reply = false, Creation = DateTime.MinValue }];
    }
    internal static Msg GetFirstMessage(List<Msg> messages)
    {
        Msg leastRecent = messages.Last();
        foreach (var m in messages)
        {
            if (DateTime.Compare(m.Creation, leastRecent.Creation) < 0 && m.Direction == "ingress")
            {
                leastRecent = m;
            }
        }
        return leastRecent;
    }
}
