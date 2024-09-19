using Automate.Application.InfrastructureInterfaces;
using Automate.Domain.ValueObjects;
using CSharpFunctionalExtensions;

namespace Automate.Application.ApiRepoUpdate;

public class LeafApiRepoUpdateManager(ILeafApiService service, IHttpClientFactory factory, IReportService report) : IRepoUpdateManager
{
    readonly ILeafApiService _service = service;
    readonly IReportService _reportService = report;

    public Result Manage(string valueRepoCsv, string rawRepoJson, bool hardUpdate, bool forceUpdate)
    {
        HttpClient client = _service.GetClient(factory);
        bool match = _service.ReposMatch(out List<IMessage> msgs, out List<LeafThread> leaf, valueRepoCsv, rawRepoJson);
        const string failure = "Call to the API failed";

        // Force Update
        if (forceUpdate)
        {
            // Call
            Task<Result<List<LeafThread>>> threads = _service.GetLeafThreadsAsync(client);

            // Check for errors
            if (!threads.IsFaulted)
            {
                Result<List<LeafThread>> threadVals = threads.Result;
                if (threadVals.IsSuccess)
                {
                    var value = threadVals.Value;
                    _service.Update(value, rawRepoJson);

                    List<IMessage> m = value.Select(v => v.ConvertToMessage()).ToList();
                    Result<FileInfo> file = _reportService.GenerateLeafMessages(m, valueRepoCsv);

                    return file;
                }
                else
                    return threads.Result;
            }
            else
                return Result.Failure(failure);
        }
        else if (hardUpdate)
        {
            // Call
            Task<Result<List<LeafThread>>> threads = _service.GetLeafThreadsAsync(client, leaf.Count - 1);

            // Check for errors
            if (!threads.IsFaulted)
            {
                Result<List<LeafThread>> threadVals = threads.Result;
                if (threadVals.IsSuccess)
                {
                    List<LeafThread> value = threadVals.Value;
                    _service.Update(leaf, value, rawRepoJson);

                    List<IMessage> mVal = value.Select(v => v.ConvertToMessage()).ToList();
                    List<IMessage> mLeaf = leaf.Select(v => v.ConvertToMessage()).ToList();
                    List<IMessage> m = [.. mLeaf, .. mVal];
                    var result = _reportService.GenerateLeafMessages(m, valueRepoCsv);

                    return result;
                }
                else
                    return threads.Result;
            }
            else
                return Result.Failure(failure);
        }
        else
        {
            List<IMessage> m = leaf.Select(l => l.ConvertToMessage()).ToList();
            var result = _reportService.GenerateLeafMessages(m, valueRepoCsv);
            return result;
        }
    }
}
