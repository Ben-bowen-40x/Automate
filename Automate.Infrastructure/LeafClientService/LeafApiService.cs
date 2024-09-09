using Automate.Domain.SolutionFunctionality;
using Automate.Infrastructure.JsonService;
using CSharpFunctionalExtensions;
using System.Net.Http.Json;

namespace Automate.Infrastructure.LeafClientService;

public class LeafApiService
{
    #region Getter
    public static async Task<Result<List<T>>> GetAsync<T>(HttpClient client, ILeafApiSettings settings, int offset, int limit = 1000)// 1000=largest page size
    {
        // Attempt to make the call
        try
        {
            string url = $"{settings.LeafBase}{settings.LeafThreadsEndpoint}?offset={offset}&limit={limit}";
            Task<HttpResponseMessage> responseTask = GetResponseAsync(client, url);
            if (!responseTask.IsFaulted)
            {
                HttpResponseMessage response = responseTask.Result;
                if (response.IsSuccessStatusCode)
                {
                    T[]? thread = await response.Content.ReadFromJsonAsync<T[]>();
                    if (thread is not null)
                    {
                        return thread!.ToList();
                    }
                    return Result.Failure<List<T>>("Parsing failure. The process of reading the results from Json failed. The results somehow became null.");
                }
                return Result.Failure<List<T>>(response.ReasonPhrase);
            }
            return Result.Failure<List<T>>(responseTask.Exception!.Message);
        }
        catch (Exception ex)
        {
            return Result.Failure<List<T>>(ex.Message);
        }
    }
    private static async Task<HttpResponseMessage> GetResponseAsync(HttpClient client, string fullUri)
    {
        HttpResponseMessage response = await client.GetAsync(fullUri);
        return response;
    }
    #endregion

    #region Updater
    private static string DefaultRepoLoc => FolderFinder.GetLocalFile(nameof(Infrastructure), ".info/ApiRepos/", $"{nameof(LeafApiService)}Repo.json");
    public static bool UpdateRepo<TValueObject>(List<TValueObject> getResult, out List<TValueObject> partition, string repoLocation = "")
    {
        // Set the repo location
        string location = repoLocation == string.Empty
            ? DefaultRepoLoc
            : repoLocation;

        // Check the repo for its existence
        if (!File.Exists(location))
            File.WriteAllText(location, "");

        // Read repo contents
        List<TValueObject> repoContents = JsonRW.DeserializeFile<TValueObject>(location);

        if (Parition(getResult, [.. repoContents], out partition))
            return true;

        return false;

    }

    private static bool Parition<TValueObject>(List<TValueObject> getResult, List<TValueObject> repoIntermediary, out List<TValueObject> partition)
    {
        // Initiate result
        bool result = false;

        // Attempt to find differences between the results and the repo contents
        // It doesn't actually matter if the repo contents contains things that the get result does not. It only matters if the get result contains contents
        List<TValueObject> partitionIntermediary = new(getResult.Count);
        foreach (var r in getResult)
        {
            if (!repoIntermediary.Remove(r))
            {
                partitionIntermediary.Add(r);

                // Reset the result value
                if (result)
                    continue;
                else
                    result = true;
            }
        }
        partition = partitionIntermediary;
        return result;
    }
    #endregion
}
