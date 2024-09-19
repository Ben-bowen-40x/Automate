using Automate.Application.InfrastructureInterfaces;
using Automate.Infrastructure.DatabaseService;
using Automate.Infrastructure.JsonService;
using CSharpFunctionalExtensions;

namespace Automate.Infrastructure.DwhRepoUpdateService;

public class DwhRepoUpdate
{
    public DwhContext<TEntity> GetContext<TEntity>(string connectionString) where TEntity : class
    {
        return new(connectionString);
    }

    public Result<List<TEntity>> GetItems<TEntity>(DwhContext<TEntity> context, string query) where TEntity : class
    {
        var values = DwhContextHelpers.GetItemsFromRawAsync(context, query);
        if (!values.IsFaulted)
            return values.Result.ToList();
        return Result.Failure<List<TEntity>>("Failed to get values from Dwh.");
    }

    internal Result<List<TEntity>> GetItems<TEntity>(DwhContext<TEntity> context, List<TEntity> existingRepo, string query) where TEntity : class
    {
        var values = DwhContextHelpers.GetItemsFromRawAsync(context, query);
        if (!values.IsFaulted)
            return values.Result.ToList();
        return Result.Failure<List<TEntity>>("Failed to get values from Dwh.");
    }

    public List<TTarget> Convert<TTarget, TEntity>(List<TEntity> list) where TEntity : IConversionCompatible
    {
        return list.Select(l => l.Convert<TTarget, TEntity>()).ToList();
    }

    public Result Update<TEntity>(List<TEntity> list1, List<TEntity> list2, string repoLocation)
    {
        List<TEntity> list = [.. list1, .. list2];
        return Update(list, repoLocation);
    }

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
}