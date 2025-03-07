using Automate.Application.InfrastructureInterfaces;
using Automate.Domain.MessageAnalysis;
using Automate.Domain.ValueObjects;
using CSharpFunctionalExtensions;

namespace Automate.Application.MessageReportAnalysis;
public class MessageAnalysisReportManager(IReportMessageService msgService, IReportService reportService) : IMessageAnalysisReportManager
{
    private readonly IReportMessageService _msgService = msgService;
    private readonly IReportService _reportService = reportService;

    public Result<FileInfo> Manage<T>(string reportDefault, string messages, string callsFile, string customersFile, string report, MessageType type) where T : IConvert
    {
        List<QualifiedMessageRecord> reportRecords = Commond<T>(messages, callsFile, customersFile, report, type);

        // Generate Report
        Result<FileInfo> success = _reportService.GenerateMessageLeadReport(reportDefault, reportRecords, report);

        return success;
    }

    public const int DefaultDays = 120;
    public Result<FileInfo> Manage<T>(string reportDefault, string messages, string callsFile, string customersFile, string report, string truncatedReport, bool truncate, MessageType type, int days = DefaultDays) where T : IConvert
    {
        List<QualifiedMessageRecord> reportRecords = Commond<T>(messages, callsFile, customersFile, report, type);

        // Truncate the report 
        if (truncate)
        {
            DateTimeOffset past = DateTimeOffset.Now - TimeSpan.FromDays(days);
            List<QualifiedMessageRecord> truncatedRecords = reportRecords.Where(r => DateTimeOffset.Compare(past, r.Message.Date) <= 0).ToList();
            var result = _reportService.GenerateMessageLeadReport(reportDefault + "Truncated" + days, truncatedRecords, truncatedReport);
            Result<FileInfo> appended = _reportService.GenerateMessageLeadReport(reportDefault, reportRecords, report);
            if (result.IsSuccess)
                return result;
            if (result.IsFailure && appended.IsSuccess)
                return appended;
            if (result.IsFailure && appended.IsFailure)
                return Result.Failure<FileInfo>(result.Error + "\n" + appended.Error);
        }

        // Generate Report
        Result<FileInfo> success = _reportService.GenerateMessageLeadReport(reportDefault, reportRecords, report);

        return success;
    }

    private List<QualifiedMessageRecord> Commond<T>(string messages, string callsFile, string customersFile, string report, MessageType type, bool reset = false) where T : IConvert
    {
        // Retrieve Items This is here
        List<IMessage> reportMsgs = _msgService.RetrieveReportMessages(type, report, out List<QualifiedMessageRecord> records);
        List<IMessage> msgs = _msgService.GetMessages<T>(messages);

        //reset = true; // This should not execute except when customer information should be reset in the report. VERY RARE
        records = reset ? ResetRecords(customersFile, records) : records;

        // Messages
        List<IMessage> messagePartitions = _msgService.PartitionMessagesAndReportRecords(msgs, reportMsgs);
        List<long> num = messagePartitions.Select(m => m.Number.Number).ToList();

        // Retrieve items specific to messages
        List<ICallRecord> calls = _msgService.GetCallRecords(num, callsFile);
        List<ICustomerSubscription> customers = _msgService.GetCustomerRecords(num, customersFile);

        // Process Items
        List<QualifiedMessageRecord> qualified = MessageQualifier.Qualify(messagePartitions, calls, customers, type);

        // Collect the report together
        List<QualifiedMessageRecord> result = [.. records, .. qualified];
        return result;
    }

    private List<QualifiedMessageRecord> ResetRecords(string customersFile, List<QualifiedMessageRecord> records)
    {
        // Refresh report data to be consistent with repository
        List<ICustomerSubscription> reportCust = _msgService.GetCustomerRecords(customersFile);
        List<QualifiedMessageRecord> recordsRefreshed = records
            .Select(r => ResetRecords(r, reportCust))
            .ToList();
        records = recordsRefreshed; // If the report record could not be found in the list of customers, then the original record's customer is used

        return records;

        #region Local
        static QualifiedMessageRecord ResetRecords(QualifiedMessageRecord r, List<ICustomerSubscription> reportCust)
        {
            // Find Customer
            long rph = r.Customer.Number.Number;
            long r2ph = r.Customer.Number2.Number;
            long rsubid = r.Customer.SubscriptionId;
            ICustomerSubscription? newc = reportCust.FirstOrDefault(c => //c.Number.Number == rph || c.Number2.Number == r2ph || 
                c.SubscriptionId == rsubid);
            ICustomerSubscription newcust = newc ?? r.Customer; // This means that the report contains subscription records that don't exist in the repo. This happens all the time because there are default values.

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
