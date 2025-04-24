using Automate.Application.InfrastructureInterfaces;
using Automate.Application.InfrastructureValueObjects;
using Automate.Domain.SolutionFunctionality;
using Automate.Domain.ValueObjects;
using Automate.Infrastructure.CsvManipulationService;
using Automate.Infrastructure.DataRetrievalFormats;
using Automate.Infrastructure.JsonManipulationService;
using CSharpFunctionalExtensions;
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
    internal Uri LeafThreadUrl(int offset = 0, int limit = 1000) => new($"{settings.LeafBase}{settings.LeafThreadsEndpoint}?limit={limit}&offset={offset}");
    internal Uri LeafMessagesUrl(string thread, int limit = 100, string type = "sms") => new($"{settings.LeafBase}{settings.LeafThreadsEndpoint}/{thread}{settings.LeafMessagesEndpoint}?limit={limit}&type={type}&offset=0");
    #endregion

    #region Internal
    internal static async Task<Result<T>> GetSingleAsync<T>(Uri url, HttpClient client)
    {
        // Attempt to make the call
        try
        {
            HttpResponseMessage response = await client.GetAsync(url);
            Wait();
            if (response.IsSuccessStatusCode)
            {
                T? value = await response.Content.ReadFromJsonAsync<T>();
                if (value is not null)
                {
                    return value!;
                }

                string str = await response.Content.ReadAsStringAsync();
                string error = string.IsNullOrWhiteSpace(str) || str.Length == 0
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
                if (string.IsNullOrWhiteSpace(result) || result.Length == 0)
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
        FileInfo repo = string.IsNullOrWhiteSpace(msgRepoLoc) || !File.Exists(msgRepoLoc)
            ? MessageRepoLocation
            : new(msgRepoLoc);

        // Check if location exists
        if (!repo.Exists)
            File.WriteAllText(repo.FullName, "");

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

    #endregion

    #region Implementation
    public Uri DefaultLeafThreadUrl(int offset = 0) => LeafThreadUrl(offset);
    private static FileInfo? _msgRepoLoc;
    private static FileInfo? _leafRepoLoc;
    public static FileInfo MessageRepoLocation => _msgRepoLoc ??= FolderFinder.GetLocalFile(nameof(Infrastructure), ".info/ApiRepos/", "LeafMessages.csv");
    public static FileInfo LeafRepoLocation => _leafRepoLoc ??= FolderFinder.GetLocalFile(nameof(Infrastructure), ".info/ApiRepos/", "LeafThreads.json");
    
    public Result<List<TEntity>> GetLocalRepo<TEntity>(string leafRepo) where TEntity : class, IConvert, ILeafThread
    {
        // Check location string
        FileInfo repo = string.IsNullOrWhiteSpace(leafRepo) || !File.Exists(leafRepo)
            ? LeafRepoLocation
            : new(leafRepo);

        // Check if location exists
        if (!repo.Exists)
            File.WriteAllText(repo.FullName, string.Empty);

        try
        {
            // Retrieve contents
            Result<List<TEntity>> result = JsonService.ReadFile<TEntity>(repo);

            // The train MUST stop here because this is very unexcpected behavior at this point
            // Plus, this point contains all of the necessary information that we need to see all the context;
            // JsonService does NOT have enough context for exceptions to be thrown there, either during debugging or during live executions
            List<TEntity> content = result.IsSuccess
                ? [.. result.Value]
                : throw new Exception(result.Error); // Stop the train here -- this is the best place
            return content;
        }
        catch (Exception ex) { return Result.Failure<List<TEntity>>(ex.Message); }

    }

    public async Task<Result<List<TEntity>>> GetAsync<TEntity>(HttpClient client, int offset = 0, int errorLimit = 5, int sleepInterval = 500, int limit = 1000) where TEntity : class, IConvert
    {
        int errorCount = 0;

        List<TEntity> master = [];

        bool resume = true;
        while (resume)
        {
            if (errorCount == errorLimit)
                return Result.Failure<List<TEntity>>($"Reached error limit. Error limit: {errorLimit} attempts");

            try
            {
                // Call the api
                Uri newurl = LeafThreadUrl(offset, limit);
                Result<List<TEntity>> result = await GetSingleAsync<List<TEntity>>(newurl, client);
                if (result.IsSuccess)
                {
                    List<TEntity> value = result.Value;
                    value.ForEach(master.Add);
                    resume = value.Count == limit;
                }
                else
                {
                    errorCount++;
                    return result;
                }
                offset += limit;
            }
            catch { errorCount++; }
        }

        if (master.Count == 0)
            return Result.Failure<List<TEntity>>("Something went wrong and values were not retrieved.");

        return master;
    }

    public Task<Result<List<Msg>>[]> GetMessages<TEntity>(HttpClient client, List<TEntity> threads) where TEntity : ILeafThread
    {
        List<Task<Result<List<Msg>>>> tasks = new(threads.Count);
        foreach (TEntity thread in threads)
        {
            // Retrieve the thread id
            if (thread.Uuid is null)
                continue;
            string threadid = thread.Uuid!;
            Uri uri = LeafMessagesUrl(threadid);

            // Retrieve the new list 
            Task<Result<List<Msg>>> messagesResultTask = GetSingleAsync<List<Msg>>(uri, client);
            tasks.Add(messagesResultTask);

            Thread.Sleep(150);
        }

        Task<Result<List<Msg>>[]> completedTask = Task.WhenAll(tasks);

        return completedTask;
    }

    private static async void Wait(int sleepInterval = 500) => await Task.Delay(sleepInterval);

    public Result<List<TEntity>> ReassignMessages<TEntity>(List<TEntity> threads, Task<Result<List<Msg>>[]> completedTask) where TEntity : ILeafThread
    {
        if (completedTask.IsCompletedSuccessfully)
        {
            Result<List<Msg>>[] messageResult = completedTask.Result;

            foreach (Result<List<Msg>> msgR in messageResult)
            {
                // Unwrap 
                if (msgR.IsSuccess)
                {
                    List<Msg> messages = msgR.Value;
                    foreach (TEntity thread in threads)
                    {
                        if (thread.Uuid is null)
                            continue;
                        else if (messages.Count > 0 && thread.Uuid.Equals(messages[0].Thread))
                        {
                            thread.Messages = [.. messages];
                            break;
                        }
                    }
                }
            }
            return threads;
        }
        string error = completedTask.Exception is not null ? completedTask.Exception.Message : "The task failed to complete and carried no exception message";
        return Result.Failure<List<TEntity>>(error);
    }

    public Result ReposMatch<TEntity>(out List<IMessage> msgs, out List<TEntity> leaf, string msgRepo = "", string leafRepo = "") where TEntity : class, IConvert, ILeafThread
    {
        Result<List<IMessage>> imsgs = RetrieveMessageRepo(msgRepo);
        Result<List<TEntity>> ileaf = GetLocalRepo<TEntity>(leafRepo);
        msgs = imsgs.IsSuccess ? imsgs.Value : [];
        leaf = ileaf.IsSuccess ? ileaf.Value : [];

        // Please allow the debugger to see the value of this variable, and don't return it in-line, please
        Result result = (imsgs.IsSuccess, ileaf.IsSuccess) switch
        {
            (true, true) => Result.Success(),
            (true, false) => Result.Failure<bool>(ileaf.Error),
            (false, true) => Result.Failure<bool>(imsgs.Error),
            _ => Result.Failure<bool>(imsgs.Error + " " + ileaf.Error)
        };
        return result;
    }

    public Result Update<TEntity>(List<TEntity> leafRepo, List<TEntity> apiResult, string leafRepoLoc = "") where TEntity : class, IConvert
    {
        FileInfo leafRepoLocation = string.IsNullOrWhiteSpace(leafRepoLoc) || !File.Exists(leafRepoLoc)
            ? LeafRepoLocation
            : new(leafRepoLoc);

        List<TEntity> combined = [.. leafRepo, .. apiResult];
        Result result = Update(combined, leafRepoLocation.FullName);

        return result;
    }

    public Result Update<TEntity>(List<TEntity> leafRepo, string leafRepoLoc) where TEntity : class, IConvert
    {
        FileInfo leafRepoLocation = string.IsNullOrWhiteSpace(leafRepoLoc) || !File.Exists(leafRepoLoc)
            ? LeafRepoLocation
            : new(leafRepoLoc);

        try
        {
            return JsonService.WriteToFile(leafRepoLocation, leafRepo);
        }
        catch (Exception ex)
        {
            return Result.Failure(ex.Message);
        }
    }
    #endregion
}
