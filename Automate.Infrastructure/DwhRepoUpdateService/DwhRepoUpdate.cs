using Automate.Application.InfrastructureInterfaces;
using Automate.Infrastructure.DatabaseService;
using Automate.Infrastructure.JsonService;
using Automate.Infrastructure.QueryService;
using CSharpFunctionalExtensions;

namespace Automate.Infrastructure.DwhRepoUpdateService;

public class DwhRepoUpdate
{
    #region Getters
    public string GetQuery(DwhQueryType type)
        => type switch
        {
            DwhQueryType.Calls => RawQuery.CallBasic,
            DwhQueryType.Customers => RawQuery.CustomerBasic,
            _ => RawQuery.CustomerBasic
        };

    public DwhContext<TEntity> GetContext<TEntity>(string connectionString) where TEntity : class
        => new(connectionString);

    public Result<List<TEntity>> GetEntities<TEntity>(DwhContext<TEntity> context, string query) where TEntity : class
    {
        var values = DwhContextHelpers.GetItemsFromRawAsync(context, query);
        if (!values.IsFaulted)
            return values.Result.ToList();
        return Result.Failure<List<TEntity>>("Failed to get values from Dwh.");
    }

    public Result<List<TEntity>> GetRepo<TEntity>(string location)
    {
        try
        {
            return JsonRW.DeserializeFile<TEntity>(location);
        }
        catch (Exception ex)
        {
            return Result.Failure<List<TEntity>>(ex.Message);
        }
    }
    #endregion

    #region Auxiliary
    public List<TTarget> Convert<TTarget, TEntity>(List<TEntity> list) where TEntity : IConversionCompatible
        => list.Select(l => l.Convert<TTarget, TEntity>()).ToList();
    #endregion

    #region Update
    public Result Update<TEntity>(List<TEntity> list1, List<TEntity> list2, string repoLocation)
        => Update([.. list1, .. list2], repoLocation);

    public Result Update<TEntity>(List<TEntity> list, string repoLocation)
    {
        try
        {
            JsonRW.SerializeToFile(repoLocation, list);
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(ex.Message);
        }
    }
    #endregion
}