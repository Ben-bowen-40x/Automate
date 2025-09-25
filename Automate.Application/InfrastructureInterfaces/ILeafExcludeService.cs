using Automate.Application.InfrastructureValueObjects;
using Automate.Domain.ValueObjects;
using CSharpFunctionalExtensions;

namespace Automate.Application.InfrastructureInterfaces;

public interface ILeafExcludeService
{
    List<ILeafThread> GetConsentList(FileInfo fileInfo);
    List<PhoneNumber> GetConsentNumbers(List<ILeafThread> optOutThreads);
    Result Save(List<PhoneNumber> numbers, FileInfo location);
}
