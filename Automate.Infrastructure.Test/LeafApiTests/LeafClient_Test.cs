using Automate.Domain.ValueObjects;
using Automate.Infrastructure.LeafClientService;
using Automate.Infrastructure.Test.TestConfigurations;
using System.Net.Http.Json;

namespace Automate.Infrastructure.Test.LeafApiTests;

public class LeafClient_Test
{
    public LeafClient_Test()
    {
        Config = (ILeafTestSettings)new InfraTestConfiguration().TestSettings;

        Client = new HttpClient();
        Client.DefaultRequestHeaders.Add("Accept", "application/json");
        Client.DefaultRequestHeaders.Add("Authorization", Config.LeafTokenType);
        Client.BaseAddress = new(Config.LeafBase!);
    }
    private ILeafTestSettings Config { get; }
    private HttpClient Client { get; }

    [
        Theory,
        InlineData(0, 10)
    ]
    public async void TestHttpClientAsync(int offset, int limit)
    {
        Uri url = Config.LeafThreadsEp($"?offset={offset}&limit={limit}")!;
        HttpResponseMessage response = await Client.GetAsync(url);
        var headers = response.Headers;
        LeafThread[]? result = await response.Content.ReadFromJsonAsync<LeafThread[]>();
        IEnumerable<IMessage> messages = result is not null && result.Length > 0
            ? result.Select(r => r.ConvertToMessage())
            : [];
    }
    [
        Theory,
        InlineData(0, 1)
    ]
    public void TestLeafAsync(int offset, int limit)
    {

        var result = LeafApiService.GetAsync<LeafThread>(Client, Config, offset, limit);
    }
}
