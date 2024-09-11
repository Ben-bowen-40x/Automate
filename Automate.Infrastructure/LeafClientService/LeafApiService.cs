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
    internal static Uri LeafThreadUrl(ILeafApiSettings settings, int offset, int limit) => new($"{settings.LeafBase}{settings.LeafThreadsEndpoint}?offset={offset}&limit={limit}");
    internal Uri LeafThreadUrl(int offset, int limit) => new($"{settings.LeafBase}{settings.LeafThreadsEndpoint}?offset={offset}&limit={limit}");
    public HttpClient GetClient(IHttpClientFactory factory)
    {
        var client = factory.CreateClient(settings.LeafName!);
        return client;
    }
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

        // Retrieve contents
        var content = CsvRW.ParseFromCsv<MessageClass>(repo);
        var conversion = content.Select(c => c.ConvertToMessage()).ToList();
        return conversion;

    }

    internal List<LeafThread> RetrieveLeafRepo(string leafRepo = "")
    {
        // Check location string
        string repo = leafRepo == string.Empty || !File.Exists(leafRepo)
            ? LeafRepoLocation
            : leafRepo;

        // Retrieve contents
        var content = JsonRW.DeserializeFile<LeafThread>(repo);
        return content;
    }
    #endregion

    #region Implementation
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

    private string? _msgRepoLoc;
    private string? _leafRepoLoc;
    private string MessageRepoLocation => _msgRepoLoc ??= FolderFinder.GetLocalFile(nameof(Infrastructure), ".info/ApiRepos/", "LeafMessages.csv");
    private string LeafRepoLocation => _leafRepoLoc ??= FolderFinder.GetLocalFile(nameof(Infrastructure), ".info/ApiRepos/", "LeafThreads.csv");
    public Result RepoUpdate(bool hardUpdate, List<LeafThread> list, List<IMessage> repoContents, string msgRepoLocation = "", string leafThreadRepoLocation = "")
    {
        // Perform local file location checks
        string msgRepo = msgRepoLocation == string.Empty || !File.Exists(msgRepoLocation)
            ? MessageRepoLocation
            : msgRepoLocation;
        string leafRepo = leafThreadRepoLocation == string.Empty || !File.Exists(leafThreadRepoLocation)
            ? LeafRepoLocation
            : leafThreadRepoLocation;

        // Convert list to value objects
        List<IMessage> contents = list.Select(l => l.ConvertToMessage()).ToList();

        return hardUpdate && list.Count > repoContents.Count
            ? HardUpdate(list, repoContents, msgRepo, leafRepo, contents)
            : SoftUpdate(repoContents, msgRepo, contents);

        // Locals
        static Result HardUpdate(List<LeafThread> list, List<IMessage> repoContents, string msgRepo, string leafRepo, List<IMessage> contents)
        {
            try
            {
                // Combine and conquer the IMessage repo
                List<IMessage> combined = [.. repoContents, .. contents];
                CsvRW.WriteToCsv<IMessage, MessageMapRW>(msgRepo, contents);

                // Write the LeafThread repo
                JsonRW.SerializeToFile(leafRepo, list);

                return Result.Success();
            }
            catch (Exception ex) { return Result.Failure(ex.Message); }
        }

        static Result SoftUpdate(List<IMessage> repoContents, string msgRepo, List<IMessage> contents)
        {
            try
            {
                // Attept to append contents to the local repo
                CsvRW.AppendToCsv<IMessage, MessageMapRW>(msgRepo, contents);
                return Result.Success();
            }
            catch
            {
                try
                {
                    // Combine and conquer
                    List<IMessage> combined = [.. repoContents, .. contents];
                    CsvRW.WriteToCsv<IMessage, MessageMapRW>(msgRepo, contents);
                    return Result.Success();
                }
                catch (Exception ex) { return Result.Failure(ex.Message); }
            }
        }
    }

    public bool ReposMatch(out List<IMessage> msgs, out List<LeafThread> leaf, string msgRepo = "", string leafRepo = "")
    {
        msgs = RetrieveMessageRepo(msgRepo);
        leaf = RetrieveLeafRepo(leafRepo);

        return msgs.Count == leaf.Count;
    }
    #endregion
}
