using Automate.Application.InfrastructureInterfaces;
using Automate.Domain.ValueObjects;

namespace Automate.Application.UpdateContacts;

public class ContactUpdateManager(IUpdateContactsService updateService, IReportService reportService) : IContactUpdateManager
{
    public UpdateResult UpdateContacts(string reportDirectory)
    {
        List<List<Contacts>> contacts = updateService.GenerateContactLists();
        Task<bool> success = updateService.ExecuteContactUpdateAsync(contacts);
        bool resultResult = success.Result;
        bool report = reportService.GenerateContactsReport(contacts, out DirectoryInfo directory, reportDirectory);
        
        return new(resultResult, report, directory);
    }
}
public record UpdateResult(bool UploadedContacts, bool GeneratedContacts, DirectoryInfo ContactLocation);
public record Contacts(PhoneNumber Number, PhoneNumber Phone2);