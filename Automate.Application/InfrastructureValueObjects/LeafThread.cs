using Automate.Application.InfrastructureInterfaces;
using Automate.Domain.ValueObjects;

namespace Automate.Application.InfrastructureValueObjects;
public class LeafThread : IConvert
{
    public string? Uuid { get; set; }
    public string? Profile { get; set; }
    public string? Category { get; set; }
    public bool Read { get; set; }
    public bool Spam { get; set; }
    public string? State { get; set; }
    public string[]? Channels { get; set; }
    public DateTime Creation { get; set; }
    public DateTime Modification { get; set; }
    public bool IsCallRequest { get; set; }
    public object[]? Tags { get; set; }
    public Prospect? Prospect { get; set; }
    public Msg[]? Messages { get; set; }
    public Assignee? Assignee { get; set; }
    public IMessage Convert<LeafThread, IMessage>()
    {
        // Get the first chronological message in the list
        List<Msg> messages = Messages is not null && Messages.Length > 0
            ? [.. Messages!]
            : DefaultMsgList;
        Msg first = GetFirstMessage(messages);

        // Extract the Phone Number
        PhoneNumber num = Prospect is not null && Prospect.Cellphone is not null
            ? new(Prospect.Cellphone)
            : new(0);

        // Extract Date
        DateTimeOffset dto = new(first.Creation);

        // Extract contents
        string contents = first.Message is not null
            ? ContentsJoined(first.Message)
            : string.Empty;

        // Extract Source
        string source = first.Source is not null
            ? first.Source
            : string.Empty;

        // Cast new message into IMessage
        List<Message> rlist = [new Message(num, dto, contents, source)];
        List<IMessage> mlist = (List<IMessage>)rlist.Cast<IMessage>();
        IMessage result = mlist[0];

        return result;

        static string ContentsJoined(string contents)
        {
            string s = string.Join('|', contents.Split(',', '"'));
            string strin = string.Join(" | ", s.Split('\n', '\r'));
            return strin;
        }
    }
    private static Msg GetFirstMessage(List<Msg> messages)
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
    private static List<Msg> DefaultMsgList => [new() { Message = "This is an empty message", Auto_reply = false, Creation = DateTime.MinValue }];
}

public class Prospect
{
    public string? Uuid { get; set; }
    public string? First_name { get; set; }
    public string? Last_name { get; set; }
    public string? Cellphone { get; set; }
    public DateTime Creation { get; set; }
    public DateTime Modification { get; set; }
    public bool HasConsented { get; set; }
    public bool Blocked { get; set; }
    public string? Customer { get; set; }
    public string[]? Profiles { get; set; }
}

public class Assignee
{
    public string? Uuid { get; set; }
    public string? First_name { get; set; }
    public string? Last_name { get; set; }
}

public class Msg
{
    public string? Uuid { get; set; }
    public string? Message { get; set; }
    public string? State { get; set; }
    public string? Direction { get; set; }
    public string? Type { get; set; }
    public bool Auto_reply { get; set; }
    public DateTime Creation { get; set; }
    public DateTime Modification { get; set; }
    public DateTime Sent { get; set; }
    public string? Profile { get; set; }
    public string? Thread { get; set; }
    public string? Source { get; set; }
    public Sender? Sender { get; set; }
}

public class Sender
{
    public string? Uuid { get; set; }
    public string? First_name { get; set; }
    public string? Last_name { get; set; }
}