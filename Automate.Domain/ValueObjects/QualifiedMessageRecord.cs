namespace Automate.Domain.ValueObjects;

public record QualifiedMessageRecord(IMessage Message, ICustomerSubscription Customer, bool Billable, bool IsSalesLead, MessageType Type)
{
    private readonly long _mod = Message.Date.Year % 71;
    private long Divisor => _mod == 0 || _mod == 1
        ? 72
        : _mod;
    public ulong Id => (ulong)
        (
            Message.Date.Year + Message.Date.Month + Message.Date.DayOfYear + Message.Date.Day +
            Message.Date.Hour + Message.Date.Minute + Message.Date.Second +
            (Message.Number.Number / Divisor)
        );
};

public enum MessageType
{
    Pan,
    Leaf,
    LeafRepo,
    ManualWebForm,
    GAdsLeaf,
    GAdsLeafRepo,
    MetaForm,
    Libacion,
    Leased,
    CalliValley,
}
public class MessageTypeText
{
    public const string Text = """
        Pan,
        Leaf,
        LeafRepo,
        ManualWebForm,
        GAdsLeaf,
        GAdsLeafRepo,
        MetaForm,
        Libacion,
        Leased,
        CalliValley,
        """;
}
