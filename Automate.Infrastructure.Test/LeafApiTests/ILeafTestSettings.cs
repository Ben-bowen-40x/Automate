using Automate.Infrastructure.LeafClientService;

namespace Automate.Infrastructure.Test.LeafApiTests;

public interface ILeafTestSettings : ILeafApiSettings
{
    public string? LeafApiTestRepo { get; set; }

    public Uri? LeafThreadsEp(string additions = "")
    {
        Uri? result = default;
        if (LeafBase is not null && LeafThreadsEndpoint is not null)
        {
            result = new(LeafBase + LeafThreadsEndpoint + additions);
        }
        return result;
    }
}
