using Automate.Application.InfrastructureInterfaces;
using Automate.Domain.MessageAnalysis;
using Automate.Domain.ValueObjects;
using CSharpFunctionalExtensions;

namespace Automate.Application.MessageAnalysis;
public class MessageAnalysisManager(IMessageService textService, IReportService reportService) : IMessageAnalysisManager
{
    private readonly IMessageService _textService = textService;
    private readonly IReportService _reportService = reportService;

    public Result<FileInfo> Manage<T>(string reportDefault, string messages, string callQuery, string customerQuery, string report, MessageType type) where T : IConvert
    {
        // Retrieve Items
        List<IMessage> msgs = _textService.GetMessages<T>(messages);
        IEnumerable<long> nums = msgs.Select(t => t.Number.Number);
        List<ICallRecord> calls = _textService.GetCallRecords(nums, callQuery);
        List<ICustomerSubscription> customers = _textService.GetCustomerRecords(nums, customerQuery);

        // Process Items
        List<QualifiedMessageRecord> qualified = MessageQualifier.Qualify(msgs, calls, customers, type);

        // Generate Report
        Result<FileInfo> file = _reportService.GenerateMessageLeadReport(reportDefault, qualified, report);

        return file;
    }

    public Result<FileInfo> Manage<T>(string reportDefault, string messages, string callQuery, string customerQuery, string report, bool truncate, MessageType type, int days = 120) where T : IConvert
    {
        // Retrieve Items
        List<IMessage> msgs = _textService.GetMessages<T>(messages);
        IEnumerable<long> nums = msgs.Select(t => t.Number.Number);
        List<ICallRecord> calls = _textService.GetCallRecords(nums, callQuery);
        List<ICustomerSubscription> customers = _textService.GetCustomerRecords(nums, customerQuery);

        // Process Items
        List<QualifiedMessageRecord> qualified = MessageQualifier.Qualify(msgs, calls, customers, type);

        // Truncate report
        if (truncate)
        {
            DateTimeOffset past = DateTimeOffset.Now - TimeSpan.FromDays(days);
            qualified = qualified.Where(q => DateTimeOffset.Compare(past, q.Message.Date) >= 0).ToList();
        }

        // Generate Report
        Result<FileInfo> file = _reportService.GenerateMessageLeadReport(reportDefault, qualified, report);

        return file;
    }
}
