using CSharpFunctionalExtensions;

namespace Automate.Application.ApiRepoUpdate;

public interface IRepoUpdateManager
{
    Result Manage(string valueRepo, string leafRepo, bool hardUpdate, bool forceUpdate);
}
