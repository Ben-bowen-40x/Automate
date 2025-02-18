using Automate.Application.InfrastructureInterfaces;
using Automate.Domain.ValueObjects;
using CSharpFunctionalExtensions;

namespace Automate.Application.RepoUpdate;

public class LeafApiRepoUpdateManager(ILeafApiService service, IHttpClientFactory factory, IReportService report) : IRepoUpdateManager
{
    readonly ILeafApiService _service = service;
    readonly IReportService _reportService = report;

    public Result Manage<TEntity>(string valueRepoCsv, string rawRepo, bool hardUpdate, bool forceUpdate) where TEntity : class, IConvert
    {
        HttpClient client = _service.GetClient(factory);
        Result<bool> _ = _service.ReposMatch(out List<IMessage> msgs, out List<TEntity> leaf, valueRepoCsv, rawRepo);
        const string failure = "Call to the API failed";

        // Force Update
        if (forceUpdate)
        {
            // Call
            Task<Result<List<TEntity>>> threads = _service.GetLeafThreadsAsync<TEntity>(client);

            // Check for errors
            if (!threads.IsFaulted)
            {
                Result<List<TEntity>> threadVals = threads.Result;
                if (threadVals.IsSuccess)
                {
                    var value = threadVals.Value;
                    _service.Update(value, rawRepo);

                    List<IMessage> m = value.Select(v => v.Convert<TEntity, IMessage>()).ToList();
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
            Task<Result<List<TEntity>>> threads = _service.GetLeafThreadsAsync<TEntity>(client, leaf.Count - 1);

            // Check for errors
            if (!threads.IsFaulted)
            {
                Result<List<TEntity>> threadVals = threads.Result;
                if (threadVals.IsSuccess)
                {
                    List<TEntity> value = threadVals.Value;
                    _service.Update(leaf, value, rawRepo);

                    List<IMessage> mVal = value.Select(v => v.Convert<TEntity, IMessage>()).ToList();
                    List<IMessage> mLeaf = leaf.Select(v => v.Convert<TEntity, IMessage>()).ToList();
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
            List<IMessage> m = leaf.Select(l => l.Convert<TEntity, IMessage>()).ToList();
            var result = _reportService.GenerateLeafMessages(m, valueRepoCsv);
            return result;
        }
    }
}
