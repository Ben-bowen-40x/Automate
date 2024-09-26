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
        // Retrieve Items
        List<IMessage> reportMsgs = _msgService.RetrieveReportMessages(report, out List<QualifiedMessageRecord> records);
        List<IMessage> msgs = _msgService.GetMessages<T>(messages);
        List<IMessage> messagePartitions = _msgService.PartitionMessagesAndReportRecords(msgs, reportMsgs);
        List<long> num = messagePartitions.Select(m => m.Number.Number).ToList();
        List<ICallRecord> calls = _msgService.GetCallRecords(num, callsFile);
        List<ICustomerSubscription> customers = _msgService.GetCustomerRecords(num, customersFile);

        // Process Items
        List<QualifiedMessageRecord> qualified = MessageQualifier.Qualify(messagePartitions, calls, customers);

        // Collect the report together
        List<QualifiedMessageRecord> reportRecords = [.. records, .. qualified];

        // Generate Report
        var success = _reportService.GenerateMessageLeadReportAppend(reportDefault, reportRecords, report);

        return success;
    }
}
