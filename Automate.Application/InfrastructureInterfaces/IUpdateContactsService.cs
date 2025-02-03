using Automate.Domain.ValueObjects;
using CSharpFunctionalExtensions;

namespace Automate.Application.InfrastructureInterfaces;

public interface IUpdateContactsService
{
    Result ExecuteContactUpdateAsync(List<List<Contact>> contacts);
    List<List<Contact>> GenerateContactLists();
}
