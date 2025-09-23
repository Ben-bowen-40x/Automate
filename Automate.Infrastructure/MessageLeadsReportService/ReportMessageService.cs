using Automate.Application.InfrastructureInterfaces;
using Automate.Domain.SolutionFunctionality;
using Automate.Domain.ValueObjects;
using Automate.Infrastructure.CsvManipulationService;
using Automate.Infrastructure.DataRetrievalFormats;
using Automate.Infrastructure.JsonManipulationService;
using Automate.Translation.CallTranslate;
using Automate.Translation.CustomerTranslate;
using Automate.Translation.QualifiedMessageTranslate;
using CSharpFunctionalExtensions;

namespace Automate.Infrastructure.MessageLeadsReportService;

public class ReportMessageService : IReportMessageService
{
    #region Pathing
    // Parent folder
    private const string _fileLoc = @".info\MessageAnalysis";

    // File Locations
    private DirectoryInfo? loc;
    public DirectoryInfo Loc => loc ??= FolderFinder.GetLocalFolder(nameof(Infrastructure), _fileLoc);
    #endregion

    #region Implementation
    public List<QualifiedMessageRecord> RetrieveReportMessages(MessageType type, string reportLocation)
    {
        // Check to see whether the report file actually exists. If not, create it
        FileInfo reportLoc = new(reportLocation);
        if (!File.Exists(reportLoc.FullName))
            File.WriteAllText(reportLoc.FullName, string.Empty);

        // Retrieve messages from report
        Result<List<QualifiedMessageMap>> result = CsvService.Parse<QualifiedMessageMap>(reportLoc);
        List<QualifiedMessageMap> reportColumns = result.IsSuccess
            ? result.Value
            : throw new Exception(result.Error);

        // Translate report columns to qualified messages
        List<QualifiedMessageRecord> records = [.. reportColumns.Select(m => m.Translate(type))];

        return records;
    }

    public List<IMessage> GetMessages<T>(FileInfo messageLocation) where T : IConvert
    {
        Result<List<T>> result = CsvService.Parse<T>(messageLocation);
        IEnumerable<T> messageCol = result.IsSuccess
            ? result.Value
            : throw new Exception(result.Error);

        // Translate from column type to IMessage type...
        List<IMessage> msgs = [.. messageCol.Select(m => m.Convert<T, IMessage>())];

        // Remove duplicates from message origin
        List<IMessage> uniqueMsgs = RemoveDuplicates(msgs);

        return uniqueMsgs;
    }

    public List<IMessage> PartitionMessagesAndReportRecords(List<IMessage> uniqueMsgs, List<IMessage> reportRecords)
    {
        // Only accept occurrences that do not also occur in the list of report records, by phone number
        List<IMessage> reportAndMessageDiscrepancy = [.. uniqueMsgs // Leave this here for debugging purposes
            .Where(m => FindPartition(reportRecords, m))];
        return reportAndMessageDiscrepancy;

        static bool FindPartition(List<IMessage> reportRecords, IMessage m)
        {
            foreach (var report in reportRecords)
                if (report.Number.Number == m.Number.Number)
                    return false;
            return true;
        }
    }

    public List<ICallRecord> GetCallRecords(List<long> msgNums, FileInfo callRepo)
    {
        Result<List<CallRecordJsonReader>> result = JsonService.ReadFile<CallRecordJsonReader>(callRepo);
        List<CallRecordJsonReader> localCalls = result.IsSuccess
            ? result.Value
            : throw new Exception(result.Error);
        List<ICallRecord> filteredCalls = [.. localCalls
            .Select(c => c.Translate())
            .Where(c => msgNums.Contains(c.Number.Number))];
        return filteredCalls;
    }

    public List<ICustomerSubscription> GetCustomerRecords(List<long> msgNums, FileInfo customerRepo)
    {
        List<ICustomerSubscription> filteredCustomers = [.. GetCustomerRecords(customerRepo).Where(c => msgNums.Contains(c.Number.Number) || msgNums.Contains(c.Number2.Number))];
        return filteredCustomers;
    }
    public List<ICustomerSubscription> GetCustomerRecords(FileInfo customerRepo)
    {
        Result<List<CustSubJsonReader>> result = JsonService.ReadFile<CustSubJsonReader>(customerRepo);
        List<CustSubJsonReader> localCustomers = result.IsSuccess
            ? result.Value
            : throw new Exception(result.Error);

        List<ICustomerSubscription> translated = [.. localCustomers.Select(c => c.Translate())];

        return translated;
    }
    #endregion

    #region Private Members
    private static DateTimeOffset? _twelve;
    private static DateTimeOffset Early => _twelve ??= new(new DateTime(2012, 1, 1)); // This is a sufficient amount of time in the past
    private static T FindFirst<T>(List<T> items) where T : IDatedRecord
    {
        T leastRecent = items[0];
        foreach (T item in items)
        {
            bool a = DateTimeOffset.Compare(item.Date, leastRecent.Date) < 0;
            bool b = item.Date != DateTimeOffset.MinValue;
            bool c = DateTimeOffset.Compare(item.Date, Early) >= 0;
            bool d = a && b && c;
            if (d)
                leastRecent = item;
        }
        return leastRecent;
    }
    #endregion

    #region Internal Members
    internal static List<IMessage> RemoveDuplicates(IEnumerable<IMessage> msgs)
    {
        // Remove duplicated phone numbers. Also remove any phone numbers that defaulted because they won't be useful
        List<long> numbers = [.. msgs
            .Select(i => i.Number.Number)
            .Where(i => i != PhoneNumber.Default)
            .Distinct()]; // This must either be an array or a list because we want the Length/Count

        // Create a new list that contains the chronologically earliest text that matches each phone number
        List<IMessage> result = new(numbers.Count);
        foreach (long num in numbers)
        {
            List<IMessage> shortList = [.. msgs.Where(i => i.Number.Number == num)];
            IMessage first = FindFirst(shortList);
            result.Add(first);
        }

        return result;
    }
    #endregion
}
