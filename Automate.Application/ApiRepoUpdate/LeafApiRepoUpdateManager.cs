using Automate.Application.InfrastructureInterfaces;
using Automate.Domain.ValueObjects;
using CSharpFunctionalExtensions;

namespace Automate.Application.ApiRepoUpdate;

public class LeafApiRepoUpdateManager(ILeafApiService service, IHttpClientFactory factory) : ILeafApiRepoUpdateManager
{
    ILeafApiService _service = service;

    public Result Manage(FileInfo valueRepo, FileInfo leafRepo, bool hardUpdate)
    {
        HttpClient client = _service.GetClient(factory);

        if (_service.ReposMatch(out List<IMessage> msgs, out List<LeafThread> leaf, valueRepo.FullName, leafRepo.FullName))
        {
            if (hardUpdate)
            {
                return HardUpdate(valueRepo, leafRepo, hardUpdate, client, msgs);
            }
            else
            {
                return SoftUpdate(valueRepo, leafRepo, hardUpdate, client, msgs);
            }
        }
        else
            return HardUpdate(valueRepo, leafRepo, true, client, msgs);

        // Locals
        Result HardUpdate(FileInfo valueRepo, FileInfo leafRepo, bool hardUpdate, HttpClient client, List<IMessage> msgs)
        {
            Task<Result<List<LeafThread>>> threads = _service.GetLeafThreadsAsync(client);
            if (!threads.IsFaulted)
            {
                var threadVals = threads.Result;
                if (threadVals.IsSuccess)
                {
                    _service.RepoUpdate(hardUpdate, threadVals.Value, msgs, valueRepo.FullName, leafRepo.FullName);
                    return Result.Success();
                }
                else
                    return threads.Result;
            }
            else
                return Result.Failure("Call to the API failed");
        }

        Result SoftUpdate(FileInfo valueRepo, FileInfo leafRepo, bool hardUpdate, HttpClient client, List<IMessage> msgs)
        {
            Task<Result<List<LeafThread>>> threads = _service.GetLeafThreadsAsync(client, offset: msgs.Count);
            if (!threads.IsFaulted)
            {
                var threadVals = threads.Result;
                if (threadVals.IsSuccess)
                {
                    _service.RepoUpdate(hardUpdate, threadVals.Value, msgs, valueRepo.FullName, leafRepo.FullName);
                    return Result.Success();
                }
                else
                    return threads.Result;
            }
            else
                return Result.Failure("Call to the API failed");
        }
    }
}
