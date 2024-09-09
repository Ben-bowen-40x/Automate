using Automate.Application.InfrastructureInterfaces;
using Automate.Domain.MessageAnalysis;
using Automate.Domain.ValueObjects;

namespace Automate.Application.MessageReportAnalysis;
public class MessageAnalysisReportManager(IReportMessageService msgService, IReportService reportService) : IMessageAnalysisReportManager
{
    private readonly IReportMessageService _msgService = msgService;
    private readonly IReportService _reportService = reportService;

    public Dictionary<bool, FileInfo> ManageMessageAnalysis<T>(string messages, string callQuery, string customerQuery, string report) where T : IMessageConvert
    {
        // Retrieve Items
        List<IMessage> reportMsgs = _msgService.RetrieveReportMessages(report, out List<QualifiedMessageRecord> records);
        List<IMessage> msgs = _msgService.GetMessages<T>(messages);
        List<IMessage> messagePartitions = _msgService.PartitionMessagesAndReportRecords(msgs, reportMsgs);
        List<ICallRecord> calls = _msgService.GetCallRecords(callQuery);
        List<ICustomerSubscription> customers = _msgService.GetCustomerRecords(customerQuery);

        // Process Items
        List<QualifiedMessageRecord> qualified = MessageQualifier.Qualify(messagePartitions, calls, customers);

        // Collect the report together
        List<QualifiedMessageRecord> reportRecords = [.. records, .. qualified];

        // Generate Report
        bool success = _reportService.GenerateMessageLeadReport(reportRecords, out FileInfo file, report);

        return new() { { success, file } };
    }
}
