namespace Automate.Infrastructure.Test.LeafApiTests;

public interface ILeafTestSettings
{
    public string? LeafName { get; set; }
    public string? LeafTokenType { get; set; }
    public string? LeafRefreshToken { get; set; }
    public string? LeafBase { get; set; }
    public string? LeafAcctUuid { get; set; }
    public string? LeafUuid { get; set; }
    public string? LeafThreads { get; set; }
    public Uri? LeafThreadsEndpoint(string additions = "")
    {
        Uri? result = default;
        if (LeafBase is not null && LeafThreads is not null)
        {
            result = new(LeafBase + "/" + LeafThreads + additions);
        }
        return result;
    }
}
