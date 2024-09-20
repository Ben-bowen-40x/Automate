using Automate.Application.InfrastructureInterfaces;
using Automate.Application.InfrastructureValueObjects;
using CSharpFunctionalExtensions;

namespace Automate.Application.TypedRepoUpdate;

public interface ITypedRepoUpdateManager
{
    Result Manage<TEntity, TTarget>(DwhQueryType type, DwhConnectionType connection, string repoJson, bool hardUpdate) where TEntity : class, IPhoneNumberCompatible;
}