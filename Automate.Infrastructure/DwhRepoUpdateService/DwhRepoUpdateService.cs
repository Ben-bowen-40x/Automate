using Automate.Application.InfrastructureInterfaces;
using Automate.Infrastructure.DatabaseService;
using Automate.Infrastructure.JsonService;
using CSharpFunctionalExtensions;

namespace Automate.Infrastructure.DwhRepoUpdateService;

public class DwhRepoUpdateService(IDwhSettings settings) : IDwhRepoUpdateService
{
    readonly IDwhSettings _settings = settings;
    readonly RawQuery RawQuery = new(settings);

    #region Getters
    public string GetConnection(DwhConnectionType type)
        => type switch
        {
            DwhConnectionType.Calls => _settings.CallsConnectionString!,
            DwhConnectionType.Customers => _settings.CustomersConnectionString!,
            _ => _settings.CallsConnectionString!
        };

    public string GetQuery(DwhQueryType type)
        => type switch
        {
            DwhQueryType.AllCalls => RawQuery.CallBasicAddon,
            DwhQueryType.AllCustomers => RawQuery.CustomerBasic,
            _ => RawQuery.CustomerBasic
        };

    public Result<List<TEntity>> GetEntitiesList<TEntity>(string connectionString, string query) where TEntity : class, IPhoneNumberCompatible
    {
        try
        {
            DwhContext<TEntity> context = new(connectionString);
            Task<IEnumerable<TEntity>> values = DwhContextHelpers.GetItemsFromRawAsync(context, query);
            if (!values.IsFaulted)
                return values.Result.ToList();
            return Result.Failure<List<TEntity>>("Failed to get values from Dwh.");
        }
        catch (Exception ex)
        {
            return Result.Failure<List<TEntity>>(ex.Message);
        }
    }

    public Result<List<TEntity>> GetEntitiesParition<TEntity>(DwhQueryType type, List<TEntity> existing, string connectionString, string query) where TEntity : class, IPhoneNumberCompatible
    {
        // Filter the connection string
        string newQuery = RawQuery.Filter(type, query, existing.Select(e => e.Number.Number).ToList());
        return GetEntitiesList<TEntity>(connectionString, newQuery);
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
            if (!File.Exists(repoLocation))
                File.WriteAllText(repoLocation, string.Empty);

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