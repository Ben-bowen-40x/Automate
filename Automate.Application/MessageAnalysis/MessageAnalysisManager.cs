using Automate.Application.InfrastructureInterfaces;
using Automate.Domain.MessageAnalysis;
using Automate.Domain.ValueObjects;
using CSharpFunctionalExtensions;

namespace Automate.Application.MessageAnalysis;
public class MessageAnalysisManager(IMessageService textService, IReportService reportService) : IMessageAnalysisManager
{
    private readonly IMessageService _textService = textService;
    private readonly IReportService _reportService = reportService;

    public Result<FileInfo> Manage<T>(string reportDefault, string messages, string callQuery, string customerQuery, string report) where T : IConvert
    {
        // Retrieve Items
        List<IMessage> textMessages = _textService.GetMessages<T>(messages);
        List<ICallRecord> calls = _textService.GetCallRecords(callQuery);
        List<ICustomerSubscription> customers = _textService.GetCustomerRecords(customerQuery);

        // Process Items
        List<QualifiedMessageRecord> qualified = MessageQualifier.Qualify(textMessages, calls, customers);

        // Generate Report
        Result<FileInfo> file = _reportService.GenerateMessageLeadReport(reportDefault, qualified, report);

        return file;
    }
}
