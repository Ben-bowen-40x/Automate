using Automate.Application.InfrastructureInterfaces;
using CSharpFunctionalExtensions;

namespace Automate.Application.DwhRepoUpdate;

public class DwhRepoUpdateManager(IDwhRepoUpdateService service) : IDwhRepoUpdateManager
{
    private readonly IDwhRepoUpdateService _service = service;
    public Result Manage(SqlFileType sqlFileType, FileInfo saveLocation)
    {
        Result<List<dynamic>> resultList = _service.GetEntitiesList(sqlFileType);
        if (resultList.IsFailure)
            return resultList;

        List<dynamic> result = resultList.Value;
        Result saved = _service.WriteEntitiesList(saveLocation, result);
        return saved;
    }
}
