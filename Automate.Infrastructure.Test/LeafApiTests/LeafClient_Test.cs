using Automate.Domain.ValueObjects;
using Automate.Infrastructure.LeafClientService;
using Automate.Infrastructure.Test.TestConfigurations;
using System.Net.Http.Json;

namespace Automate.Infrastructure.Test.LeafApiTests;

public class LeafClient_Test
{
    public LeafClient_Test()
    {
        _config = new InfrastructureConfiguration().Settings;
    }
    private IInfrastructureTestSettings _config { get; }

    [
        Theory,
        InlineData(0, 10)
    ]
    public async void TestHttpClientAsync(int offset, int limit)
    {
        var client = new HttpClient();
        client.DefaultRequestHeaders.Add("Accept", "application/json");
        client.DefaultRequestHeaders.Add("Authorization", _config.LeafTokenType);
        client.BaseAddress = new(_config.LeafBase!);

        HttpResponseMessage response = await client.GetAsync(_config.LeafThreadsEndpoint($"?offset={offset}&limit={limit}"));
        LeafThread[]? result = await response.Content.ReadFromJsonAsync<LeafThread[]>();
        IEnumerable<IMessage> messages = result is not null && result.Length > 0
            ? result.Select(r => r.ConvertToMessage())
            : [];
    }
    [
        Theory,
        InlineData(0, 10)
    ]
    public async void TestClientGenericAsync(int offset, int limit)
    {

    }
}
