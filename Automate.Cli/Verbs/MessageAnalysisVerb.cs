using CommandLine;
using Automate.Cli.Verbs.VerbHelper;
using Automate.Domain.SolutionFunctionality;
using CSharpFunctionalExtensions;

namespace Automate.Cli.Verbs;

[Verb(AnalyzeMessages, HelpText = "Initiate the analysis of message leads")]
internal class MessageAnalysisVerb : IVerb
{
    public const string AnalyzeMessages = "analyzeMessages";

    #region Options
    // Required Options
    [Option('s', "source", Required = true, HelpText = "Enter the csv file location of messages." + fileDefault)]
    public string MessageLocation { get; set; } = string.Empty;
    [Option('o', "output", Required = false, HelpText = "Enter the location where you would like the report file to be output." + fileDefault + "In any case, the program will print to screen the location where the report file is generated. Also, remember that you are providing the full file path, not a relative path.")]
    public string ReportLocation { get; set; } = string.Empty;
    [Option('t', "csvType", Required = true, HelpText = MessageVerbHelper.HelpText)]
    public MessageCsvType MessageType { get; set; }

    // Essentially required options
    [Option('a', "appendToReport", Default = false, HelpText = "This option allows the user to append the results of the analysis to the report, rather than generating an entirely new report.")]
    public bool Append { get; set; }

    // Not Required Calls
    [Option('c', "calls", Required = false, HelpText = "Enter the sql file location that retrieves call records." + fileDefault + queryWarning)]
    public string CallQueryLocation { get; set; } = string.Empty;
    [Option('C', "callRepo", Required = false, HelpText = "Enter the call repository file location that contains call records in local repo. This must be a Json. If one is not provided, or if the provided file does not exist, or the provided repo is not a Json file, a default will be used instead.")]
    public string CallRepoLocation { get; set; } = string.Empty;

    // Not Required Customers
    [Option('q', "customers", Required = false, HelpText = "Enter the sql file location that retrieves customer records." + fileDefault + queryWarning)]
    public string CustomerQueryLocation { get; set; } = string.Empty;
    [Option('Q', "customerRepo", Required = false, HelpText = "Enter the customer repository file location that holds customer records in local repo. This must be a Json file. If one is not provided, or if the provided file does not exist, or the provided repo is not a Json file, a default will be used instead.")]
    public string CustomerRepoLocation { get; set; } = string.Empty;
    #endregion

    #region Public (Besides Options)
    public int Run(IServiceProvider service)
    {
        // Inform the user what is going on
        InformUser();
        VerifyInput(out string messageLocation, out string callQueryLocation, out string callRepoLocation, out string customerQueryLocation, out string customerRepoLocation, out string reportLocation);

        // Execute
        Result<FileInfo> result = MessageVerbHelper.Execute(Append, service, messageLocation, callQueryLocation, customerQueryLocation, reportLocation, MessageType);

        // Logger
        StringLogger.NameLog(DateTime.Now, AnalyzeMessages, MessageType.ToString());

        return DetermineReturnCode(result, MessageLocation, CallQueryLocation, CustomerQueryLocation, ReportLocation);
    }

    private void VerifyInput(out string messageLocation, out string callQueryLocation, out string callRepoLocation, out string customerQueryLocation, out string customerRepoLocation, out string reportLocation)
    {
        // Verify that the file inputs exist. If they don't, default them
        Result<FileType> msgLoc = PathManipulation.VerifyType(MessageLocation);
        messageLocation = Path.Exists(MessageLocation) && msgLoc.IsSuccess && msgLoc.Value == FileType.Csv
            ? Path.GetFullPath(MessageLocation)
            : string.Empty;

        // Call Locations
        Result<FileType> callLoc = PathManipulation.VerifyType(CallQueryLocation);
        callQueryLocation = Path.Exists(CallQueryLocation) && callLoc.IsSuccess && callLoc.Value == FileType.Sql
            ? Path.GetFullPath(CallQueryLocation)
            : string.Empty;
        Result<FileType> callRepo = PathManipulation.VerifyType(CallRepoLocation);
        callRepoLocation = Path.Exists(CallRepoLocation) && callRepo.IsSuccess && callRepo.Value == FileType.Json
            ? Path.GetFullPath(CallRepoLocation)
            : string.Empty;

        // Customer Locations
        Result<FileType> customerLoc = PathManipulation.VerifyType(CustomerQueryLocation);
        customerQueryLocation = Path.Exists(CustomerQueryLocation) && customerLoc.IsSuccess && customerLoc.Value == FileType.Sql
            ? Path.GetFullPath(CustomerQueryLocation)
            : string.Empty;
        Result<FileType> customerRepo = PathManipulation.VerifyType(CustomerRepoLocation);
        customerRepoLocation = Path.Exists(CustomerRepoLocation) && customerRepo.IsSuccess && customerRepo.Value == FileType.Json
            ? Path.GetFullPath(CustomerRepoLocation)
            : string.Empty;

        // Report location
        Result<FileType> reportLoc = PathManipulation.VerifyType(ReportLocation);
        reportLocation = ReportLocation.TryCreate(out string error) && reportLoc.IsSuccess && reportLoc.Value == FileType.Csv
            ? Path.GetFullPath(ReportLocation)
            : Append
                ? throw new ArgumentException($"The user provided the following literal as the report location: {ReportLocation} -- That file location does not exist. This cannot be done when the option {nameof(Append)} is {Append} because no such file location exists. This resulted in the following error:\n {error}")
                : ReportLocation;
    }

