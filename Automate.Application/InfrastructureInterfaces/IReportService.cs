using Automate.Application.UpdateContacts;
using Automate.Domain.ValueObjects;

namespace Automate.Application.InfrastructureInterfaces;

public interface IReportService
{
    bool AppendMessageLeadReport(List<QualifiedMessageRecord> messages, out FileInfo file, string reportLocation);
    bool GenerateContactsReport(List<List<Contacts>> contacts, out DirectoryInfo directory, string reportDirectory);
    bool GenerateDiscrepancyReport(List<DiscrepancyMatch> matches, out FileInfo file, string reportLocation);
    bool GenerateMessageLeadReport(List<QualifiedMessageRecord> messages, out FileInfo file, string reportLocation);
    bool GenerateLeafMessages(List<IMessage> msgs, out FileInfo file, string location);
}
