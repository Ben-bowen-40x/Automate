using Automate.Application.UpdateContacts;
using CsvHelper.Configuration;

namespace Automate.Infrastructure.ReportingService;

public class ContactsMap : ClassMap<Contacts>
{
    public ContactsMap()
    {
        Map(m => m.Number.Number).Index(0).Name("Phone1");
        Map(m => m.Phone2.Number).Index(1).Name("Phone2");
    }
}