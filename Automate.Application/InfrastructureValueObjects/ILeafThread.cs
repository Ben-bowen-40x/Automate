using Automate.Domain.ValueObjects;

namespace Automate.Application.InfrastructureValueObjects;
public interface ILeafThread : IConvert
{
    public Msg[]? Messages { get; set; }
    public Prospect? Prospect { get; set; }
    public DateTime Creation { get; set; }
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