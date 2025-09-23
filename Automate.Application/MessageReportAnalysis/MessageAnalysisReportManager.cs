using Automate.Application.InfrastructureInterfaces;
using Automate.Domain.MessageAnalysis;
using Automate.Domain.ValueObjects;
using CSharpFunctionalExtensions;

namespace Automate.Application.MessageReportAnalysis;
public class MessageAnalysisReportManager(IReportMessageService msgService, IReportService reportService) : IMessageAnalysisReportManager
{
    private readonly IReportMessageService _msgService = msgService;
    private readonly IReportService _reportService = reportService;

    public Result<FileInfo> Manage<T>(string reportDefault, FileInfo messages, FileInfo callsFile, FileInfo customersFile, string report, MessageType type) where T : IConvert
    {
        List<QualifiedMessageRecord> reportRecords = GetReportRecords<T>(messages, callsFile, customersFile, report, type);

        // Generate Report
        Result<FileInfo> success = _reportService.GenerateMessageLeadReport(reportDefault, reportRecords, report);

        return success;
    }

    public const int DefaultDays = 120;
    public static int DefaultYtd = (DateTime.Today - new DateTime(DateTime.Now.Year, 1, 1)).Days;
    public static int DefaultLastYtd = (DateTime.Today - new DateTime(DateTime.Now.Year - 1, 1, 1)).Days;
    public Result<FileInfo> Manage<T>(string reportDefault, FileInfo messages, FileInfo callsFile, FileInfo customersFile, string report, string truncatedReport, bool truncate, MessageType type, int days = DefaultDays) where T : IConvert
    {
        List<QualifiedMessageRecord> reportRecords = GetReportRecords<T>(messages, callsFile, customersFile, report, type);

        // Truncate the report by filtering from a specific number of days
        if (truncate)
        {
            DateTimeOffset past = DateTimeOffset.Now - TimeSpan.FromDays(days);
            List<QualifiedMessageRecord> truncatedRecords = reportRecords.Where(r => DateTimeOffset.Compare(past, r.Message.Date) <= 0).ToList();
            var result = _reportService.GenerateMessageLeadReport(reportDefault + "Truncated" + days, truncatedRecords, truncatedReport);
            Result<FileInfo> appended = _reportService.GenerateMessageLeadReport(reportDefault, reportRecords, report);
            return (result.IsSuccess, appended.IsSuccess) switch
            {
                (true, true) or (true, false) => result,
                (false, true) => appended,
                (false, false) => Result.Failure<FileInfo>(result.Error + "\n" + appended.Error)
            };
        }

        // Generate Report
        Result<FileInfo> success = _reportService.GenerateMessageLeadReport(reportDefault, reportRecords, report);

        return success;
    }

    List<QualifiedMessageRecord> GetReportRecords<T>(FileInfo messages, FileInfo callsFile, FileInfo customersFile, string report, MessageType type) where T : IConvert
    {
        // Retrieve Items This is here
        List<QualifiedMessageRecord> records = _msgService.RetrieveReportMessages(type, report);
        List<IMessage> reportMsgs = [.. records.Select(r => r.Message)];
        List<IMessage> msgs = _msgService.GetMessages<T>(messages);

        // Customer information regularly updates in the repo, which could change the outcome of the analysis
        records = ResetRecords(customersFile, records);

        // This should not execute except when message information should be reset in the report
        records = ResetMessages(msgs, records);

        // Messages
        List<IMessage> messagePartitions = _msgService.PartitionMessagesAndReportRecords(msgs, reportMsgs);
        List<long> num = [.. messagePartitions.Select(m => m.Number.Number)];

        // Retrieve items specific to messages
        List<ICallRecord> calls = _msgService.GetCallRecords(num, callsFile);
        List<ICustomerSubscription> customers = _msgService.GetCustomerRecords(num, customersFile);

        // Process Items
        List<QualifiedMessageRecord> qualified = MessageQualifier.Qualify(messagePartitions, calls, customers, type);

        // Collect the report together
        List<QualifiedMessageRecord> result = [.. records, .. qualified];
        return result;
    }

