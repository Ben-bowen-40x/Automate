using Automate.Application.InfrastructureInterfaces;
using Automate.Domain.SolutionFunctionality;
using Automate.Domain.ValueObjects;
using Automate.Infrastructure.CsvManipulationService;
using Automate.Infrastructure.DatabaseService;
using Automate.Infrastructure.DataRetrievalFormats;
using Automate.Infrastructure.JsonManipulationService;
using Automate.Translation.CallTranslate;
using Automate.Translation.CustomerTranslate;
using CSharpFunctionalExtensions;

namespace Automate.Infrastructure.MessageLeadsService;

public class MessageService(IDwhSettings settings) : IMessageService
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
    public List<IMessage> GetMessages<T>(string msgs) where T : IConvert
    {
        // Retrieve Messages
        string msgLocStr = msgs == string.Empty ? Location(_messagesLocation) : msgs;
        Result<List<T>> result = CsvService.Parse<T>(msgLocStr);
        IEnumerable<T> messageCol = result.IsSuccess
            ? result.Value
            : throw new Exception(result.Error);

        // Translate from column type to IMessage type
        List<IMessage> msgList = messageCol.Select(m => m.Convert<T, IMessage>()).ToList();

        // Remove duplicates
        List<IMessage> uniqueMsgs = RemoveDuplicates(msgList);

        return uniqueMsgs;
    }
    public List<ICallRecord> GetCallRecords(List<long> msgNums, string callRepo)
    {
        // Prepare the repo location
        FileInfo callLocation = callRepo == string.Empty || !File.Exists(callRepo)
            ? new(Location(_callRecordRepo))
            : new(callRepo);

        var result = JsonService.ReadFile<CallRecordJsonReader>(callLocation.FullName);
        List<CallRecordJsonReader> localCalls = result.IsSuccess
            ? result.Value
            : throw new Exception(result.Error);
        IEnumerable<ICallRecord> filteredCalls = localCalls
            .Select(c => c.Translate())
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
            FileInfo callLocation = callLoc == string.Empty || !File.Exists(callLoc) ? new(Location(_callRecordQuery)) : new(callLoc);
            string query = _rawQuery.MessageCallQuery(_startDate);
            try
            {
                Task<IEnumerable<CallDbEntity>> callTask =
                    callLoc == string.Empty
                    ? DwhContextHelpers.GetItemsFromRawAsync(callContext, query)
                    : DwhContextHelpers.GetItemsFromFileAsync(callContext, callLocation);
                List<ICallRecord> resultList = callTask.Result
                    .Select(c => (ICallDateTimeInUTC)c)
                    .Select(c => c.Translate())
                    .ToList();

                // Save results to local repo
                JsonService.WriteToFile(callLocRepo, resultList);
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
        var r = JsonService.ReadFile<CallRecordJson>(callLocRepo);
        List<CallRecordJson> localCalls = r.IsSuccess
            ? r.Value
            : throw new Exception(r.Error);
        IEnumerable<ICallRecord> result = localCalls.Select(c => c.Translate());
        return result.ToList();
    }

    public List<ICustomerSubscription> GetCustomerRecords(List<long> msgNums, string customerRepo)
    {
        // Prepare the repo location. This is the default location
        FileInfo customerLocation = customerRepo == string.Empty || !File.Exists(customerRepo)
            ? new(Location(_customerRecordRepo))
            : new(customerRepo);

        var result = JsonService.ReadFile<CustSubJsonReader>(customerLocation.FullName);
        List<CustSubJsonReader> localCustomers = result.IsSuccess
            ? result.Value
            : throw new Exception(result.Error);
        IEnumerable<ICustomerSubscription> filteredCustomers = localCustomers
            .Select(c => c.Translate())
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
                IEnumerable<CustSubDbEntity> customers = customerTask.Result;
                IEnumerable<ICustomerSubscription> records = customers.Select(c => c.Translate());
                List<ICustomerSubscription> resultList = records.ToList();

                // Save results to local repo
                JsonService.WriteToFile(customerLocRepo, resultList);
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
        var r = JsonService.ReadFile<CustSubJson>(customerLocRepo);
        List<CustSubJson> localCustomers = r.IsSuccess
            ? r.Value
            : throw new Exception(r.Error);
        IEnumerable<ICustomerSubscription> result = localCustomers.Select(c => c.Translate());
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
            var r = JsonService.ReadFile<CallRecordJson>(repoLocation);
            localCalls = r.IsSuccess
                ? r.Value
                : throw new Exception(); // The exception message is unused here, so no need to carry Result.Error through
        }
        catch
        {
            return true;
        }

        // If the most recent msg occurred before the most recent call AND the first msg occurred after the first call, set the local field to the list of call records
        CallRecordJson recentCall = FindMostRecent(localCalls);
        CallRecordJson firstCall = FindFirst(localCalls);
        if (DateTimeOffset.Compare(recentMsg.Date, recentCall.Date) < 0 && DateTimeOffset.Compare(firstMsg.Date - _rawQuery.NinetyDays, firstCall.Date) > 0)
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
            return localCalls.Select(m => m.Translate());
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
            var result = JsonService.ReadFile<CustSubJson>(repoLocation);
            custRepo = result.IsSuccess
                ? result.Value
                : throw new Exception(result.Error);
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
            IEnumerable<ICustomerSubscription> convertedCalls = custRepo.Select(m => m.Translate());
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
