using Automate.Application.InfrastructureValueObjects;
using CSharpFunctionalExtensions;

namespace Automate.Application.InfrastructureInterfaces;

public interface IDwhRepoUpdateService
{
    string GetConnection(DwhConnectionType type);
    IQuery GetQuery(DwhQueryType type);
    Result<List<TEntity>> GetEntitiesList<TEntity>(string connectionString, IQuery query) where TEntity : class, IPhoneNumberCompatible;
    Result<List<TEntity>> GetEntitiesParition<TEntity>(DwhQueryType type, List<TEntity> existing, string connectionString, IQuery query) where TEntity : class, IPhoneNumberCompatible;
    Result<List<TEntity>> GetRepo<TEntity>(string location);
    List<TTarget> Convert<TTarget, TEntity>(List<TEntity> list) where TEntity : IConversionCompatible;
    Result Update<TEntity>(List<TEntity> list1, List<TEntity> list2, string repoLocation);
    Result Update<TEntity>(List<TEntity> list, string repoLocation);
}