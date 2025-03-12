using Automate.Application.InfrastructureInterfaces;
using Automate.Application.InfrastructureValueObjects;
using CSharpFunctionalExtensions;

namespace Automate.Application.TypedRepoUpdate;

public interface ITypedRepoUpdateManager
{
    Result Manage<TEntity>(DwhQueryType type, DwhConnectionType connection, FileInfo repoJson, string valueRepo, bool hardUpdate) where TEntity : class, IPhoneNumberCompatible;
}