using Automate.Application.InfrastructureInterfaces;
using Automate.Application.InfrastructureValueObjects;
using Automate.Domain.SolutionFunctionality;
using Automate.Domain.ValueObjects;
using Automate.Infrastructure.CsvManipulationService;
using Automate.Infrastructure.DataRetrievalFormats;
using Automate.Infrastructure.JsonManipulationService;
using CSharpFunctionalExtensions;
using System.Net.Http;
using System.Net.Http.Json;

namespace Automate.Infrastructure.LeafClientService;

public class LeafApiService(ILeafApiSettings settings) : ILeafApiService
{
    #region Setup
    public HttpClient GetClient(IHttpClientFactory factory)
    {
        HttpClient client = factory.CreateClient(settings.LeafName!);
        return client;
    }
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

    internal static Result<List<IMessage>> RetrieveMessageRepo(string msgRepoLoc = "")
    {
        // Check loc string
        string repo = msgRepoLoc == string.Empty || !File.Exists(msgRepoLoc)
            ? MessageRepoLocation
            : msgRepoLoc;

        // Check if location exists
        if (!File.Exists(repo))
            File.WriteAllText(repo, "");

        // Retrieve contents
        try
        {
            Result<List<MessageClass>> result = CsvService.Parse<MessageClass>(repo);
            List<MessageClass> content = result.Value;
            List<IMessage> conversion = content
                .Select(c => c.Convert<MessageClass, IMessage>())
                .ToList();
            return conversion;
        }
        catch (Exception ex)
        {
            return Result.Failure<List<IMessage>>(ex.Message);
        }
    }

    internal static Result<List<ILeafThread>> RetrieveLeafRepo(string leafRepo = "")
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
            Result<List<LeafThread>> result = JsonService.ReadFile<LeafThread>(repo);

            // The train MUST stop here because this is very unexcpected behavior at this point
            // Plus, this point contains all of the necessary information that we need to see all the context;
            // JsonService does NOT have enough context for exceptions to be thrown there, either during debugging or during live executions
            List<ILeafThread> content = result.IsSuccess
                ? result.Value
                    .Select(c => (ILeafThread)c)
                    .ToList()
                : throw new Exception(result.Error); // Stop the train here -- this is the best place
            return content;
        }
        catch (Exception ex) { return Result.Failure<List<ILeafThread>>(ex.Message); }

    }
    #endregion

    #region Implementation
    private static string? _msgRepoLoc;
    private static string? _leafRepoLoc;
    public static string MessageRepoLocation => _msgRepoLoc ??= FolderFinder.GetLocalFile(nameof(Infrastructure), ".info/ApiRepos/", "LeafMessages.csv");
    public static string LeafRepoLocation => _leafRepoLoc ??= FolderFinder.GetLocalFile(nameof(Infrastructure), ".info/ApiRepos/", "LeafThreads.json");

    public async Task<Result<List<ILeafThread>>> GetLeafThreadsAsync(HttpClient client, int offset = 0, int errorLimit = 5, int sleepInterval = 500, int limit = 1000)
    {
        int errorCount = 0;

        List<ILeafThread> master = [];

        bool resume = true;
        while (resume)
        {
            if (errorCount == errorLimit)
                return Result.Failure<List<ILeafThread>>($"Reached error limit. Error limit: {errorLimit}");

            try
            {
                // Call the api
                Result<List<ILeafThread>> result = await GetAsync<List<ILeafThread>>(LeafThreadUrl(offset, limit), client);
                if (result.IsSuccess)
                {
                    List<ILeafThread> value = result.Value;
                    value.ForEach(v => master.Add(v));
                    resume = value.Count == limit;
                }
                else
                    return result;
                offset += limit;
                Thread.Sleep(sleepInterval);
            }
            catch { errorCount++; }
        }

        if (master.Count == 0)
            return Result.Failure<List<ILeafThread>>("Something went wrong and values were not retrieved.");

        return master;
    }

    public Result<bool> ReposMatch(out List<IMessage> msgs, out List<ILeafThread> leaf, string msgRepo = "", string leafRepo = "")
    {
        var imsgs = RetrieveMessageRepo(msgRepo);
        Result<List<ILeafThread>> ileaf = RetrieveLeafRepo(leafRepo);
        msgs = [];
        leaf = [];

        if (imsgs.IsSuccess && ileaf.IsSuccess)
        {
            msgs = imsgs.Value;
            leaf = ileaf.Value;
            return msgs.Count == leaf.Count;
        }
        else if (imsgs.IsFailure)
            return Result.Failure<bool>(imsgs.Error);
        else if (ileaf.IsFailure)
            return Result.Failure<bool>(ileaf.Error);
        else
            return Result.Failure<bool>(imsgs + " " + ileaf.Error);
    }

    public Result Update(List<ILeafThread> leafRepo, List<ILeafThread> apiResult, string leafRepoLoc = "")
    {
        string leafRepoLocation = leafRepoLoc == string.Empty || !File.Exists(leafRepoLoc)
            ? LeafRepoLocation
            : leafRepoLoc;

        List<ILeafThread> combined = [.. leafRepo, .. apiResult];
        var result = Update(combined, leafRepoLocation);

        return result;
    }

    public Result Update(List<ILeafThread> leafRepo, string leafRepoLoc)
    {
        string leafRepoLocation = leafRepoLoc == string.Empty || !File.Exists(leafRepoLoc)
            ? LeafRepoLocation
            : leafRepoLoc;

        try
        {
            JsonService.WriteToFile(leafRepoLocation, leafRepo); return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(ex.Message);
        }
    }
    #endregion
}
