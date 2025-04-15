using Automate.Application.InfrastructureInterfaces;
using CSharpFunctionalExtensions;

namespace Automate.Application.DwhRepoUpdate;

public class DwhRepoUpdateManager(IDwhRepoUpdateService service) : IDwhRepoUpdateManager
{
    private readonly IDwhRepoUpdateService _service = service;
    public Result Manage<T>(SqlFileType sqlFileType, FileInfo saveLocation) where T : class
    {
        Result<List<T>> resultList = _service.GetEntitiesList<T>(sqlFileType);
        if (resultList.IsFailure)
            return resultList;

        List<T> result = resultList.Value;
        Result saved = _service.WriteEntitiesList(saveLocation, result);
        return saved;
    }
}
