using Automate.Application.InfrastructureInterfaces;
using Automate.Domain.ValueObjects;
using CSharpFunctionalExtensions;

namespace Automate.Application.UpdateContacts;

public class ContactUpdateManager(IUpdateContactsService updateService, IReportService reportService) : IContactUpdateManager
{
    public UpdateResult UpdateContacts(string reportDirectory)
    {
        List<List<Contacts>> contacts = updateService.GenerateContactLists();
        Result success = updateService.ExecuteContactUpdateAsync(contacts);
        Result<DirectoryInfo> report = reportService.GenerateContactsReport(contacts, reportDirectory);
        
        return new(success, report);
    }
}