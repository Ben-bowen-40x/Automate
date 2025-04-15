using Automate.Application.InfrastructureInterfaces;
using CSharpFunctionalExtensions;

namespace Automate.Application.DwhRepoUpdate;

public interface IDwhRepoUpdateManager
{
    Result Manage<T>(SqlFileType sqlFileType, FileInfo saveLocation) where T : class;
}
