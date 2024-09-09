using Automate.Application.UpdateContacts;

namespace Automate.Application.InfrastructureInterfaces;

public interface IUpdateContactsService
{
    Task<bool> ExecuteContactUpdateAsync(List<List<Contacts>> contacts);
    List<List<Contacts>> GenerateContactLists();
}
