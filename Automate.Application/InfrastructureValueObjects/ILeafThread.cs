using Automate.Domain.ValueObjects;

namespace Automate.Application.InfrastructureValueObjects;
public interface ILeafThread : IConvert
{
    string? Uuid { get; set; }
    Msg[]? Messages { get; set; }
    Prospect? Prospect { get; set; }
    DateTimeOffset Creation { get; set; }
}

public interface IProspect
{
    public string? Uuid { get; set; }
    public string? First_name { get; set; }
    public string? Last_name { get; set; }
    public string? Cellphone { get; set; }
    public DateTimeOffset Creation { get; set; }
    public DateTimeOffset Modification { get; set; }
    public bool Consent { get; set; }
    public bool Blocked { get; set; }
    public string? Customer { get; set; }
    public string[]? Profiles { get; set; }
}
public class Prospect : IProspect
{
    public string? Uuid { get; set; }
    public string? First_name { get; set; }
    public string? Last_name { get; set; }
    public string? Cellphone { get; set; }
    public DateTimeOffset Creation { get; set; }
    public DateTimeOffset Modification { get; set; }
    public bool Consent { get; set; }
    public bool Blocked { get; set; }
    public string? Customer { get; set; }
    public string[]? Profiles { get; set; }
}

public interface IAssignee
{
    public string? Uuid { get; set; }
    public string? First_name { get; set; }
    public string? Last_name { get; set; }
}
public class Assignee : IAssignee
{
    public string? Uuid { get; set; }
    public string? First_name { get; set; }
    public string? Last_name { get; set; }
}

public interface IMsg
{
    public string? Uuid { get; set; }
    public string? Message { get; set; }
    public string? State { get; set; }
    public string? Direction { get; set; }
    public string? Type { get; set; }
    public bool Auto_reply { get; set; }
    public DateTimeOffset Creation { get; set; }
    public DateTimeOffset Modification { get; set; }
    public DateTimeOffset Sent { get; set; }
    public string? Profile { get; set; }
    public string? Thread { get; set; }
    public string? Source { get; set; }
    public Sender? Sender { get; set; }
}
public class Msg : IMsg
{
    public string? Uuid { get; set; }
    public string? Message { get; set; }
    public string? State { get; set; }
    public string? Direction { get; set; }
    public string? Type { get; set; }
    public bool Auto_reply { get; set; }
    public DateTimeOffset Creation { get; set; }
    public DateTimeOffset Modification { get; set; }
    public DateTimeOffset Sent { get; set; }
    public string? Profile { get; set; }
    public string? Thread { get; set; }
    public string? Source { get; set; }
    public Sender? Sender { get; set; }
}

public interface ISender
{
    public string? Uuid { get; set; }
    public string? First_name { get; set; }
    public string? Last_name { get; set; }
}
public class Sender : ISender
{
    public string? Uuid { get; set; }
    public string? First_name { get; set; }
    public string? Last_name { get; set; }
}