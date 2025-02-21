using Automate.Application.InfrastructureInterfaces;
using Automate.Domain.ValueObjects;
using Automate.Infrastructure.DatabaseService;
using Automate.Translation.ContactTranslate;
using CSharpFunctionalExtensions;

namespace Automate.Infrastructure.ContactsUpdateService;

internal class UpdateContactsService(IDwhSettings settings) : IUpdateContactsService
{
    private readonly IDwhSettings _settings = settings;

    #region Implementation
    public Result ExecuteContactUpdateAsync(List<List<Contact>> contacts)
    {
        // TODO: Automatically update the contacts list using api call
        return Result.Failure("This method is not yet implemented");
    }
    internal const int MagicNum = 2;
    public List<List<Contact>> GenerateContactLists()
    {
        const uint magicNumber = 10;
        List<string> queries = new((int)magicNumber);
        List<List<Contact>> listOfContacts = new((int)magicNumber);

        // Generate a database query for each set of numbers and save the result
        var q = new RawQuery(_settings);
        for (uint i = MagicNum; i < magicNumber; i++)
        {
            // Generate the raw query
            string query = q.ContactQuery(i);

            // Query the database
            DwhContext<ContactsDbEntity> contactsContext = new(_settings.CallsConnectionString!);
            Task<IEnumerable<ContactsDbEntity>> task = DwhContextHelpers.GetItemsFromRawAsync(contactsContext, query);
            List<Contact> result = task.Result
                .Select(c => c as IContactsEntity)
                .Select(c => c.Translate()).ToList();
            listOfContacts.Add(result);
        }
        return listOfContacts;
    }
    #endregion
}

