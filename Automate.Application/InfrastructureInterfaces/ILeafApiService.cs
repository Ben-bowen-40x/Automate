using CSharpFunctionalExtensions;
using Automate.Domain.ValueObjects;

namespace Automate.Application.InfrastructureInterfaces;

public interface ILeafApiService
{
    HttpClient GetClient(IHttpClientFactory factory);
    Task<Result<List<TEntity>>> GetLeafThreadsAsync<TEntity>(HttpClient client, int offset = 0, int errorLimit = 5, int sleepInterval = 500, int limit = 1000) where TEntity: class, IConvert;
    Result ReposMatch<TEntity>(out List<IMessage> msgs, out List<TEntity> leaf, string msgRepo = "", string leafRepo = "") where TEntity: class, IConvert;
    Result Update<TEntity>(List<TEntity> repo, List<TEntity> apiResult, string repoLoc = "") where TEntity : class, IConvert;
    Result Update<TEntity>(List<TEntity> repo, string repoLoc) where TEntity : class, IConvert;
}
