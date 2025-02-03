using Automate.Application.InfrastructureValueObjects;
using Automate.Domain.ValueObjects;
using Automate.Translation.MessageTranslate;
using Automate.Translation.PhoneNumTranslate;

namespace Automate.Translation.LeafTranslate;

public static class LeafThreadTranslate
{
    public static IMessage Translate(this ILeafThread entity)
    {
        // Get the first chronological message in the list
        List<Msg> messages = VerifyMessages(entity.Messages);
        Msg first = GetFirstMessage(messages);

        // Extract the Phone Number
        PhoneNumber num = ExtractPhoneNumber(entity.Prospect);

        // Extract Date
        DateTimeOffset dto = new(first.Creation);

        // Extract contents
        string contents = MessageInterfaceTranslate.VerifyContents(first.Message);

        // Extract Source
        string source = MessageInterfaceTranslate.VerifySource(first.Source);

        // Cast new message into IMessage
        IMessage rMsg = new Message(num, dto, contents, source);

        return rMsg;
    }

    #region Internal -- For testing
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

    internal static List<Msg> VerifyMessages(Msg[]? msgs)
    {
        return msgs is not null && msgs.Length > 0
            ? [.. msgs!]
            : DefaultMsgArr();
    }
    
    internal static List<Msg> DefaultMsgArr() => [new() { Message = "This is an empty message", Auto_reply = false, Creation = DateTime.MinValue }];

    internal static PhoneNumber ExtractPhoneNumber(Prospect? prospect)
    {
        return prospect is not null && prospect.Cellphone is not null
            ? PhoneNumberTranslate.Translate(prospect.Cellphone)
            : PhoneNumberTranslate.Default;
    }
    #endregion
}
