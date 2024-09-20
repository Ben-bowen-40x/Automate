using CSharpFunctionalExtensions;

namespace Automate.Application.RepoUpdate;

public interface IRepoUpdateManager
{
    Result Manage(string valueRepo, string leafRepo, bool hardUpdate, bool forceUpdate);
}