    #endregion

    #region Private
    
    private const string fileDefault = " If a file is not provided or the provided location does not exist, a default will be used instead. ";
    private const string queryWarning = " Keep in mind that the query must be properly formulated in order for the program to receive the query. ";

    private void InformUser()
    {
        const string not = "(The given path does not exist)";
        string messageLoc = Path.Exists(MessageLocation)
            ? Path.GetFullPath(MessageLocation)
            : not;
        string reportLoc = Path.Exists(ReportLocation)
            ? Path.GetFullPath(ReportLocation)
            : not;
        string callQueryLoc = Path.Exists(CallQueryLocation)
            ? Path.GetFullPath(CallQueryLocation)
            : not;
        string customerQueryLoc = Path.Exists(CustomerQueryLocation)
            ? Path.GetFullPath(CustomerQueryLocation)
            : not;
        Console.WriteLine($"The user chose the following verb: {AnalyzeMessages}");
        Console.WriteLine($"The user chose the following options.");
        Console.WriteLine($"- Location of Message file to analyze (required): \n\t{MessageLocation}\n\t- Literal path: \n\t{messageLoc}");
        Console.WriteLine($"- Location of report output (not required): \n\t\"{ReportLocation}\"\n\t- Literal path: \n\t{reportLoc}");
        Console.WriteLine($"- Whether or not to append to existing report (defaults to False): \n\t{Append}");
        if (CallQueryLocation is not null && CallQueryLocation != string.Empty && CallQueryLocation.Length > 0)
            Console.WriteLine($"- Location of the sql query file that will be used to retrieve call records: \n\t{CallQueryLocation}\n\t- Literal path: \n\t{callQueryLoc}");
        if (CustomerQueryLocation is not null && CustomerQueryLocation != string.Empty && CustomerQueryLocation.Length > 0)
            Console.WriteLine($"- Location of the sql query file that will be used to retrieve call records: \n\t{CustomerQueryLocation}\n\t- Literal path: \n\t{customerQueryLoc}");
        Console.WriteLine("");
    }

    private static int DetermineReturnCode(Result<FileInfo> result, string msgLoc, string callQuery, string customerQuery, string reportLoc)
    {
        if (result.IsSuccess)
        {
            Console.WriteLine("The report creation was successful.");
            Console.WriteLine("Here is the report:");
            Console.WriteLine(result.Value);
        }
        else
        {
            Console.WriteLine("There was a critical error. The report could not be generated.");
            return ProgramErrorCodes.Message_CriticalFailure;
        }

        return DetermineReturnCode(result.IsSuccess, msgLoc, callQuery, customerQuery, reportLoc);
    }

