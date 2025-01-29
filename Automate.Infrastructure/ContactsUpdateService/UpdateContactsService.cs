using Automate.Application.InfrastructureInterfaces;
using Automate.Domain.ValueObjects;
using Automate.Infrastructure.DatabaseService;
using Automate.Infrastructure.DataRetrievalFormats;
using Automate.Translation.ContactTranslate;
using Automate.Translation.ValueObjectsTranslations;
using CSharpFunctionalExtensions;

namespace Automate.Infrastructure.ContactsUpdateService;

internal class UpdateContactsService(IDwhSettings settings) : IUpdateContactsService
{
    #region Implementation
    public Result ExecuteContactUpdateAsync(List<List<Contacts>> contacts)
    {
        // TODO: Automatically update the contacts list using api call
        return Result.Failure("This method is not yet implemented");
    }
    internal const int MagicNum = 2;
    public List<List<Contacts>> GenerateContactLists()
    {
        const uint magicNumber = 10;
        List<string> queries = new((int)magicNumber);
        List<List<Contacts>> listOfContacts = new((int)magicNumber);

        // Generate a database query for each set of numbers and save the result
        for (uint i = MagicNum; i < magicNumber; i++)
        {
            // Generate the raw query
            string query = new RawQuery(settings).ContactQuery(i);

            // Query the database
            DwhContext<ContactsDbEntity> contactsContext = new(settings.CallsConnectionString!);
            Task<IEnumerable<ContactsDbEntity>> task = DwhContextHelpers.GetItemsFromRawAsync(contactsContext, query);
            List<Contacts> result = task.Result
                .Select(c => c as IContactsEntity)
                .Select(c => c.Convert()).ToList();
            listOfContacts.Add(result);
        }
        return listOfContacts;
    }
    #endregion
}

