using Automate.Application.InfrastructureInterfaces;
using Automate.Domain.MessageAnalysis;
using Automate.Domain.ValueObjects;

namespace Automate.Application.MessageAnalysis;
public class MessageAnalysisManager(IMessageService textService, IReportService reportService) : IMessageAnalysisManager
{
    private readonly IMessageService _textService = textService;
    private readonly IReportService _reportService = reportService;

    public Dictionary<bool, FileInfo> ManageMessageAnalysis<T>(string reportDefault, string messages, string callQuery, string customerQuery, string report) where T : IMessageConvert
    {
        // Retrieve Items
        List<IMessage> textMessages = _textService.GetMessages<T>(messages);
        List<ICallRecord> calls = _textService.GetCallRecords(callQuery);
        List<ICustomerSubscription> customers = _textService.GetCustomerRecords(customerQuery);

        // Process Items
        List<QualifiedMessageRecord> qualified = MessageQualifier.Qualify(textMessages, calls, customers);

        // Generate Report
        bool success = _reportService.GenerateMessageLeadReport(reportDefault, qualified, out FileInfo file, report);

        return new() { { success, file } };
    }
}
