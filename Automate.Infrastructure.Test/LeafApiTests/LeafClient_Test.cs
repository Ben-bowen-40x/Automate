using Automate.Domain.ValueObjects;
using Automate.Infrastructure.DataRetrievalFormats;
using Automate.Infrastructure.JsonManipulationService;
using Automate.Infrastructure.LeafClientService;
using Automate.Infrastructure.Test.TestConfigurations;
using CSharpFunctionalExtensions;
using System.Net.Http.Json;

namespace Automate.Infrastructure.Test.LeafApiTests;

public class LeafClient_Test
{
    #region Ctor and Readonly values
    public LeafClient_Test()
    {
        Config = new InfraTestConfiguration().TestSettings;

        Client = new HttpClient();
        Client.DefaultRequestHeaders.Add("Accept", "application/json");
        Client.DefaultRequestHeaders.Add("Authorization", Config.LeafTokenType);
        Client.BaseAddress = new(Config.LeafBase!);
    }
    private ILeafTestSettings Config { get; }
    private HttpClient Client { get; }
    private const string SkipMsg = "There is no need to spam our API";
    private Uri Url(int offset, int limit) => new($"{Config.LeafBase}{Config.LeafThreadsEndpoint}?offset={offset}&limit={limit}");
    #endregion

    #region TestHttpClientAsync
    [
        Theory
        (Skip = SkipMsg)
        ,
        InlineData(0, 10)
    ]
    public async Task TestHttpClientAsync(int offset, int limit)
    {
        Uri url = Config.LeafThreadsEp($"?offset={offset}&limit={limit}")!;
        HttpResponseMessage response = await Client.GetAsync(url);
        var headers = response.Headers;
        LeafThread[]? result = await response.Content.ReadFromJsonAsync<LeafThread[]>();
        IEnumerable<IMessage> messages = result is not null && result.Length > 0
            ? result.Select(r => r.Convert<LeafThread, IMessage>())
            : [];
    }
    #endregion

    #region TestLeafApiService_GetAsync
    [
        Theory
        (Skip = SkipMsg)
        ,
        InlineData(0, 1)
    ]
    public async Task TestLeafApiService_GetAsync(int offset, int limit)
    {
        await LeafApiService.GetAsync<List<LeafThread>>(Url(offset, limit), Client);
    }
    #endregion

    #region TestLeafApiService_GetAsync
    [
        Theory
        //(Skip = SkipMsg)
        ,
        InlineData(0, 1)
    ]
    public async Task TestLeafApiService_GetStringAsync(int offset, int limit)
    {
        await LeafApiService.GetAsync(Url(offset, limit), Client);
    }
    #endregion

    #region TestLeaf_RefreshRepoFully_PaginationMakesSense
    [
        Theory
        (Skip = SkipMsg)
        ,
        InlineData(0, 1000),
    //InlineData(5000,1000)
    ]
    public async Task TestLeaf_RefreshRepoFully_PaginationMakesSense(int offset, int limit)
    {
        bool resume = true;
        while (resume)
        {
            // Create the new repo file name
            string now = DateTime.Now.ToString(DateTimeStrings.FileDateTimeFormat2);

            // Call the api
            Result<List<LeafThread>> result = await LeafApiService.GetAsync<List<LeafThread>>(Url(offset, limit), Client);
            if (result.IsSuccess)
            {
                List<LeafThread> value = result.Value;
                int ct = value.Count;
                FileInfo repo = new($"{Config.LeafApiTestRepo}/LeafTestRepo_{ct}_{now}.json");
                List<IMessage> messages = value.Select(v => v.Convert<LeafThread, IMessage>()).ToList();
                JsonService.WriteToFile(repo, messages);
                resume = ct == limit;
            }
            else
            {
                var error = result.Error;
            }
            offset += limit;
            Thread.Sleep(10000);
        }
    }
    #endregion

    #region TestLeaf_refreshRepoFully_PaginationMakesSense_V2
    [
        Theory
        (Skip = SkipMsg)
        ,
        InlineData(0, 1000),
    //InlineData(5000,1000)
    ]
    public async Task TestLeaf_RefreshRepoFully_PaginationMakesSense_V2(int offset, int limit)
    {

        // Create the new repo file name
        string now = DateTime.Now.ToString(DateTimeStrings.FileDateTimeFormat2);
        FileInfo repoThread = new($"{Config.LeafApiTestRepo}/LeafTestRepo_LeafThread_{now}.json");
        FileInfo repoMessage = new($"{Config.LeafApiTestRepo}/LeafTestRepo_LeafMessage_{now}.json");

        int c = limit * 10;
        List<LeafThread> masterList = new(c);
        List<IMessage> messageList = new(c);

        bool resume = true;
        while (resume)
        {
            // Call the api
            Result<List<LeafThread>> result = await LeafApiService.GetAsync<List<LeafThread>>(Url(offset, limit), Client);
            if (result.IsSuccess)
            {
                List<LeafThread> value = result.Value;
                value.ForEach(v => masterList.Add(v));

                List<IMessage> messages = value.Select(v => v.Convert<LeafThread, IMessage>()).ToList();
                messages.ForEach(m => messageList.Add(m));

                resume = value.Count == limit;
            }
            else
            {
                var error = result.Error;
            }
            offset += limit;
            Thread.Sleep(10000);
        }

        JsonService.WriteToFile(repoThread, masterList);
        JsonService.WriteToFile(repoMessage, messageList);
    }
    #endregion
}
