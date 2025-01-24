using CSharpFunctionalExtensions;
using Automate.Domain.ValueObjects;
using Automate.Application.InfrastructureValueObjects;

namespace Automate.Application.InfrastructureInterfaces;

public interface ILeafApiService
{
    HttpClient GetClient(IHttpClientFactory factory);
    Task<Result<List<ILeafThread>>> GetLeafThreadsAsync(HttpClient client, int offset = 0, int errorLimit = 5, int sleepInterval = 500, int limit = 1000);
    Result<bool> ReposMatch(out List<IMessage> msgs, out List<ILeafThread> leaf, string msgRepo = "", string leafRepo = "");
    Result Update(List<ILeafThread> leafRepo, List<ILeafThread> apiResult, string leafRepoLoc = "");
    Result Update(List<ILeafThread> leafRepo, string leafRepoLoc);
}
