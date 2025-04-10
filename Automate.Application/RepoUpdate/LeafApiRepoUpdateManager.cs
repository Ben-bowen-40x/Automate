using Automate.Application.InfrastructureInterfaces;
using Automate.Application.InfrastructureValueObjects;
using Automate.Domain.ValueObjects;
using CSharpFunctionalExtensions;

namespace Automate.Application.RepoUpdate;

public class LeafApiRepoUpdateManager(ILeafApiService service, IHttpClientFactory factory, IReportService report) : IRepoUpdateManager
{
    readonly ILeafApiService _service = service;
    readonly IReportService _reportService = report;

    public Result Manage<TEntity>(string valueRepoCsv, string rawRepo, bool hardUpdate, bool forceUpdate) where TEntity : class, IConvert, ILeafThread
    {
        HttpClient client = _service.GetClient(factory);
        Result _ = _service.ReposMatch(out List<IMessage> msgs, out List<TEntity> leaf, valueRepoCsv, rawRepo);
        const string failure = "Call to the API failed";

        #region Get rid of this
        if (false)
        {
            // Retrieve messages
            Task<Result<List<Msg>>[]> msgsTask = _service.GetMessages(client, leaf);

            // Reassign messages
            var leafCopy = leaf.ToList();
            Result<List<TEntity>> val = _service.ReassignMessages(leafCopy, msgsTask);

            // Unwrap messages
            List<TEntity> newValue = val.IsSuccess
                ? val.Value
                : leaf;

            // Update the repo
            Result update = _service.Update(newValue, rawRepo);

            // Convert the newValues and save
            List<IMessage> m = newValue.Select(v => v.Convert<TEntity, IMessage>()).ToList();
            Result<FileInfo> file = _reportService.GenerateLeafMessages(m, valueRepoCsv);

            return file;
        }
        #endregion

        // Force Update
        if (forceUpdate)
        {
            // Call
            Task<Result<List<TEntity>>> threads = _service.GetAsync<TEntity>(client);

            // Check for errors
            if (!threads.IsFaulted)
            {
                Result<List<TEntity>> threadVals = threads.Result;
                if (threadVals.IsSuccess)
                {
                    List<TEntity> value = threadVals.Value;

                    // Change message values
                    Task<Result<List<Msg>>[]> msgsTask = _service.GetMessages(client, value);
                    var valueCopy = value.ToList();
                    Result<List<TEntity>> val = _service.ReassignMessages(valueCopy, msgsTask);

                    // Unwrap messages
                    List<TEntity> newValue = val.IsSuccess
                        ? val.Value
                        : value;

                    // Update the repo
                    Result update = _service.Update(newValue, rawRepo);

                    // Convert the newValues and save
                    List<IMessage> m = newValue.Select(v => v.Convert<TEntity, IMessage>()).ToList();
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
            Task<Result<List<TEntity>>> threads = _service.GetAsync<TEntity>(client, offset: leaf.Count - 1);

            // Check for errors
            if (!threads.IsFaulted)
            {
                Result<List<TEntity>> threadVals = threads.Result;
                if (threadVals.IsSuccess)
                {
                    List<TEntity> value = threadVals.Value;

                    // Change message values
                    Task<Result<List<Msg>>[]> msgsTask = _service.GetMessages(client, value);
                    var valueCopy = value.ToList();
                    Result<List<TEntity>> val = _service.ReassignMessages(valueCopy, msgsTask);

                    // Unwrap messages
                    List<TEntity> newValue = val.IsSuccess
                        ? val.Value
                        : value;

                    Result updated = _service.Update(leaf, newValue, rawRepo);

                    List<IMessage> mVal = newValue.Select(v => v.Convert<TEntity, IMessage>()).ToList();
                    List<IMessage> mLeaf = leaf.Select(v => v.Convert<TEntity, IMessage>()).ToList();
                    List<IMessage> m = [.. mLeaf, .. mVal];
                    Result<FileInfo> result = _reportService.GenerateLeafMessages(m, valueRepoCsv);

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
            List<IMessage> m = leaf
                .Select(l => l.Convert<TEntity, IMessage>())
                .ToList();
            var result = _reportService.GenerateLeafMessages(m, valueRepoCsv);
            return result;
        }
    }
}