    private static int DetermineReturnCode(bool result, string msgLoc, string callQuery, string customerQuery, string reportLoc)
    {
        bool msgLocExists = File.Exists(msgLoc);
        bool callQExists = File.Exists(callQuery);
        bool customerExists = File.Exists(customerQuery);
        bool reportExists = File.Exists(reportLoc);
        if (result)
        {
            if (msgLocExists && callQExists && customerExists && reportExists)
                return ProgramErrorCodes.Success;
            else if (!msgLocExists && callQExists && customerExists && reportExists)
                return ProgramErrorCodes.Message_ResultAndMessageMissing;
            else if (msgLocExists && !callQExists && customerExists && reportExists)
                return ProgramErrorCodes.Message_ResultAndCallMissing;
            else if (msgLocExists && callQExists && !customerExists && reportExists)
                return ProgramErrorCodes.Message_ResultAndCustomerMissing;
            else if (msgLocExists && callQExists && customerExists && !reportExists)
                return ProgramErrorCodes.Message_ResultAndReportMissing;
            else if (!msgLocExists && !callQExists && customerExists && reportExists)
                return ProgramErrorCodes.Message_ResultAndMsgAndCallMissing;
            else if (!msgLocExists && callQExists && !customerExists && reportExists)
                return ProgramErrorCodes.Message_ResultAndMsgAndCustomerMissing;
            else if (!msgLocExists && callQExists && customerExists && !reportExists)
                return ProgramErrorCodes.Message_ResultAndMsgAndReportMissing;
            else if (msgLocExists && !callQExists && !customerExists && reportExists)
                return ProgramErrorCodes.Message_ResultAndCallAndCustomerMissing;
            else if (msgLocExists && !callQExists && customerExists && !reportExists)
                return ProgramErrorCodes.Message_ResultAndCallAndReportMissing;
            else if (msgLocExists && callQExists && !customerExists && !reportExists)
                return ProgramErrorCodes.Message_ResultAndCustomerAndReportMissing;
            else if (msgLocExists && !callQExists && !customerExists && !reportExists)
                return ProgramErrorCodes.Message_ResultAndAllMissingButText;
            else if (!msgLocExists && callQExists && !customerExists && !reportExists)
                return ProgramErrorCodes.Message_ResultAndAllMissingButCall;
            else if (!msgLocExists && !callQExists && customerExists && !reportExists)
                return ProgramErrorCodes.Message_ResultAndAllMissingButCustomer;
            else if (!msgLocExists && !callQExists && !customerExists && reportExists)
                return ProgramErrorCodes.Message_ResultAndAllMissingButReport;
            else if (!msgLocExists && !callQExists && !customerExists && !reportExists)
                return ProgramErrorCodes.Message_ResultAndAllMissing;
            else
                return ProgramErrorCodes.Message_ResultAndUnknown;
        }
        else
        {
            if (msgLocExists && callQExists && customerExists && reportExists)
                return ProgramErrorCodes.Message_NoResultAndNoneMissing;
            else if (!msgLocExists && callQExists && customerExists && reportExists)
                return ProgramErrorCodes.Message_NoResultAndMessageMissing;
            else if (msgLocExists && !callQExists && customerExists && reportExists)
                return ProgramErrorCodes.Message_NoResultAndCallMissing;
            else if (msgLocExists && callQExists && !customerExists && reportExists)
                return ProgramErrorCodes.Message_NoResultAndCustomerMissing;
            else if (msgLocExists && callQExists && customerExists && !reportExists)
                return ProgramErrorCodes.Message_NoResultAndReportMissing;
            else if (!msgLocExists && !callQExists && customerExists && reportExists)
                return ProgramErrorCodes.Message_NoResultAndMsgAndCallMissing;
            else if (!msgLocExists && callQExists && !customerExists && reportExists)
                return ProgramErrorCodes.Message_NoResultAndMsgAndCustomerMissing;
            else if (!msgLocExists && callQExists && customerExists && !reportExists)
                return ProgramErrorCodes.Message_NoResultAndMsgAndReportMissing;
            else if (msgLocExists && !callQExists && !customerExists && reportExists)
                return ProgramErrorCodes.Message_NoResultAndCallAndCustomerMissing;
            else if (msgLocExists && !callQExists && customerExists && !reportExists)
                return ProgramErrorCodes.Message_NoResultAndCallAndReportMissing;
            else if (msgLocExists && callQExists && !customerExists && !reportExists)
                return ProgramErrorCodes.Message_NoResultAndCustomerAndReportMissing;
            else if (msgLocExists && !callQExists && !customerExists && !reportExists)
                return ProgramErrorCodes.Message_NoResultAndAllMissingButText;
            else if (!msgLocExists && callQExists && !customerExists && !reportExists)
                return ProgramErrorCodes.Message_NoResultAndAllMissingButCall;
            else if (!msgLocExists && !callQExists && customerExists && !reportExists)
                return ProgramErrorCodes.Message_NoResultAndAllMissingButCustomer;
            else if (!msgLocExists && !callQExists && !customerExists && reportExists)
                return ProgramErrorCodes.Message_NoResultAndAllMissingButReport;
            else if (!msgLocExists && !callQExists && !customerExists && !reportExists)
                return ProgramErrorCodes.Message_NoResultAndAllMissing;
            else
                return ProgramErrorCodes.Message_NoResultAndUnknown;
        }
    }
    #endregion
}
