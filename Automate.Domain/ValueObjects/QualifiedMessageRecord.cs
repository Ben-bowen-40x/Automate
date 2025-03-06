namespace Automate.Domain.ValueObjects;

public record QualifiedMessageRecord(IMessage Message, ICustomerSubscription Customer, bool Billable, bool IsSalesLead, MessageType Type)
{
    private readonly ulong _id = (ulong)(
        Message.Number.Number + 
        Message.Date.Year + Message.Date.Month + Message.Date.DayOfYear + Message.Date.Hour + Message.Date.Minute + Message.Date.Second + Message.Date.Offset.Minutes +
        Customer.Date.Year + Customer.Date.Month + Customer.Date.DayOfYear + Customer.Date.Hour + Customer.Date.Minute + Customer.Date.Second + Customer.Date.Offset.Minutes
        );
    public ulong Id => _id;
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
        """;
}
