using Automate.Application.InfrastructureInterfaces;
using Automate.Domain.ValueObjects;
using CSharpFunctionalExtensions;

namespace Automate.Application.UpdateContacts;

public class ContactUpdateManager(IUpdateContactsService updateService, IReportService reportService) : IContactUpdateManager
{
    public UpdateResult UpdateContacts(string reportDirectory)
    {
        List<List<Contacts>> contacts = updateService.GenerateContactLists();
        Task<bool> success = updateService.ExecuteContactUpdateAsync(contacts);
        bool resultResult = success.Result;
        Result<DirectoryInfo> report = reportService.GenerateContactsReport(contacts, reportDirectory);
        
        return new(resultResult, report);
    }
}
public record UpdateResult(bool UploadedContacts, Result<DirectoryInfo> ContactLocation);
public record Contacts(PhoneNumber Number, PhoneNumber Phone2);