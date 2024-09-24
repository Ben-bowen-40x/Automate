using Automate.Application.InfrastructureInterfaces;
using Automate.Application.InfrastructureValueObjects;
using CSharpFunctionalExtensions;

namespace Automate.Application.TypedRepoUpdate;

internal class DwhRepoUpdateManager(IDwhRepoUpdateService service) : ITypedRepoUpdateManager
{
    readonly IDwhRepoUpdateService _service = service;
    public Result Manage<TEntity>(DwhQueryType type, DwhConnectionType connection, string repoJson, bool hardUpdate) where TEntity : class, IPhoneNumberCompatible
    {
        // Set up 
        string query = _service.GetQuery(type);
        string connectionStr = _service.GetConnection(connection);

        // Attempt
        try
        {
            if (hardUpdate)
                return HardUpdate<TEntity>(repoJson, query, connectionStr);
            else
                return SoftUpdate<TEntity>(type, repoJson, query, connectionStr);
        }
        catch (Exception ex)
        {
            return Result.Failure(ex.Message);
        }
    }

    private Result SoftUpdate<TEntity>(DwhQueryType type, string repoJson, string query, string connectionStr) where TEntity : class, IPhoneNumberCompatible
    {
        Result<List<TEntity>> repo = _service.GetRepo<TEntity>(repoJson);
        if (repo.IsSuccess)
        {
            List<TEntity> repoList = repo.Value;
            Result<List<TEntity>> call = _service.GetEntitiesParition(type, repoList, connectionStr, query);

            if (call.IsSuccess)
            {
                List<TEntity> list = call.Value;
                Result updateResult = _service.Update(repoList, list, repoJson);
                return updateResult;
            }
            else
                return call;
        }
        else return repo;
    }

    private Result HardUpdate<TEntity>(string repoJson, string query, string connectionStr) where TEntity : class, IPhoneNumberCompatible
    {
        Result<List<TEntity>> call = _service.GetEntitiesList<TEntity>(connectionStr, query);
        if (call.IsSuccess)
        {
            List<TEntity> list = call.Value;
            Result updateResult = _service.Update(list, repoJson);
            return updateResult;
        }
        return call;
    }
}
