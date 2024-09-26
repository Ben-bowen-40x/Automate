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
        List<IMessage> msgs = _textService.GetMessages<T>(messages);
        List<long> nums = msgs.Select(t => t.Number.Number).ToList();
        List<ICallRecord> calls = _textService.GetCallRecords(nums, callQuery);
        List<ICustomerSubscription> customers = _textService.GetCustomerRecords(nums, customerQuery);

        // Process Items
        List<QualifiedMessageRecord> qualified = MessageQualifier.Qualify(msgs, calls, customers);

        // Generate Report
        Result<FileInfo> file = _reportService.GenerateMessageLeadReport(reportDefault, qualified, report);

        return file;
    }
}
