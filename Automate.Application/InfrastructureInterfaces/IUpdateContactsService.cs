using Automate.Application.UpdateContacts;
using Automate.Domain.ValueObjects;
using CSharpFunctionalExtensions;

namespace Automate.Application.InfrastructureInterfaces;

public interface IUpdateContactsService
{
    Result ExecuteContactUpdateAsync(List<List<Contacts>> contacts);
    List<List<Contacts>> GenerateContactLists();
}
