using Automate.Application.InfrastructureInterfaces;
using Automate.Application.InfrastructureValueObjects;
using Automate.Domain.SolutionFunctionality;
using Automate.Infrastructure.CsvManipulationService;
using Automate.Infrastructure.DatabaseService;
using Automate.Infrastructure.JsonManipulationService;
using CSharpFunctionalExtensions;

namespace Automate.Infrastructure.DwhRepoUpdateService;

public class DwhRepoService(IDwhSettings settings) : IDwhRepoUpdateService
{
    readonly IDwhSettings _settings = settings;
    readonly RawQuery _rawQuery = new(settings);

    #region Getters
    public string GetConnection(DwhConnectionType type) => _settings.GetConnectionString(type)!;

    public IQuery GetQuery(DwhQueryType type) => type switch
    {
        DwhQueryType.AllCustomers => _rawQuery.CustomerBasic,
        DwhQueryType.ContactForms => _rawQuery.WebFormQuery,
        DwhQueryType.Discrepancy or DwhQueryType.AllCalls => _rawQuery.DatedCallsQuery(),
        _ => _rawQuery.CustomerBasic
    };

    public Result<IQuery> GetQuery(FileInfo file)
    {
        Result<string> queryStr = GetQueryString(file);
        string query = queryStr.IsSuccess
            ? queryStr.Value
            : string.Empty;
        try
        {
            Query q = new(query);
            return q;
        }
        catch (Exception ex)
        { return Result.Failure<IQuery>(ex.Message); }
    }
    static Result<string> GetQueryString(FileInfo file)
    {
        if (!file.Extension.Equals(".sql", StringComparison.CurrentCultureIgnoreCase))
            return Result.Failure<string>($"The input file must be a sql file. Input: {file.FullName}");
        try
        {
            string sqlRaw = File.ReadAllText(file.FullName);
            return sqlRaw;
        }
        catch (Exception ex)
        {
            return Result.Failure<string>(ex.Message);
        }
    }

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

    public Result<List<T>> GetEntitiesList<T>(SqlFileType type) where T : class
    {
        const string folder = ".info/Queries";
        (FileInfo file, string cxnStr) = type switch
        {
            SqlFileType.GoonDoggle => (FolderFinder.GetLocalFile(nameof(Infrastructure), folder, "GoonDoggle.sql"), _settings.GetConnectionString(DwhConnectionType.Customers)!),
            SqlFileType.MacBang => (FolderFinder.GetLocalFile(nameof(Infrastructure), folder, "MacBang.sql"), _settings.GetConnectionString(DwhConnectionType.Customers)!),
            SqlFileType.PanFries => (FolderFinder.GetLocalFile(nameof(Infrastructure), folder, "PanFries.sql"), _settings.GetConnectionString(DwhConnectionType.Customers)!),
            SqlFileType.CornFormation => (FolderFinder.GetLocalFile(nameof(Infrastructure), folder, "CornFormation.sql"), _settings.GetConnectionString(DwhConnectionType.Customers)!),
            _ => throw new NotImplementedException($"This {nameof(SqlFileType)} has not been implemented: {type}")
        };

        // Create Context
        DwhContext<T> context = new(cxnStr);
        Task<IEnumerable<T>> items = DwhContextHelpers.GetItemsFromFileAsync(context, file);

        // Ensure success
        if (items.IsFaulted)
            return Result.Failure<List<T>>(items.Exception.Message);

        // Return result
        List<T> result = items.Result.ToList();
        return result;
    }

    public Result WriteEntitiesList<T>(FileInfo file, List<T> list)
    {
        if (!file.Exists)
            File.WriteAllText(file.FullName, string.Empty);
        Result result = CsvService.Write(list, file);
        return result;
    }
    public Result<List<TEntity>> GetEntitiesParition<TEntity>(DwhQueryType type, List<TEntity> existing, string connectionString, IQuery query) where TEntity : class, IPhoneNumberCompatible
    {
        // Filter the connection string
        List<long> numbers = existing.Select(e => e.Number.Number).ToList();
        IQuery newQuery = _rawQuery.NumberFilter(type, query, numbers);
        Result<List<TEntity>> entities = GetEntitiesList<TEntity>(connectionString, newQuery);
        return entities;
    }

    public Result<List<TEntity>> GetRepo<TEntity>(FileInfo location)
    {
        try
        {
            Result<List<TEntity>> result = location.Extension switch
            {
                ".json" => JsonService.ReadFile<TEntity>(location),
                ".csv" => CsvService.Parse<TEntity>(location),
                _ => Result.Failure<List<TEntity>>($"The provided file location does not have the correct file extension. Extension: {location.Extension}. Full file location: {location.FullName}")
            };
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
    public Result Update<TEntity>(List<TEntity> list1, List<TEntity> list2, FileInfo repoLocation)
        => Update([.. list1, .. list2], repoLocation);

    public Result Update<TEntity>(List<TEntity> list, FileInfo repoLocation)
    {
        try
        {
            if (!repoLocation.Exists)
                File.WriteAllText(repoLocation.FullName, string.Empty);

            Result result = repoLocation.Extension switch
            {
                ".json" => JsonService.WriteToFile(repoLocation, list),
                ".csv" => CsvService.Write(list, repoLocation),
                _ => Result.Failure($"Failed to parse file because it's not a supported file type\n{repoLocation.FullName}")
            };

            return result;
        }
        catch (Exception ex)
        {
            return Result.Failure(ex.Message);
        }
    }
    #endregion
}
