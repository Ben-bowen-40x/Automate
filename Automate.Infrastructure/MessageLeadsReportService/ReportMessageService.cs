using Automate.Application.InfrastructureInterfaces;
using Automate.Domain.SolutionFunctionality;
using Automate.Domain.ValueObjects;
using Automate.Infrastructure.CsvManipulationService;
using Automate.Infrastructure.DatabaseService;
using Automate.Infrastructure.JsonManipulationService;
using Automate.Infrastructure.MessageLeadsService.DbMaps;
using Automate.Infrastructure.MessageLeadsService.JsonMaps;
using Automate.Translation.InfrastructureInterfaces.Customer;
using Automate.Translation.InfrastructureInterfaces.Message;
using Automate.Translation.ValueObjectsTranslations;
using CSharpFunctionalExtensions;

namespace Automate.Infrastructure.MessageLeadsReportService;

public class ReportMessageService(IDwhSettings settings) : IReportMessageService
{
    readonly RawQuery _rawQuery = new(settings);

    #region Pathing
    // Parent folder
    private const string _fileLoc = @".info\MessageAnalysis";

    // Csv File Names
    private const string _customerRecordQuery = "MessageCustSubQuery.sql";
    private const string _callRecordQuery = "MessageCallQuery.sql";
    private const string _messagesLocation = "MessagesToAnalyze.csv";

    // Json File Names
    private const string _callRecordRepo = @"LocalRepo\CallRepo.json";
    private const string _customerRecordRepo = @"LocalRepo\CustomerRepo.json";

    // File Locations
    private string? loc;
    public string Loc => loc ??= FolderFinder.GetLocalFolder(nameof(Infrastructure), _fileLoc);
    private string Location(string file) => Loc + file;

    #endregion

    #region Query helpers
    /// <summary>
    /// This is for testing purposes only.
    /// </summary>
    internal bool QueryDbCalls { get; set; }
    internal bool QueryDbCustomers { get; set; }
    private DateTimeOffset _startDate = DateTimeOffset.Now - TimeSpan.FromDays(365);

    // Lists for Local Repo
    private List<ICallRecord>? _callRecordsFromRepo;
    private List<ICustomerSubscription>? _customerRecordsFromRepo;
    #endregion

    #region Implementation
    public List<IMessage> RetrieveReportMessages(string reportLocation, out List<QualifiedMessageRecord> records)
    {
        // Check to see whether the report file actually exists. If not, create it
        if (!File.Exists(reportLocation))
            File.WriteAllText(reportLocation, string.Empty);

        // Retrieve messages from report
        Result<List<MessageReportMap>> result = CsvService.Parse<MessageReportMap>(reportLocation);
        IEnumerable<MessageReportMap> reportColumns = result.IsSuccess
            ? result.Value
            : throw new Exception(result.Error);

        // Convert report columns to IMessage
        List<IMessage> reportRecords = reportColumns.Select(m => m.Convert<MessageReportMap, IMessage>()).ToList();

        // Convert report columns to qualified messages
        records = reportColumns.Select(m => m.ConvertToQualifiedRecord()).ToList();

        return reportRecords;
    }

