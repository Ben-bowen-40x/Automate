using Automate.Application.InfrastructureInterfaces;
using Automate.Domain.ValueObjects;
using CSharpFunctionalExtensions;
using System.Threading;

namespace Automate.Application.ApiRepoUpdate;

public class LeafApiRepoUpdateManager(ILeafApiService service, IHttpClientFactory factory, IReportService report) : ILeafApiRepoUpdateManager
{
    ILeafApiService _service = service;
    IReportService _reportService = report;

    public Result Manage(string valueRepo, string leafRepo, bool hardUpdate, bool forceUpdate)
    {
        HttpClient client = _service.GetClient(factory);
        bool match = _service.ReposMatch(out List<IMessage> msgs, out List<LeafThread> leaf, valueRepo, leafRepo);
        const string failure = "Call to the API failed";

        // Force Update
        if (forceUpdate)
        {
            // Call
            Task<Result<List<LeafThread>>> threads = _service.GetLeafThreadsAsync(client);

            // Check for errors
            if (!threads.IsFaulted)
            {
                var threadVals = threads.Result;
                if (threadVals.IsSuccess)
                {
                    var value = threadVals.Value;
                    _service.Update(value, leafRepo);

                    List<IMessage> m = value.Select(v => v.ConvertToMessage()).ToList();
                    _reportService.GenerateLeafMessages(m, out FileInfo _, valueRepo);

                    return Result.Success();
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
                var threadVals = threads.Result;
                if (threadVals.IsSuccess)
                {
                    List<LeafThread> value = threadVals.Value;
                    _service.Update(leaf, value, leafRepo);

                    List<IMessage> m = value.Select(v => v.ConvertToMessage()).ToList();
                    _reportService.GenerateLeafMessages(m, out FileInfo _, valueRepo);

                    return Result.Success();
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
            _reportService.GenerateLeafMessages(m, out FileInfo _, valueRepo);
            return Result.Success();
        }
    }
}
