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

    // Csv File Names
    private const string _messagesLocation = "MessagesToAnalyze.csv";

    // Json File Names
    private const string _callRecordRepo = @"LocalRepo\CallRepo.json";
    private const string _customerRecordRepo = @"LocalRepo\CustomerRepo.json";

    // File Locations
    private DirectoryInfo? loc;
    public DirectoryInfo Loc => loc ??= FolderFinder.GetLocalFolder(nameof(Infrastructure), _fileLoc);
    private FileInfo Location(string file) => new(Loc.FullName + file);
    #endregion

    #region Implementation
    public List<IMessage> RetrieveReportMessages(MessageType type, string reportLocation, out List<QualifiedMessageRecord> records)
    {
        // Check to see whether the report file actually exists. If not, create it
        FileInfo reportLoc = new(reportLocation);
        if (!File.Exists(reportLoc.FullName))
            File.WriteAllText(reportLoc.FullName, string.Empty);

        // Retrieve messages from report
        Result<List<QualifiedMessageMap>> result = CsvService.Parse<QualifiedMessageMap>(reportLoc);
        IEnumerable<QualifiedMessageMap> reportColumns = result.IsSuccess
            ? result.Value
            : throw new Exception(result.Error);

        // Translate report columns to IMessage
        List<IMessage> reportRecords = reportColumns
            .Select(m => m.Convert<QualifiedMessageMap, IMessage>())
            .ToList();

        // Translate report columns to qualified messages
        records = reportColumns
            .Select(m => m.Translate(type))
            .ToList();

        return reportRecords;
    }

    public List<IMessage> GetMessages<T>(FileInfo messageLocation) where T : IConvert
    {
        Result<List<T>> result = CsvService.Parse<T>(messageLocation);
        IEnumerable<T> messageCol = result.IsSuccess
            ? result.Value
            : throw new Exception(result.Error);

        // Translate from column type to IMessage type...
        List<IMessage> msgs = messageCol
            .Select(m => m.Convert<T, IMessage>())
            .ToList();

        // Remove duplicates from message origin
        List<IMessage> uniqueMsgs = RemoveDuplicates(msgs);

        return uniqueMsgs;
    }

    public List<IMessage> PartitionMessagesAndReportRecords(List<IMessage> uniqueMsgs, List<IMessage> reportRecords)
    {
        // Only accept occurrences that do not also occur in the list of report records, by phone number
        List<IMessage> reportAndMessageDiscrepancy = uniqueMsgs // Leave this here for debugging purposes
            .Where(m => FindPartition(reportRecords, m))
            .ToList();
        return reportAndMessageDiscrepancy;

        static bool FindPartition(List<IMessage> reportRecords, IMessage m)
        {
            foreach (var report in reportRecords)
            {
                if (report.Number.Number == m.Number.Number)
                    return false;
            }
            return true;
        }
    }

    public List<ICallRecord> GetCallRecords(List<long> msgNums, FileInfo callRepo)
    {
        Result<List<CallRecordJsonReader>> result = JsonService.ReadFile<CallRecordJsonReader>(callRepo);
        List<CallRecordJsonReader> localCalls = result.IsSuccess
            ? result.Value
            : throw new Exception(result.Error);
        List<ICallRecord> filteredCalls = localCalls
            .Select(c => c.Translate())
            .Where(c => msgNums.Contains(c.Number.Number))
            .ToList();
        return filteredCalls;
    }

    public List<ICustomerSubscription> GetCustomerRecords(List<long> msgNums, FileInfo customerRepo)
    {
        List<ICustomerSubscription> filteredCustomers = GetCustomerRecords(customerRepo)
            .Where(c => msgNums.Contains(c.Number.Number) || msgNums.Contains(c.Number2.Number))
            .ToList();
        return filteredCustomers;
    }
    public List<ICustomerSubscription> GetCustomerRecords(FileInfo customerRepo)
    {
        Result<List<CustSubJsonReader>> result = JsonService.ReadFile<CustSubJsonReader>(customerRepo);
        List<CustSubJsonReader> localCustomers = result.IsSuccess
            ? result.Value
            : throw new Exception(result.Error);

        List<ICustomerSubscription> translated = localCustomers
            .Select(c => c.Translate())
            .ToList();

        return translated;
    }
    #endregion

    #region Private Members
    private static DateTimeOffset? _twelve;
    private static DateTimeOffset Early => _twelve ??= new(new DateTime(2012, 1, 1)); // This is a sufficient amount of time in the past

    /// <summary>
    /// <paramref name="items"/> must be of type <see cref="IList{T}"/> because we will be using the indices of <paramref name="items"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="items"></param>
    /// <returns></returns>
    private static T FindFirst<T>(IList<T> items) where T : IDatedRecord
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
        List<long> numbers = msgs
            .Select(i => i.Number.Number)
            .Where(i => i != PhoneNumber.Default)
            .Distinct() // Yes, this makes the time complexity O(3N), but it's really not that different than if we did the same thing in a loop
            .ToList(); // This must either be an array or a list because we want the Length/Count

        // Create a new list that contains the chronologically earliest text that matches each phone number
        List<IMessage> result = new(numbers.Count);
        foreach (long num in numbers)
        {
            List<IMessage> shortList = msgs
                .Where(i => i.Number.Number == num)
                .ToList();
            IMessage first = FindFirst(shortList);
            result.Add(first);
        }

        return result;
    }

    #endregion
}