    public List<IMessage> GetMessages<T>(string messageLocation) where T : IConvert
    {
        // Retrieve Messages
        string msgLocStr = messageLocation == string.Empty
            ? Location(_messagesLocation)
            : messageLocation;
        Result<List<T>> result = CsvService.Parse<T>(msgLocStr);
        IEnumerable<T> messageCol = result.IsSuccess
            ? result.Value
            : throw new Exception(result.Error);

        // Convert from column type to IMessage type...
        List<IMessage> msgs = messageCol
            .Select(m => m.Convert<T, IMessage>()).ToList();

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

    private bool WhetherToQueryDB(List<IMessage> uniqueMsgs, out bool queryCustomers)
    {
        // Use the most recent message from the list of messages to determine whether the local repos need to be updated
        IMessage recentMsg = FindMostRecent(uniqueMsgs);
        IMessage firstMsg = FindFirst(uniqueMsgs);
        _startDate = firstMsg.Date;
        queryCustomers = CustomerRepoNeedsUpdate(recentMsg, Location(_customerRecordRepo));
        bool result = CallRepoNeedsUpdate(recentMsg, firstMsg, Location(_callRecordRepo));
        return result;
    }

    public List<ICallRecord> GetCallRecords(List<long> msgNums, string callRepo)
    {
        // Prepare the repo location
        FileInfo callLocation = callRepo == string.Empty || !File.Exists(callRepo)
            ? new(Location(_callRecordRepo))
            : new(callRepo);

        Result<List<CallRecordJsonReader>> result = JsonService.ReadFile<CallRecordJsonReader>(callLocation.FullName);
        List<CallRecordJsonReader> localCalls = result.IsSuccess
            ? result.Value
            : throw new Exception(result.Error);
        IEnumerable<ICallRecord> filteredCalls = localCalls
            .Select(c => c.Convert())
            .Where(c =>
                msgNums
                .Contains(c.Number.Number)
             );
        return filteredCalls.ToList();
    }

    public List<ICallRecord> GetCallRecords(string callLoc)
    {
        string callLocRepo = Location(_callRecordRepo);
        if (QueryDbCalls)
        {
            // Retrieve Calls using Db
            DwhContext<CallDbEntity> callContext = new(settings.CallsConnectionString!);
            FileInfo callLocation = callLoc == string.Empty || !File.Exists(callLoc)
                ? new(Location(_callRecordQuery))
                : new(callLoc);
            string query = _rawQuery.MessageCallQuery(_startDate);
            try
            {
                Task<IEnumerable<CallDbEntity>> callTask =
                    callLoc == string.Empty
                    ? DwhContextHelpers.GetItemsFromRawAsync(callContext, query)
                    : DwhContextHelpers.GetItemsFromFileAsync(callContext, callLocation);
                IEnumerable<CallDbEntity> calls = callTask.Result;
                List<ICallRecord> resultList = calls
                    .Select(c => c.Convert())
                    .ToList();

                // Save results to local repo
                JsonService.WriteToFile(callLocRepo, resultList);
                return resultList;
            }
            catch (Exception ex)
            {
                string member = nameof(GetCallRecords);
                StringLogger.AddLog($"Failed to query DB in: {GetFullName.GetMemberName(new ReportMessageService(settings), member)}", "An exception arose while attempting to query the database. Exception:", ex.ToString());
            }
        }
        else if (_callRecordsFromRepo != null && _callRecordsFromRepo.Count != 0)
            return _callRecordsFromRepo;

        // Default behavior is to retrieve information from the local repo
        Result<List<CallRecordJsonReader>> r = JsonService.ReadFile<CallRecordJsonReader>(callLocRepo);
        List<CallRecordJsonReader> localCalls = r.IsSuccess
            ? r.Value
            : throw new Exception(r.Error);
        IEnumerable<ICallRecord> result = localCalls.Select(c => c.Convert());
        return result.ToList();
    }

    public List<ICustomerSubscription> GetCustomerRecords(List<long> msgNums, string customerRepo)
    {
        // Prepare the repo location. This is the default location
        string customerLocation = customerRepo == string.Empty || !File.Exists(customerRepo)
            ? Location(_customerRecordRepo)
            : customerRepo;

        Result<List<CustSubJsonReader>> result = JsonService.ReadFile<CustSubJsonReader>(customerLocation);
        List<CustSubJsonReader> localCustomers = result.IsSuccess
            ? result.Value
            : throw new Exception(result.Error);
        IEnumerable<ICustomerSubscription> filteredCustomers = localCustomers
            .Select(c => c.Convert())
            .Where(c =>
                msgNums
                .Contains(c.Number.Number)
            );
        return filteredCustomers.ToList();

    }

    public List<ICustomerSubscription> GetCustomerRecords(string customerLocation)
    {
        string customerLocRepo = Location(_customerRecordRepo);
        if (QueryDbCustomers)
        {
            // Retrieve Customers
            DwhContext<CustSubDbEntity> customerContext = new(settings.CustomersConnectionString!);
            FileInfo custStr =
                customerLocation == string.Empty || !File.Exists(customerLocation)
                ? new(Location(_customerRecordQuery))
                : new(customerLocation);
            string query = _rawQuery.MessageCustomerQuery();
            try
            {
                Task<IEnumerable<CustSubDbEntity>> customerTask =
                    customerLocation == string.Empty
                    ? DwhContextHelpers.GetItemsFromRawAsync(customerContext, query)
                    : DwhContextHelpers.GetItemsFromFileAsync(customerContext, custStr);
                List<ICustomerSubscription> customers = customerTask.Result
                    .Select(c => (ICustSubIntIdNumberStr)c)
                    .Select(c => c.Convert())
                    .Select(c => (ICustomerSubscription)c)
                    .ToList();

                // Save results to local repo
                JsonService.WriteToFile(customerLocRepo, customers);
                return customers;
            }
            catch (Exception ex)
            {
                string member = nameof(GetCustomerRecords);
                StringLogger.AddLog($"Failed to query DB in: {GetFullName.GetMemberName(new ReportMessageService(settings), member)}", "An exception arose while attempting to query the database. Exception:", ex.ToString());
            }
        }
        else if (_customerRecordsFromRepo != null && _customerRecordsFromRepo.Count != 0)
            return _customerRecordsFromRepo;

        // Exceptions default to local repo retrieval
        Result<List<CustSubJson>> r = JsonService.ReadFile<CustSubJson>(customerLocRepo);
        List<CustSubJson> localCustomers = r.IsSuccess
            ? r.Value
            : throw new Exception(r.Error);
        List<ICustomerSubscription> result = localCustomers
            .Select(c => (ICustSubLongIdPhoneNumber)c)
            .Select(c => c.Convert())
            .Select(c => (ICustomerSubscription)c)
            .ToList();
        return result;
    }
    #endregion

    #region Private Members
    private bool CallRepoNeedsUpdate(IMessage recentMsg, IMessage firstMsg, string repoLocation)
    {
        // Find out whether the local repository contains records up to the most recent text message
        // Ensure the local file repos exist and are not empty
        if (!File.Exists(repoLocation) || File.ReadAllText(repoLocation) == string.Empty)
            return true;

        // Prepare extraction of calls and customers from local repo
        List<CallRecordJson> localCalls = [];
        try
        {
            // Extract the local repo of calls
            var result = JsonService.ReadFile<CallRecordJson>(repoLocation);
            localCalls = result.IsSuccess
                ? result.Value
                : throw new Exception(); // The error message for this exception is not used, so no need to carry it through
        }
        catch
        {
            return true;
        }

        // If the most recent msg occurred before the most recent call AND the first msg occurred after the first call, set the local field to the list of call records
        CallRecordJson recentCall = FindMostRecent(localCalls);
        CallRecordJson firstCall = FindFirst(localCalls);
        if (DateTimeOffset.Compare(firstMsg.Date - _rawQuery.NinetyDays, firstCall.Date) > 0 && DateTimeOffset.Compare(recentMsg.Date, recentCall.Date) < 0)
        {
            IEnumerable<ICallRecord> convertedCalls = ConvertCallsFromRepo(localCalls);
            _callRecordsFromRepo = convertedCalls.ToList();
        }
        // Recent msgs are not covered by the repo, so it must be renewed by the Db, further calculations are unnecessary, so we can return here
        else
            return true;

        // Return
        return false;

        // Local
        static IEnumerable<ICallRecord> ConvertCallsFromRepo(List<CallRecordJson> localCalls)
        {
            return localCalls.Select(m => m.Convert());
        }
    }

    private bool CustomerRepoNeedsUpdate(IMessage recentMsg, string repoLocation)
    {
        // Find out whether the local repository contains records up to the most recent text message
        // Ensure the local file repos exist and are not empty
        if (!File.Exists(repoLocation) || File.ReadAllText(repoLocation) == string.Empty)
            return true;

        // Prepare extraction of calls and customers from local repo
        List<CustSubJson> localCalls = [];
        try
        {
            // Extract the local repo of calls
            var result = JsonService.ReadFile<CustSubJson>(repoLocation);
            localCalls = result.IsSuccess
                ? result.Value
                : throw new Exception(); // The exception is not being used, so Result.Error does not need to be carried through
        }
        catch
        {
            return true;
        }

        // If the most recent text occurred before the most recent call, set the local field to the list of call records
        CustSubJson recentCall = FindMostRecent(localCalls);
        if (DateTimeOffset.Compare(recentMsg.Date, recentCall.Date) < 0)
        {
            IEnumerable<ICustomerSubscription> convertedCalls = localCalls.Select(m => m.Convert());
            _customerRecordsFromRepo = convertedCalls.ToList();
        }
        // Recent texts are not covered by the repo, so it must be renewed by the Db
        // Further calculations are unnecessary, so we can return here
        else
            return true;

        return false;
    }

    private static T FindMostRecent<T>(IEnumerable<T> items) where T : IDatedRecord
    {
        var mostRecent = items.Last();
        foreach (var item in items)
        {
            if (item.Date > mostRecent.Date)
                mostRecent = item;
        }
        return mostRecent;
    }

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
        var leastRecent = items[0];
        foreach (var item in items)
        {
            if (DateTimeOffset.Compare(item.Date, leastRecent.Date) < 0 && item.Date != DateTimeOffset.MinValue && DateTimeOffset.Compare(item.Date, Early) >= 0)
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
            .Distinct()
            .ToList(); // This must either be an array or a list because we want the Length/Count

        // Create a new list that contains the earliest text that matches each unique phone number
        List<IMessage> result = new(numbers.Count);
        foreach (long num in numbers)
        {
            result.Add(
                FindFirst(
                    msgs
                    .Where(i => i.Number.Number == num)
                    .ToList()
                ));
        }

        // Return
        return result;
    }

    #endregion
}
