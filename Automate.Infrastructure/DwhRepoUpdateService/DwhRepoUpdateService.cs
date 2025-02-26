using Automate.Application.InfrastructureInterfaces;
using Automate.Application.InfrastructureValueObjects;
using Automate.Infrastructure.DatabaseService;
using Automate.Infrastructure.JsonManipulationService;
using CSharpFunctionalExtensions;

namespace Automate.Infrastructure.DwhRepoUpdateService;

public class DwhRepoService(IDwhSettings settings) : IDwhRepoUpdateService
{
    readonly IDwhSettings _settings = settings;
    readonly RawQuery _rawQuery = new(settings);

    #region Getters
    public string GetConnection(DwhConnectionType type)
        => type switch
        {
            DwhConnectionType.Calls => _settings.CallsConnectionString!,
            DwhConnectionType.Customers => _settings.CustomersConnectionString!,
            DwhConnectionType.ContactForms => _settings.ContactFormsConnectionString!,
            _ => _settings.CallsConnectionString!
        };

    public IQuery GetQuery(DwhQueryType type)
        => type switch
        {
            DwhQueryType.AllCalls => _rawQuery.CallBasicAddon,
            DwhQueryType.AllCustomers => _rawQuery.CustomerBasic,
            DwhQueryType.ContactForms => _rawQuery.WebFormQuery1,
            DwhQueryType.Discrepancy => _rawQuery.DiscrepancyQuery(),
            _ => _rawQuery.CustomerBasic
        };

    public Result<List<TEntity>> GetEntitiesList<TEntity>(string connectionString, IQuery query) where TEntity : class, IPhoneNumberCompatible
    {
        try
        {
            DwhContext<TEntity> context = new(connectionString);
            Task<IEnumerable<TEntity>> values = DwhContextHelpers.GetItemsFromRawAsync(context, query.QueryString);
            if (!values.IsFaulted)
                return values.Result.ToList();
            return Result.Failure<List<TEntity>>($"Failed to get values from Dwh. Fault/Exception message: {values.Exception.Message}");
        }
        catch (Exception ex)
        {
            return Result.Failure<List<TEntity>>(ex.Message);
        }
    }

    public Result<List<TEntity>> GetEntitiesParition<TEntity>(DwhQueryType type, List<TEntity> existing, string connectionString, IQuery query) where TEntity : class, IPhoneNumberCompatible
    {
        // Filter the connection string
        List<long> numbers = existing.Select(e => e.Number.Number).ToList();
        IQuery newQuery = _rawQuery.Filter(type, query, numbers);
        Result<List<TEntity>> entities = GetEntitiesList<TEntity>(connectionString, newQuery);
        return entities;
    }

    public Result<List<TEntity>> GetRepo<TEntity>(string location)
    {
        try
        {
            FileInfo loc = new(location);
            Result<List<TEntity>> result = JsonService.ReadFile<TEntity>(loc);
            return result;
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

            FileInfo repo = new(repoLocation);
            Result result = JsonService.WriteToFile(repo, list);
            return result;
        }
        catch (Exception ex)
        {
            return Result.Failure(ex.Message);
        }
    }
    #endregion
}
