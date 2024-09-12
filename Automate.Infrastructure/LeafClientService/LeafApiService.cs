using Automate.Application.InfrastructureInterfaces;
using Automate.Domain.SolutionFunctionality;
using Automate.Domain.ValueObjects;
using Automate.Infrastructure.CsvService;
using Automate.Infrastructure.JsonService;
using Automate.Infrastructure.MessageLeadsService.CsvMaps;
using CSharpFunctionalExtensions;
using System.Net.Http.Json;

namespace Automate.Infrastructure.LeafClientService;

public class LeafApiService(ILeafApiSettings settings) : ILeafApiService
{
    #region Setup
    public HttpClient GetClient(IHttpClientFactory factory)
    {
        var client = factory.CreateClient(settings.LeafName!);
        return client;
    }
    internal static Uri LeafThreadUrl(ILeafApiSettings settings, int offset, int limit) => new($"{settings.LeafBase}{settings.LeafThreadsEndpoint}?offset={offset}&limit={limit}");
    internal Uri LeafThreadUrl(int offset, int limit) => new($"{settings.LeafBase}{settings.LeafThreadsEndpoint}?offset={offset}&limit={limit}");
    #endregion

    #region Internal
    internal static async Task<Result<T>> GetAsync<T>(Uri url, HttpClient client)
    {
        // Attempt to make the call
        try
        {
            HttpResponseMessage response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                T? value = await response.Content.ReadFromJsonAsync<T>();
                if (value is not null)
                {
                    return value!;
                }

                string str = await response.Content.ReadAsStringAsync();
                string error = str is null || str.Length == 0 || str == string.Empty
                    ? "Parsing failure. The process of reading the results from Json failed. The results somehow became null."
                    : str;

                return Result.Failure<T>(error);
            }
            return Result.Failure<T>(response.ReasonPhrase);
        }
        catch (Exception ex)
        {
            return Result.Failure<T>(ex.Message);
        }
    }

    internal static async Task<Result<string>> GetAsync(Uri url, HttpClient client)// 1000=largest page size
    {
        // Attempt to make the call
        try
        {
            HttpResponseMessage response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                string result = await response.Content.ReadAsStringAsync();
                if (result is null || result.Length == 0 || result == string.Empty)
                {
                    return Result.Failure<string>("Parsing failure. The process of reading the results from Json failed. The results somehow became null.");
                }

                return result;
            }
            return Result.Failure<string>(response.ReasonPhrase);
        }
        catch (Exception ex)
        {
            return Result.Failure<string>(ex.Message);
        }
    }

    internal List<IMessage> RetrieveMessageRepo(string msgRepoLoc = "")
    {
        // Check loc string
        string repo = msgRepoLoc == string.Empty || !File.Exists(msgRepoLoc)
            ? MessageRepoLocation
            : msgRepoLoc;

        // Check if location exists
        if (!File.Exists(repo))
            File.WriteAllText(repo, "");

        // Retrieve contents
        List<MessageClass> content = CsvRW.ParseFromCsv<MessageClass>(repo);
        var conversion = content.Select(c => c.ConvertToMessage()).ToList();
        return conversion;
    }

    internal List<LeafThread> RetrieveLeafRepo(string leafRepo = "")
    {
        // Check location string
        string repo = leafRepo == string.Empty || !File.Exists(leafRepo)
            ? LeafRepoLocation
            : leafRepo;

        // Check if location exists
        if (!File.Exists(repo))
            File.WriteAllText(repo, "");

        try
        {
            // Retrieve contents
            List<LeafThread> content = JsonRW.DeserializeFile<LeafThread>(repo);
            return content;
        }
        catch { return []; }

    }
    #endregion

    #region Implementation
    private static string? _msgRepoLoc;
    private string? _leafRepoLoc;
    public static string MessageRepoLocation => _msgRepoLoc ??= FolderFinder.GetLocalFile(nameof(Infrastructure), ".info/ApiRepos/", "LeafMessages.csv");
    private string LeafRepoLocation => _leafRepoLoc ??= FolderFinder.GetLocalFile(nameof(Infrastructure), ".info/ApiRepos/", "LeafThreads.json");

    public async Task<Result<List<LeafThread>>> GetLeafThreadsAsync(HttpClient client, int offset = 0, int errorLimit = 5, int sleepInterval = 500)
    {
        const int limit = 1000;
        int errorCount = 0;

        List<LeafThread> master = [];

        bool resume = true;
        while (resume)
        {
            if (errorCount == errorLimit)
                return Result.Failure<List<LeafThread>>($"Reached error limit. Error limit: {errorLimit}");

            try
            {
                // Call the api
                Result<List<LeafThread>> result = await GetAsync<List<LeafThread>>(LeafThreadUrl(offset, limit), client);
                if (result.IsSuccess)
                {
                    List<LeafThread> value = result.Value;
                    value.ForEach(v => master.Add(v));
                    resume = value.Count == limit;
                }
                offset += limit;
                Thread.Sleep(sleepInterval);
            }
            catch { errorCount++; }
        }

        if (master.Count == 0)
            return Result.Failure<List<LeafThread>>("Something went wrong and values were not retrieved.");

        return master;
    }
    
    public bool ReposMatch(out List<IMessage> msgs, out List<LeafThread> leaf, string msgRepo = "", string leafRepo = "")
    {
        msgs = RetrieveMessageRepo(msgRepo);
        leaf = RetrieveLeafRepo(leafRepo);

        return msgs.Count == leaf.Count;
    }

    public Result Update(List<LeafThread> leafRepo, List<LeafThread> apiResult, string leafRepoLoc = "")
    {
        string leafRepoLocation = leafRepoLoc == string.Empty || !File.Exists(leafRepoLoc)
            ? LeafRepoLocation
            : leafRepoLoc;

        List<LeafThread> combined = [.. leafRepo, .. apiResult];
        var result = Update(combined, leafRepoLocation);

        return result;
    }

    public Result Update(List<LeafThread> leafRepo, string leafRepoLoc)
    {
        string leafRepoLocation = leafRepoLoc == string.Empty || !File.Exists(leafRepoLoc)
            ? LeafRepoLocation
            : leafRepoLoc;

        try
        {
            JsonRW.SerializeToFile(leafRepoLocation, leafRepo); return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(ex.Message);
        }
    }
    #endregion
}
