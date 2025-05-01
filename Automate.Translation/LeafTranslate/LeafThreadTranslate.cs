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
        // Keep in mind that the messages all come from the same prospect,
        // which means the phone number is in the prospect, not the messages
        PhoneNumber num = ExtractPhoneNumber(entity.Prospect);

        // Extract Date
        DateTimeOffset dto = (entity.Creation - first.Creation).Duration() > TimeSpan.FromSeconds(2)
            ? entity.Creation // This means that the true first message could not be found
            : first.Creation;

        // Extract contents
        string contents = MessageInterfaceTranslate.VerifyContents(first.Message);

        // Extract Source
        string source = MessageInterfaceTranslate.VerifySource(first.Source);

        // Cast new message into IMessage
        IMessage rMsg = new Message(num, dto, contents, source);

        return rMsg;
    }

    #region Internal -- For testing
    internal static Msg GetFirstMessage(IList<Msg> messages)
    {
        Msg leastRecent = new() { Creation = DateTimeOffset.MaxValue };
        foreach (Msg msg in messages)
        {
            bool date = DateTimeOffset.Compare(msg.Creation, leastRecent.Creation) < 0;
            if (date)
                leastRecent = msg;
        }
        return leastRecent;
    }

    internal static List<Msg> VerifyMessages(Msg[]? msgs)
    {
        return msgs is not null && msgs.Length > 0
            ? [.. msgs!]
            : DefaultMsgArr();
    }

    internal static List<Msg> DefaultMsgArr() => [new() { Message = "This is an empty message", Auto_reply = false, Creation = DateTimeOffset.MaxValue }];

    internal static PhoneNumber ExtractPhoneNumber(Prospect? prospect)
    {
        return prospect is not null && prospect.Cellphone is not null
            ? PhoneNumberTranslate.Translate(prospect.Cellphone)
            : PhoneNumberTranslate.Default;
    }
    #endregion
}
