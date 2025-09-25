using Automate.Application.InfrastructureValueObjects;
using Automate.Infrastructure.CsvManipulationService;
using Automate.Infrastructure.JsonManipulationService;
using CSharpFunctionalExtensions;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Automate.Infrastructure.FatSap;

public class FatSapService(IFatSapSettings settings, IHttpClientFactory factory)
{

    #region Private
    private const int _maxPerSecond = 1000 / 8;
    private readonly IFatSapSettings _settings = settings;
    private HttpClient Cliente()
    {
        var client = factory.CreateClient(_settings.FatSapClientName!);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _settings.FatToken!);
        return client;
    }
    private HttpClient? _client;
    private HttpClient Client { get => _client ??= Cliente(); }
    private static void Wait(int ms) => Thread.Sleep(ms);
    private static async Task WaitAsync(int ms) => await Task.Delay(ms);
    #endregion

    #region Internal
    private string CallUriStr(DateTime startDate, DateTime endDate) => $"{_settings.FatBaseEndpoint}/accounts/{_settings.FatAccountId}/calls.json?start_date={startDate.ToString(_settings.FatDateFormat)}&end_date={endDate.ToString(_settings.FatDateFormat)}";
    internal Uri CallUri(DateTime startDate, DateTime endDate) => new(CallUriStr(startDate, endDate));
    #endregion

    #region Implementation
    public async Task<Result<FatSapRoot>> GetCallAsync(DateTime startDate, DateTime endDate) => await GetCallAsync(CallUri(startDate, endDate));

    /// <summary>
    /// Utility method
    /// </summary>
    /// <param name="uri"></param>
    /// <returns></returns>
    private async Task<Result<FatSapRoot>> GetCallAsync(Uri uri)
    {
        try
        {
            // Make the call
            HttpResponseMessage response = await Client.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                var value = await response.Content.ReadFromJsonAsync<FatSapRoot>();
                if (value is not null) return value!;

                // If we've reached this line, then something went wrong with the parsing and we have to default to reading the data as string
                string str = await response.Content.ReadAsStringAsync();
                string error = string.IsNullOrWhiteSpace(str) || str.Length == 0
                    ? "Parsing failure. The process of parsing the string failed. The results somehow became null"
                    : str;
                return Result.Failure<FatSapRoot>(error);
            }
            return Result.Failure<FatSapRoot>(response.ReasonPhrase);
        }
        catch (Exception ex)
        {
            return Result.Failure<FatSapRoot>(ex.Message);
        }
    }

    public Task<Result<FatSapRoot>[]> GetCallsAsync<T>(DateTime startDate, DateTime endDate)
    {
        // Gather the tasks together
        List<Task<Result<FatSapRoot>>> results = [];

        // Make the initial call
        string? next = "null";
        string? after = "null";
        string? before;
        bool cont = true;
        Task<Result<FatSapRoot>> call = GetCallAsync(startDate, endDate);
        results.Add(call);

        // Unwrap the next endpoint
        call.Wait();
        if (call.IsCompletedSuccessfully)
        {
            next = call.Result.IsSuccess
                ? call.Result.Value.NextPage
                : null;
            after = call.Result.IsSuccess
                ? call.Result.Value.After
                : null;
        }

        // Keep looping through the results until after is null
        while (cont && next is not null && after is not null)
        {
            // Make the call
            Task<Result<FatSapRoot>> loopCall = GetCallAsync(new(next!));
            results.Add(loopCall);

            // Unwrap the next endpoint
            loopCall.Wait();
            if (loopCall.IsCompletedSuccessfully)
            {
                next = loopCall.Result.IsSuccess
                    ? loopCall.Result.Value.NextPage
                    : null;
                before = after;
                after = loopCall.Result.IsSuccess
                    ? loopCall.Result.Value.After
                    : null;
                cont = !string.Equals(before, after); // We will only continue if these are not equal to each other
            }
            Wait(_maxPerSecond);
        }

        Task<Result<FatSapRoot>[]> final = Task.WhenAll(results);
        return final;
    }

    public Result<List<T>> GetCallRepo<T>(FileInfo file)
    {
        // Open file repo
        try
        {
            Result<List<T>> json = JsonService.ReadFile<T>(file);
            return json.IsSuccess
                ? json
                : CsvService.Parse<T>(file); // We're anticipating that the file will be a json file, but this is an acceptable contingency
        }
        catch (Exception ex)
        {
            return Result.Failure<List<T>>(ex.Message);
        }
    }

    public List<Call> ExtractCalls(IList<Result<FatSapRoot>> roots)
    {
        // Gather calls into a list
        List<Call> result = new(roots.Count * 10);

        // Iterate through each root 
        foreach (Result<FatSapRoot> root in roots)
        {
            if (root.IsSuccess)
            {
                if (root.Value.Calls is null) continue;

                Call[] calls = root.Value.Calls!;
                foreach (var call in calls)
                    result.Add(call);
            }
        }
        return result;
    }

    public Result SaveToRepo<T>(FileInfo file, List<Call> items)
    {
        return JsonService.WriteToFile(file, items);
    }

    public Result<DateTimeOffset> MostRecentCallDate(List<Call> callRepo)
    {
        // Set most recent date
        DateTimeOffset? recent = null;
        foreach (var call in callRepo)
        {
            if (DateTimeOffset.TryParse(call.CalledAt, out DateTimeOffset date))
            {
                if (recent == null || date.UtcDateTime > recent.Value.UtcDateTime)
                    recent = date;
            }
        }
        return recent is null
            ? Result.Failure<DateTimeOffset>("The repo does not have any datetime values")
            : recent.Value;
    }
    #endregion
}

public class FatSapRepoService(IFatSapSettings settings)
{
    #region Private
    private readonly IFatSapSettings _settings = settings;
    #endregion

    // Retrieve repo

}