    static List<QualifiedMessageRecord> ResetMessages(List<IMessage> msgs, List<QualifiedMessageRecord> records)
    {
        // Refresh report data to be consistent with Messages repository
        List<QualifiedMessageRecord> recordsrefreshed = records
            .Select(r => resetRecords(r, msgs))
            .ToList();
        return recordsrefreshed;

        // local
        static QualifiedMessageRecord resetRecords(QualifiedMessageRecord r, List<IMessage> msgs)
        {
            var newMessage = msgs.First(m => m.Number.Number == r.Message.Number.Number);
            return new QualifiedMessageRecord(newMessage, r.Customer, r.Billable, r.IsSalesLead, r.Type);
        }
    }

    List<QualifiedMessageRecord> ResetRecords(FileInfo customersFile, List<QualifiedMessageRecord> records)
    {
        // Refresh report data to be consistent with repository
        List<ICustomerSubscription> reportCust = _msgService.GetCustomerRecords(customersFile);
        List<QualifiedMessageRecord> recordsRefreshed = [.. records.Select(r => resetRecords(r, reportCust))];
        records = recordsRefreshed; // If the report record could not be found in the list of customers, then the original record's customer is used

        return records;

        #region Local
        static QualifiedMessageRecord resetRecords(QualifiedMessageRecord r, List<ICustomerSubscription> reportCust)
        {
            // Find Customer
            long rph = r.Customer.Number.Number;
            long r2ph = r.Customer.Number2.Number;
            long rsubid = r.Customer.SubscriptionId;
            ICustomerSubscription? newc = reportCust.FirstOrDefault(c => //c.Number.Number == rph || c.Number2.Number == r2ph || 
                c.SubscriptionId == rsubid);
            ICustomerSubscription newcust = newc ?? r.Customer; // This means that the report contains subscription records that don't exist in the repo. This happens all the time because the report cannot use NULL: the report uses default values that either do not exist in the repo, or they exist as NULL.

            // Customer Matches
            bool idMatch = r.Customer.CustomerId == newcust.CustomerId;
            bool subMatch = r.Customer.SubscriptionId == newcust.SubscriptionId;
            bool dtmatch = DateTime.Compare(r.Customer.Date.DateTime, newcust.Date.DateTime) == 0; // DateTimeOffset.DateTime does not do any weird conversions
            bool subDtMatch = DateTime.Compare(r.Customer.SubscriptionStartDate.DateTime, newcust.SubscriptionStartDate.DateTime) == 0;
            bool cxlDtMatch = DateTime.Compare(r.Customer.CustomerCancelDate.DateTime, newcust.CustomerCancelDate.DateTime) == 0;
            bool sCxlDtMatch = DateTime.Compare(r.Customer.SubscriptionCancelDate.DateTime, newcust.SubscriptionCancelDate.DateTime) == 0;
            bool activeMatch = r.Customer.CustomerActive == newcust.CustomerActive;
            bool subActMatch = r.Customer.SubscriptionActive == newcust.SubscriptionActive;
            bool initialMatch = r.Customer.InitialCompleted == newcust.InitialCompleted;
            bool phMatch = r.Customer.Number.Number == newcust.Number.Number;
            bool ph2Match = r.Customer.Number2.Number == newcust.Number2.Number;
            bool cvMatch = r.Customer.ContractValue == newcust.ContractValue;
            bool sellMatch = r.Customer.Sellers.Equals(newcust.Sellers, StringComparison.CurrentCultureIgnoreCase);
            bool customerMatches = idMatch && subMatch && dtmatch && subDtMatch && cxlDtMatch && sCxlDtMatch && activeMatch && subActMatch && initialMatch && phMatch && ph2Match && cvMatch && sellMatch;

            QualifiedMessageRecord result = new(r.Message, newcust, r.Billable, r.IsSalesLead, r.Type);
            return result;
        }
        #endregion
    }
}
