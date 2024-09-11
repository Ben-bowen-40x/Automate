using CSharpFunctionalExtensions;
using Automate.Domain.ValueObjects;

namespace Automate.Application.InfrastructureInterfaces;

public interface ILeafApiService
{
    HttpClient GetClient(IHttpClientFactory factory);
    Task<Result<List<LeafThread>>> GetLeafThreadsAsync(HttpClient client, int offset = 0, int errorLimit = 5, int sleepInterval = 500);
    bool ReposMatch(out List<IMessage> msgs, out List<LeafThread> leaf, string msgRepo = "", string leafRepo = "");
    Result RepoUpdate(bool hardUpdate, List<LeafThread> list, List<IMessage> repoContents, string msgRepoLocation = "", string leafThreadRepoLocation = "");
}
