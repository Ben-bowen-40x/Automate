using Automate.Application.InfrastructureInterfaces;
using CSharpFunctionalExtensions;

namespace Automate.Application.DwhRepoUpdate;

public interface IDwhRepoUpdateManager
{
    Result Manage(SqlFileType sqlFileType, FileInfo saveLocation);
}
