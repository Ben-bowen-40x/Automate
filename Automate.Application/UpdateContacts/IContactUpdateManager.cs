using Automate.Domain.ValueObjects;

namespace Automate.Application.UpdateContacts;

public interface IContactUpdateManager
{
    UpdateResult UpdateContacts(string reportDirectory);
}
