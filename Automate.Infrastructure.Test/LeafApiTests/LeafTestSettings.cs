using Automate.Infrastructure.LeafClientService;

namespace Automate.Infrastructure.Test.LeafApiTests;

public class LeafTestSettings : ILeafApiSettings
{
    public string? LeafName { get; set; }
    public string? LeafTokenType { get; set; }
    public string? LeafRefreshToken { get; set; }
    public string? LeafBase { get; set; }
    public string? LeafAcctUuid { get; set; }
    public string? LeafUuid { get; set; }
    public string? LeafThreads { get; set; }
    public string? LeafThreadsEndpoint { get; set; }
    public string? LeafMessagesEndpoint { get; set; }
}
