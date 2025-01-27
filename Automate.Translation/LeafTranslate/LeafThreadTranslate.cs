using Automate.Application.InfrastructureValueObjects;
using Automate.Domain.ValueObjects;
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
            ? new(entity.Prospect.Cellphone)
            : new(0);

        // Extract Date
        DateTimeOffset dto = new(first.Creation);

        // Extract contents
        string contents = first.Message is not null
            ? TSH.ContentsJoined(first.Message)
            : string.Empty;

        // Extract Source
        string source = first.Source is not null
            ? first.Source
            : string.Empty;

        // Cast new message into IMessage
        IMessage rMsg = new Message(num, dto, contents, source);

        return rMsg;

        Msg GetFirstMessage(List<Msg> messages)
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
        List<Msg> DefaultMsgList() => [new() { Message = "This is an empty message", Auto_reply = false, Creation = DateTime.MinValue }];
    }
}
