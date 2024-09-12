using CSharpFunctionalExtensions;

namespace Automate.Application.ApiRepoUpdate;

public interface ILeafApiRepoUpdateManager
{
    Result Manage(string valueRepo, string leafRepo, bool hardUpdate, bool forceUpdate);
}
