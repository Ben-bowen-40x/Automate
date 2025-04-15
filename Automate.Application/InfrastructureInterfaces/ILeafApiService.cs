using CSharpFunctionalExtensions;
using Automate.Domain.ValueObjects;
using Automate.Application.InfrastructureValueObjects;

namespace Automate.Application.InfrastructureInterfaces;

public interface ILeafApiService
{
    Task<Result<List<Msg>>[]> GetMessages<TEntity>(HttpClient client, List<TEntity> threads) where TEntity : ILeafThread;
    Result<List<TEntity>> ReassignMessages<TEntity>(List<TEntity> threads, Task<Result<List<Msg>>[]> completedTask) where TEntity: ILeafThread;
    HttpClient GetClient(IHttpClientFactory factory);
    Task<Result<List<TEntity>>> GetAsync<TEntity>(HttpClient client, int offset = 0, int errorLimit = 5, int sleepInterval = 500, int limit = 1000) where TEntity: class, IConvert;
    Result Update<TEntity>(List<TEntity> repo, List<TEntity> apiResult, string repoLoc = "") where TEntity : class, IConvert;
    Result Update<TEntity>(List<TEntity> repo, string repoLoc) where TEntity : class, IConvert;
    Result<List<TEntity>> GetLocalRepo<TEntity>(string rawRepo) where TEntity : class, IConvert, ILeafThread;
}
