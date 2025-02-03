using Automate.Domain.ValueObjects;
using CSharpFunctionalExtensions;

namespace Automate.Application.InfrastructureInterfaces;

public interface IReportService
{
    Result<FileInfo> AppendMessageLeadReport(List<QualifiedMessageRecord> messages, string reportLocation);
    Result<DirectoryInfo> GenerateContactsReport(List<List<Contact>> contacts, string reportDirectory);
    Result<FileInfo> GenerateDiscrepancyReport(List<DiscrepancyMatch> matches, string reportLocation);
    Result<FileInfo> GenerateMessageLeadReport(string reportDefault, List<QualifiedMessageRecord> messages, string reportLocation);
    Result<FileInfo> GenerateLeafMessages(List<IMessage> msgs, string location);
    Result<FileInfo> GenerateMessageLeadReportAppend(string reportDefault, List<QualifiedMessageRecord> messages, string reportLocation);
}
