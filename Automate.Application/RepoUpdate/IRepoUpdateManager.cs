using CSharpFunctionalExtensions;

namespace Automate.Application.RepoUpdate;

public interface IRepoUpdateManager
{
    Result Manage(string valueRepoCsv, string repoJson, bool hardUpdate, bool forceUpdate);
}
