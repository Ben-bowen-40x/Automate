using CSharpFunctionalExtensions;

namespace Automate.Application.ApiRepoUpdate;

public interface ILeafApiRepoUpdateManager
{
    Result Manage(FileInfo valueRepo, FileInfo leafRepo, bool hardUpdate);
}
