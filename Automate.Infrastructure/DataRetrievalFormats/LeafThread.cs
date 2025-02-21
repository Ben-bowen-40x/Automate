using Automate.Application.InfrastructureValueObjects;
using Automate.Translation.LeafTranslate;

namespace Automate.Infrastructure.DataRetrievalFormats;

public class LeafThread : ILeafThread
{
    public string? Uuid { get; set; }
    public string? Profile { get; set; }
    public string? Category { get; set; }
    public bool Read { get; set; }
    public bool Spam { get; set; }
    public string? State { get; set; }
    public string[]? Channels { get; set; }
    public DateTimeOffset Creation { get; set; }
    public DateTimeOffset Modification { get; set; }
    public bool IsCallRequest { get; set; }
    public object[]? Tags { get; set; }
    public Prospect? Prospect { get; set; }
    public Msg[]? Messages { get; set; }
    public Assignee? Assignee { get; set; }

    public IMessage Convert<ILeafThread, IMessage>()
    {
        return (IMessage)this.Translate();
    }
}
