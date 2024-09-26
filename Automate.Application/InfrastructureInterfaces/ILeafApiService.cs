using CSharpFunctionalExtensions;
using Automate.Domain.ValueObjects;
using Automate.Application.InfrastructureValueObjects;

namespace Automate.Application.InfrastructureInterfaces;

public interface ILeafApiService
{
    HttpClient GetClient(IHttpClientFactory factory);
    Task<Result<List<LeafThread>>> GetLeafThreadsAsync(HttpClient client, int offset = 0, int errorLimit = 5, int sleepInterval = 500, int limit = 1000);
    Result<bool> ReposMatch(out List<IMessage> msgs, out List<LeafThread> leaf, string msgRepo = "", string leafRepo = "");
    Result Update(List<LeafThread> leafRepo, List<LeafThread> apiResult, string leafRepoLoc = "");
    Result Update(List<LeafThread> leafRepo, string leafRepoLoc);
}
