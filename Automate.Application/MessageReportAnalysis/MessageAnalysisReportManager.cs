using Automate.Application.InfrastructureInterfaces;
using Automate.Domain.MessageAnalysis;
using Automate.Domain.ValueObjects;
using CSharpFunctionalExtensions;

namespace Automate.Application.MessageReportAnalysis;
public class MessageAnalysisReportManager(IReportMessageService msgService, IReportService reportService) : IMessageAnalysisReportManager
{
    private readonly IReportMessageService _msgService = msgService;
    private readonly IReportService _reportService = reportService;

    public Result<FileInfo> Manage<T>(string reportDefault, string messages, string callsFile, string customersFile, string report) where T : IConvert
    {
        List<QualifiedMessageRecord> reportRecords = Commond<T>(messages, callsFile, customersFile, report);

        // Generate Report
        Result<FileInfo> success = _reportService.GenerateMessageLeadReportAppend(reportDefault, reportRecords, report);

        return success;
    }

    private const int _days = 120;
    public Result<FileInfo> Manage<T>(string reportDefault, string messages, string callsFile, string customersFile, string report, string truncatedReport, bool truncate, int days = _days) where T : IConvert
    {
        List<QualifiedMessageRecord> reportRecords = Commond<T>(messages, callsFile, customersFile, report);

        // Truncate the report 
        if (truncate)
        {
            DateTimeOffset past = DateTimeOffset.Now - TimeSpan.FromDays(days);
            List<QualifiedMessageRecord> truncatedRecords = reportRecords.Where(r => DateTimeOffset.Compare(past, r.Message.Date) <= 0).ToList();
            var result = _reportService.GenerateMessageLeadReport(reportDefault + "Truncated" + days, truncatedRecords, truncatedReport);
            Result<FileInfo> appended = _reportService.GenerateMessageLeadReportAppend(reportDefault, reportRecords, report);
            if (result.IsSuccess)
                return result;
            if (result.IsFailure && appended.IsSuccess)
                return appended;
            if (result.IsFailure && appended.IsFailure)
                return Result.Failure<FileInfo>(result.Error + "\n" + appended.Error);
        }

        // Generate Report
        Result<FileInfo> success = _reportService.GenerateMessageLeadReportAppend(reportDefault, reportRecords, report);

        return success;
    }

    private List<QualifiedMessageRecord> Commond<T>(string messages, string callsFile, string customersFile, string report) where T : IConvert
    {
        // Retrieve Items This is here
        List<IMessage> reportMsgs = _msgService.RetrieveReportMessages(report, out List<QualifiedMessageRecord> records);
        List<IMessage> msgs = _msgService.GetMessages<T>(messages);
        List<IMessage> messagePartitions = _msgService.PartitionMessagesAndReportRecords(msgs, reportMsgs);
        List<long> num = messagePartitions.Select(m => m.Number.Number).ToList();
        List<ICallRecord> calls = _msgService.GetCallRecords(num, callsFile);
        List<ICustomerSubscription> customers = _msgService.GetCustomerRecords(num, customersFile);

        // Process Items
        List<QualifiedMessageRecord> qualified = MessageQualifier.Qualify(messagePartitions, calls, customers);

        // Collect the report together
        return [.. records, .. qualified];
    }
}
