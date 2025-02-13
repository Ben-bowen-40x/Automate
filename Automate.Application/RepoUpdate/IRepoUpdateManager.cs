using Automate.Domain.ValueObjects;
using CSharpFunctionalExtensions;

namespace Automate.Application.RepoUpdate;

public interface IRepoUpdateManager
{
    Result Manage<TEntity>(string valueRepoCsv, string repoJson, bool hardUpdate, bool forceUpdate) where TEntity: class, IConvert;
}
