using Automate.Application.InfrastructureInterfaces;
using Automate.Application.InfrastructureValueObjects;
using CSharpFunctionalExtensions;

namespace Automate.Application.TypedRepoUpdate;

internal class DwhRepoUpdateManager(IDwhRepoUpdateService service) : ITypedRepoUpdateManager
{
    readonly IDwhRepoUpdateService _service = service;
    public Result Manage<TEntity>(DwhQueryType type, DwhConnectionType connection, FileInfo repoJson, string valueRepo, bool hardUpdate) where TEntity : class, IPhoneNumberCompatible
    {
        // Set up 
        IQuery query = _service.GetQuery(type);
        string connectionStr = _service.GetConnection(connection);

        // Attempt
        try { return Execution<TEntity>(type, repoJson, valueRepo, hardUpdate, query, connectionStr); }
        catch (Exception ex) { return Result.Failure(ex.Message); }
    }

    private Result Execution<TEntity>(DwhQueryType type, FileInfo repoJson, string valueRepo, bool hardUpdate, IQuery query, string connectionStr) where TEntity : class, IPhoneNumberCompatible
    {
        if (hardUpdate)
        {
            Result<List<TEntity>> call = _service.GetEntitiesList<TEntity>(connectionStr, query);
            if (call.IsSuccess)
            {
                List<TEntity> list = call.Value;
                Result result = ReturnResult(repoJson, valueRepo, list);
                return result;
            }
            return call;
        }
        else
        {
            Result<List<TEntity>> repo = _service.GetRepo<TEntity>(repoJson);
            if (repo.IsSuccess)
            {
                List<TEntity> repoList = repo.Value;
                Result<List<TEntity>> call = _service.GetEntitiesParition(type, repoList, connectionStr, query);

                if (call.IsSuccess)
                {
                    List<TEntity> list = call.Value;
                    Result result = ReturnResult(repoJson, valueRepo, [.. repoList, .. list]);
                    return result;
                }
                else
                    return call;
            }
            else return repo;
        }
    }

    private Result ReturnResult<TEntity>(FileInfo repoJson, string valueRepo, List<TEntity> list) where TEntity : class, IPhoneNumberCompatible
    {
        Result updateResult = _service.Update(list, repoJson);
        Result valueResult = _service.Update(list, new(string.IsNullOrWhiteSpace(valueRepo) ? "\\" : valueRepo));
        return updateResult.IsSuccess
            ? updateResult
            : valueResult.IsSuccess
                ? Result.Failure($"The value repo was successfully created, but the true json repo was not updated:\nFailed to update the following repository: {repoJson.FullName}")
                : Result.Failure($"Both repositories failed to update.\nJson repo: \"{repoJson.FullName}\"\nValue Repo: \"{valueRepo}\"");
    }
}
