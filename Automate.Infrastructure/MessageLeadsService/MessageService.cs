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
    private DirectoryInfo? loc;
    public DirectoryInfo Loc => loc ??= FolderFinder.GetLocalFolder(nameof(Infrastructure), _fileLoc);
    private FileInfo Location(string file) => new(Loc + file);

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
        FileInfo msgLocStr = msgs == string.Empty ? Location(_messagesLocation) : new(msgs);
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
            ? Location(_callRecordRepo)
            : new(callRepo);

        Result<List<CallRecordJsonReader>> result = JsonService.ReadFile<CallRecordJsonReader>(callLocation.FullName);
        List<CallRecordJsonReader> localCalls = result.IsSuccess
            ? result.Value
            : throw new Exception(result.Error);
        List<ICallRecord> filteredCalls = localCalls
            .Select(c => c.Translate())
            .Where(c => msgNums.Contains(c.Number.Number))
            .ToList();
        return filteredCalls;
    }

    public List<ICustomerSubscription> GetCustomerRecords(List<long> msgNums, string customerRepo)
    {
        // Prepare the repo location. This is the default location
        FileInfo customerLocation = customerRepo == string.Empty || !File.Exists(customerRepo)
            ? Location(_customerRecordRepo)
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
    #endregion

    #region Private Members
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
