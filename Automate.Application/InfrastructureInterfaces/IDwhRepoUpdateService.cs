using Automate.Application.InfrastructureValueObjects;
using CSharpFunctionalExtensions;

namespace Automate.Application.InfrastructureInterfaces;

public interface IDwhRepoUpdateService
{
    Result WriteEntitiesList<T>(FileInfo file, List<T> list);
    Result<List<T>> GetEntitiesList<T>(SqlFileType type) where T : class;
    string GetConnection(DwhConnectionType type);
    IQuery GetQuery(DwhQueryType type);
    Result<IQuery> GetQuery(FileInfo file);
    Result<List<TEntity>> GetEntitiesList<TEntity>(string connectionString, IQuery query) where TEntity : class, IPhoneNumberCompatible;
    Result<List<TEntity>> GetEntitiesParition<TEntity>(DwhQueryType type, List<TEntity> existing, string connectionString, IQuery query) where TEntity : class, IPhoneNumberCompatible;
    Result<List<TEntity>> GetRepo<TEntity>(FileInfo location);
    List<TTarget> Convert<TTarget, TEntity>(List<TEntity> list) where TEntity : IConversionCompatible;
    Result Update<TEntity>(List<TEntity> list1, List<TEntity> list2, FileInfo repoLocation);
    Result Update<TEntity>(List<TEntity> list, FileInfo repoLocation);
}