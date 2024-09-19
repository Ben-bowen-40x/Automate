using Automate.Application.InfrastructureInterfaces;
using Automate.Domain.SolutionFunctionality;
using Automate.Domain.ValueObjects;
using Automate.Infrastructure.CsvService;
using Automate.Infrastructure.DatabaseService;
using Automate.Infrastructure.JsonService;
using Automate.Infrastructure.MessageLeadsService.DbMaps;
using Automate.Infrastructure.MessageLeadsService.JsonMaps;
using Automate.Infrastructure.QueryService;

namespace Automate.Infrastructure.MessageLeadsService;

public class MessageService(IDwhSettings settings) : IMessageService
{
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
    public List<IMessage> GetMessages<T>(string msgs) where T : IConvert
    {
        // Retrieve Messages
        string msgLocStr = msgs == string.Empty ? Location(_messagesLocation) : msgs;
        IEnumerable<T> messageCol = CsvRW.ParseFromCsv<T>(msgLocStr);

        // Convert from column type to IMessage type
        IEnumerable<IMessage> messages = messageCol.Select(m => m.Convert<T, IMessage>());
        List<IMessage> msgList = messages.ToList();

        // Remove duplicates
        List<IMessage> uniqueMsgs = RemoveDuplicates(msgList);

        // Use the most recent message to determine whether the local repos need to be updated
        IMessage recentMsg = FindMostRecent(uniqueMsgs);
        IMessage firstMsg = FindFirst(uniqueMsgs);
        _startDate = firstMsg.Date;
        QueryDbCalls = CallRepoNeedsUpdate(recentMsg, firstMsg, Location(_callRecordRepo));
        QueryDbCustomers = CustomerRepoNeedsUpdate(recentMsg, Location(_customerRecordRepo));

        return uniqueMsgs;
    }

    public List<ICallRecord> GetCallRecords(string callLoc)
    {
        string callLocRepo = Location(_callRecordRepo);
        if (QueryDbCalls)
        {
            // Retrieve Calls using Db
            DwhContext<CallDbEntity> callContext = new(settings.CallsConnectionString!);
            FileInfo callLocation = callLoc == string.Empty || !File.Exists(callLoc) ? new(Location(_callRecordQuery)) : new(callLoc);
            string query = RawQuery.MessageCallQuery(_startDate);
            try
            {
                Task<IEnumerable<CallDbEntity>> callTask =
                    callLoc == string.Empty
                    ? DwhContextHelpers.GetItemsFromRawAsync(callContext, query)
                    : DwhContextHelpers.GetItemsFromFileAsync(callContext, callLocation);
                IEnumerable<CallDbEntity> calls = callTask.Result;
                IEnumerable<ICallRecord> callResult = calls.Select(c => c.Convert());
                List<ICallRecord> resultList = callResult.ToList();

                // Save results to local repo
                JsonRW.SerializeToFile(callLocRepo, resultList);
                return resultList;
            }
            catch (Exception ex)
            {
                string member = nameof(GetCallRecords);
                StringLogger.AddLog($"Failed to query DB in: {GetFullName.GetMemberName(new MessageService(settings), member)}", "An exception arose while attempting to query the database. Exception:", ex.ToString());
            }
        }
        else if (_callRecordsFromRepo != null && _callRecordsFromRepo.Count != 0)
            return _callRecordsFromRepo;

        // Default behavior is to retrieve information from the local repo
        List<CallRecordJson> localCalls = JsonRW.DeserializeFile<CallRecordJson>(callLocRepo);
        IEnumerable<ICallRecord> result = localCalls.Select(c => c.Convert());
        return result.ToList();
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
            string query = RawQuery.MessageCustomerQuery();
            try
            {
                Task<IEnumerable<CustSubDbEntity>> customerTask =
                    customerLocation == string.Empty
                    ? DwhContextHelpers.GetItemsFromRawAsync(customerContext, query)
                    : DwhContextHelpers.GetItemsFromFileAsync(customerContext, custStr);
                IEnumerable<CustSubDbEntity> customers = customerTask.Result;
                IEnumerable<ICustomerSubscription> records = customers.Select(c => c.Convert());
                List<ICustomerSubscription> resultList = records.ToList();

                // Save results to local repo
                JsonRW.SerializeToFile(customerLocRepo, resultList);
                return resultList;
            }
            catch (Exception ex)
            {
                string member = nameof(GetCustomerRecords);
                StringLogger.AddLog($"Failed to query DB in: {GetFullName.GetMemberName(new MessageService(settings), member)}", "An exception arose while attempting to query the database. Exception:", ex.ToString());
            }
        }
        else if (_customerRecordsFromRepo != null && _customerRecordsFromRepo.Count != 0)
            return _customerRecordsFromRepo;

        // Exceptions default to local repo retrieval
        List<CustSubJson> localCustomers = JsonRW.DeserializeFile<CustSubJson>(customerLocRepo);
        IEnumerable<ICustomerSubscription> result = localCustomers.Select(c => c.Convert());
        return result.ToList();
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
            localCalls = JsonRW.DeserializeFile<CallRecordJson>(repoLocation);
        }
        catch
        {
            return true;
        }

        // If the most recent msg occurred before the most recent call AND the first msg occurred after the first call, set the local field to the list of call records
        CallRecordJson recentCall = FindMostRecent(localCalls);
        CallRecordJson firstCall = FindFirst(localCalls);
        if (DateTimeOffset.Compare(recentMsg.Date, recentCall.Date) < 0 && DateTimeOffset.Compare(firstMsg.Date - RawQuery.NinetyDays, firstCall.Date) > 0)
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
        if (!File.Exists(repoLocation))
            return true;

        // Prepare extraction of calls and customers from local repo
        List<CustSubJson> custRepo = [];
        try
        {
            // Extract the local repo of calls
            custRepo = JsonRW.DeserializeFile<CustSubJson>(repoLocation);
            if (custRepo.Count == 0)
                return true;
        }
        catch
        {
            return true;
        }

        // If the most recent text occurred before the most recent call, set the local field to the list of call records
        CustSubJson recentCall = FindMostRecent(custRepo);
        if (DateTimeOffset.Compare(recentMsg.Date, recentCall.Date) < 0)
        {
            IEnumerable<ICustomerSubscription> convertedCalls = custRepo.Select(m => m.Convert());
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

    private static DateTimeOffset? _back;
    private static DateTimeOffset Back => _back ??= new(new DateTime(2012, 1, 1));
    private static T FindFirst<T>(IList<T> items) where T : IDatedRecord
    {
        var leastRecent = items[0];
        foreach (var item in items)
        {
            if (DateTimeOffset.Compare(item.Date, leastRecent.Date) < 0 && DateTimeOffset.Compare(item.Date, Back) > 0 && item.Date != DateTimeOffset.MinValue)
                leastRecent = item;
        }
        return leastRecent;
    }
    #endregion

    #region Internal Members
    internal static List<IMessage> RemoveDuplicates(IEnumerable<IMessage> msgs)
    {
        // Remove duplicated phone numbers. Also remove any phone numbers that defaulted because they won't be useful
        List<long> numbers = msgs.Select(i => i.Number.Number).Where(i => i != PhoneNumber.Default).Distinct().ToList();

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
