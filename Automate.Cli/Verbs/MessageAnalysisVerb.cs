using CommandLine;
using Automate.Cli.Verbs.VerbHelper;
using Automate.Domain.SolutionFunctionality;
using CSharpFunctionalExtensions;
using Automate.Domain.ValueObjects;
using Automate.Application.MessageReportAnalysis;
using Automate.Application.MessageAnalysis;
using Automate.Infrastructure.DataRetrievalFormats;
using Microsoft.Extensions.DependencyInjection;

namespace Automate.Cli.Verbs;

[Verb(AnalyzeMessages, HelpText = "Analyze message leads. Specify the repository locations, report locations, whether a truncated report should be generated")]
internal class MessageAnalysisVerb : IVerb
{
    #region Options
    public const string AnalyzeMessages = "analyzeMessages";

    // Required Options
    [Option('s', "source", Required = true, HelpText = "Enter the csv file location of messages." + _fileDefault)]
    public required FileInfo MessageLocation { get; set; }
    [Option('o', "output", Required = false, HelpText = "Enter the location where you would like the report file to be output." + _fileDefault + "In any case, the program will print to screen the location where the report file is generated. Also, remember that you are providing the full file path, not a relative path.")]
    public required FileInfo ReportLocation { get; set; }
    [Option('c', "callRepo", Required = true, HelpText = "Enter the call repository file location that contains call records in local repo. This must be a Json. If one is not provided, or if the provided file does not exist, or the provided repo is not a Json file, the execution will fail.")]
    public required FileInfo CallRepoLocation { get; set; }
    [Option('q', "customerRepo", Required = true, HelpText = "Enter the customer repository file location that holds customer records in local repo. This must be a Json file. If one is not provided, or if the provided file does not exist, or the provided repo is not a Json file, the execution will fail.")]
    public required FileInfo CustomerRepoLocation { get; set; }
    [Option('a', "appendToReport", Default = false, HelpText = "This option allows the user to append the results of the analysis to the report, rather than generating an entirely new report.")]
    public bool Append { get; set; }
    [Option('t', "messageType", Required = true, HelpText = _helpText)]
    public MessageType MessageType { get; set; }

    // Not Required Options
    [Option('x', "truncate", Required = false, Default = false, HelpText = "This option is a boolean and will truncate the report. Default truncation is 120 days. You CANNOT truncate and append at the same time, so truncation will only work with the -appendToReport or -a switch off, otherwise, the report will not be truncated. The required companion option to this option is the output location of the truncated report.")]
    public bool Truncate { get; set; }
    [Option('d', "daysToTruncate", Required = false, Default = MessageAnalysisReportManager.DefaultDays, HelpText = "This option determines how many days to truncate the report. Default truncation is 120 days. This option will NOT truncate the report if the boolean 'x' option is undefined. You CANNOT truncate and append at the same time, so truncation will only work with the -appendToReport or -a switch off, otherwise, the report will not be truncated.")]
    public int DaysOfTruncation { get; set; } = MessageAnalysisReportManager.DefaultDays;
    [Option('O', "truncatedReportOutput", Required = false, HelpText = "This option is only needed if -x or -truncate is switched on. It is the output location of the truncated report.")]
    public string TruncatedReportLoc { get; set; } = string.Empty;
    #endregion

    #region Public (Besides Options)
    public int Run(IServiceProvider service)
    {
        // Inform the user what is going on
        var inform = service.GetRequiredService<IUserInformation>();
        string info = InformUser();
        inform.InformUser(info);
        FilePaths verified = VerifyInput();

        // Execute
        Result<FileInfo> result = Execute(Append, service, verified.MessageLoc, verified.CallRepoLoc, verified.CustomerRepoLoc, verified.ReportLoc, MessageType, verified.TruncatedRepoLoc, Truncate, DaysOfTruncation);

        // Logger
        StringLogger.NameLog(DateTime.Now, AnalyzeMessages, MessageType.ToString());

        int code = DetermineReturnCode(result, MessageLocation.Exists, CallRepoLocation.Exists, CustomerRepoLocation.Exists, ReportLocation.Exists, inform);
        Environment.ExitCode = code;
        return code;
    }
    #endregion

    #region Private
    private const string _fileDefault = " If a file is not provided or the provided location does not exist, a default will be used instead. ";
    private const string _helpText = "Choose which type of message file you wish to analyze. The following options are case-sensitive: " + MessageTypeText.Text;

    private string InformUser()
    {
        const string not = "(The given path does not exist)";

        List<string> resultList = [$"The user chose the following verb: {AnalyzeMessages}"];

        string messageLoc = MessageLocation.Exists
            ? MessageLocation.FullName
            : not;
        string reportLoc = ReportLocation.Exists
            ? ReportLocation.FullName
            : not;

        resultList.Add($"The user chose the following options.");
        resultList.Add($"- Location of Message file to analyze (required): \n\t\"{MessageLocation}\"\n\t- Literal path: \n\t{messageLoc}");
        resultList.Add($"- Location of the calls repository (required): \n\t{CallRepoLocation.FullName}");
        resultList.Add($"- Location of the customer repository (required): \n\t{CustomerRepoLocation.FullName}");
        resultList.Add($"- Location of report output (not required): \n\t\"{ReportLocation}\"\n\t- Literal path: \n\t{reportLoc}");
        resultList.Add($"- Whether or not to append to existing report (defaults to False): \n\t{Append}");
        resultList.Add($"- Whether or not to truncate the report (defaults to False): \n\t{Truncate}");
        if (Truncate) resultList.Add($"- The number of days to truncate the report (defaults to {MessageAnalysisReportManager.DefaultDays}): \n\t{DaysOfTruncation}");
        resultList.Add("");

        return string.Join('\n', resultList);
    }

    // Please keep this private. It doesn't need to appear anywhere except in this class
    private record FilePaths(FileInfo MessageLoc, FileInfo CallRepoLoc, FileInfo CustomerRepoLoc, string TruncatedRepoLoc, string ReportLoc);
    private FilePaths VerifyInput()
    {
        // Verify that the file inputs exist. If they don't, default them
        Result<FileType> msgLoc = PathManipulation.VerifyFileType(MessageLocation);
        FileInfo messageLocation = MessageLocation.Exists && msgLoc.IsSuccess && msgLoc.Value == FileType.Csv
            ? MessageLocation
            : throw new ArgumentException(fileInfoError(MessageLocation.FullName, msgLoc));

        // Call Location
        Result<FileType> callRepo = PathManipulation.VerifyFileType(CallRepoLocation);
        FileInfo callRepoLocation = CallRepoLocation.Exists && callRepo.IsSuccess && callRepo.Value == FileType.Json
            ? CallRepoLocation
            : throw new ArgumentException(fileInfoError(CallRepoLocation.FullName, callRepo));

        // Customer Location
        Result<FileType> customerRepo = PathManipulation.VerifyFileType(CustomerRepoLocation);
        FileInfo customerRepoLocation = CustomerRepoLocation.Exists && customerRepo.IsSuccess && customerRepo.Value == FileType.Json
            ? CustomerRepoLocation
            : throw new ArgumentException(fileInfoError(CustomerRepoLocation.FullName, customerRepo));

        // Truncated Report Loc
        Result<FileType> truncatedLoc = PathManipulation.VerifyFileType(TruncatedReportLoc);
        string truncatedReportLoc = Path.Exists(TruncatedReportLoc) && truncatedLoc.IsSuccess && truncatedLoc.Value == FileType.Csv
            ? Path.GetFullPath(TruncatedReportLoc)
            : string.Empty;
        if (Truncate && string.IsNullOrWhiteSpace(truncatedReportLoc))
            throw new ArgumentException($"The user requested to truncate the report but did not specify a valid truncated report output location. The truncated report output location must exist to continue.");

        // Report location
        Result<FileType> reportLoc = PathManipulation.VerifyFileType(ReportLocation);
        string reportLocation = ReportLocation.TryCreate(out string error) && reportLoc.IsSuccess && reportLoc.Value == FileType.Csv
            ? ReportLocation.FullName
            : Append
                ? throw new ArgumentException($"The user provided the following literal as the report location: {ReportLocation.FullName} -- That file location does not exist. This cannot be done when the option {nameof(Append)} is {Append} because no such file location exists. This resulted in the following error:\n {error}")
                : ReportLocation.FullName;

        return new(MessageLoc: messageLocation, CallRepoLoc: callRepoLocation, CustomerRepoLoc: customerRepoLocation, TruncatedRepoLoc: truncatedReportLoc, ReportLoc: reportLocation);

        #region Local
        static string fileInfoError(string location, Result<FileType> type)
        {
            const string c = "The provided repo location is not valid:";
            string message = type.IsFailure
                ? $"{c} {location}\nHere is one of the errors: {type.Error}"
                : $"{c} {location}";
            return message;
        }
        #endregion
    }

    private static Result<FileInfo> Execute(bool append, IServiceProvider service, FileInfo messageLocation, FileInfo callLocation, FileInfo customerLocation, string reportLocation, MessageType messageType, string truncateReport, bool truncate, int days)
    {
        string excMsg = $"There is no case where the input can be executed. Here is the input:\n{nameof(append)}: {append}\n{nameof(service)}: {service}\n{nameof(messageLocation)}: {messageLocation.FullName}\n{nameof(callLocation)}: {callLocation.FullName}\n{nameof(customerLocation)}: {customerLocation.FullName}\n{nameof(reportLocation)}: {reportLocation}\n{nameof(messageType)}: {messageType}\n{nameof(truncateReport)}: {truncateReport}\n{nameof(truncate)}: {truncate}\n{nameof(messageType)}: {messageType}\n{nameof(days)}: {days}";
        IMessageAnalysisManager generator = service.GetRequiredService<IMessageAnalysisManager>();
        IMessageAnalysisReportManager appender = service.GetRequiredService<IMessageAnalysisReportManager>();
        return (messageType, append, truncate) switch
        {
            // Pan
            (MessageType.Pan, true, true) => appender.Manage<SplitDateMountainOffsetMsgCol>(MessageType.Pan.ToString(), messageLocation, callLocation, customerLocation, reportLocation, truncateReport, truncate, messageType, days),
            (MessageType.Pan, true, false) => appender.Manage<SplitDateMountainOffsetMsgCol>(MessageType.Pan.ToString(), messageLocation, callLocation, customerLocation, reportLocation, messageType),
            (MessageType.Pan, false, true) => generator.Manage<SplitDateMountainOffsetMsgCol>(MessageType.Pan.ToString(), messageLocation, callLocation, customerLocation, reportLocation, truncate, messageType, days),
            (MessageType.Pan, false, false) => generator.Manage<SplitDateMountainOffsetMsgCol>(MessageType.Pan.ToString(), messageLocation, callLocation, customerLocation, reportLocation, messageType),

            // GAdsLeaf
            (MessageType.GAdsLeaf, true, true) => appender.Manage<UnifiedDateUnchangedOffset_SeparateGclid_SourceCantBeEmpty_MsgCol>(MessageType.GAdsLeaf.ToString(), messageLocation, callLocation, customerLocation, reportLocation, truncateReport, truncate, messageType, days),
            (MessageType.GAdsLeaf, true, false) => appender.Manage<UnifiedDateUnchangedOffset_SeparateGclid_SourceCantBeEmpty_MsgCol>(MessageType.GAdsLeaf.ToString(), messageLocation, callLocation, customerLocation, reportLocation, messageType),
            (MessageType.GAdsLeaf, false, true) => generator.Manage<UnifiedDateUnchangedOffset_SeparateGclid_SourceCantBeEmpty_MsgCol>(MessageType.GAdsLeaf.ToString(), messageLocation, callLocation, customerLocation, reportLocation, truncate, messageType, days),
            (MessageType.GAdsLeaf, false, false) => generator.Manage<UnifiedDateUnchangedOffset_SeparateGclid_SourceCantBeEmpty_MsgCol>(MessageType.GAdsLeaf.ToString(), messageLocation, callLocation, customerLocation, reportLocation, messageType),

            // GAdsLeafRepo
            (MessageType.GAdsLeafRepo, true, true) => appender.Manage<MessageClass>(MessageType.GAdsLeafRepo.ToString(), messageLocation, callLocation, customerLocation, reportLocation, truncateReport, truncate, messageType, days),
            (MessageType.GAdsLeafRepo, true, false) => appender.Manage<MessageClass>(MessageType.GAdsLeafRepo.ToString(), messageLocation, callLocation, customerLocation, reportLocation, messageType),
            (MessageType.GAdsLeafRepo, false, true) => generator.Manage<MessageClass>(MessageType.GAdsLeafRepo.ToString(), messageLocation, callLocation, customerLocation, reportLocation, truncate, messageType, days),
            (MessageType.GAdsLeafRepo, false, false) => generator.Manage<MessageClass>(MessageType.GAdsLeafRepo.ToString(), messageLocation, callLocation, customerLocation, reportLocation, messageType),

            // ManualWebForm
            (MessageType.ManualWebForm, true, true) => appender.Manage<NoTimeMsgCol>(MessageType.ManualWebForm.ToString(), messageLocation, callLocation, customerLocation, reportLocation, truncateReport, truncate, messageType, days),
            (MessageType.ManualWebForm, true, false) => appender.Manage<NoTimeMsgCol>(MessageType.ManualWebForm.ToString(), messageLocation, callLocation, customerLocation, reportLocation, messageType),
            (MessageType.ManualWebForm, false, true) => generator.Manage<NoTimeMsgCol>(MessageType.ManualWebForm.ToString(), messageLocation, callLocation, customerLocation, reportLocation, truncate, messageType, days),
            (MessageType.ManualWebForm, false, false) => generator.Manage<NoTimeMsgCol>(MessageType.ManualWebForm.ToString(), messageLocation, callLocation, customerLocation, reportLocation, messageType),

            // Leaf
            (MessageType.Leaf, true, true) => appender.Manage<UnifiedDateUnchangedOffset_SeparateGclid_MsgCol>(MessageType.Leaf.ToString(), messageLocation, callLocation, customerLocation, reportLocation, truncateReport, truncate, messageType, days),
            (MessageType.Leaf, true, false) => appender.Manage<UnifiedDateUnchangedOffset_SeparateGclid_MsgCol>(MessageType.Leaf.ToString(), messageLocation, callLocation, customerLocation, reportLocation, messageType),
            (MessageType.Leaf, false, true) => generator.Manage<UnifiedDateUnchangedOffset_SeparateGclid_MsgCol>(MessageType.Leaf.ToString(), messageLocation, callLocation, customerLocation, reportLocation, truncate, messageType, days),
            (MessageType.Leaf, false, false) => generator.Manage<UnifiedDateUnchangedOffset_SeparateGclid_MsgCol>(MessageType.Leaf.ToString(), messageLocation, callLocation, customerLocation, reportLocation, messageType),

            // LeafRepo
            (MessageType.LeafRepo, true, true) => appender.Manage<MessageClass>(MessageType.LeafRepo.ToString(), messageLocation, callLocation, customerLocation, reportLocation, truncateReport, truncate, messageType, days),
            (MessageType.LeafRepo, true, false) => appender.Manage<MessageClass>(MessageType.LeafRepo.ToString(), messageLocation, callLocation, customerLocation, reportLocation, messageType),
            (MessageType.LeafRepo, false, true) => generator.Manage<MessageClass>(MessageType.LeafRepo.ToString(), messageLocation, callLocation, customerLocation, reportLocation, truncate, messageType, days),
            (MessageType.LeafRepo, false, false) => generator.Manage<MessageClass>(MessageType.LeafRepo.ToString(), messageLocation, callLocation, customerLocation, reportLocation, messageType),

            // Meta
            (MessageType.MetaForm, true, true) => appender.Manage<UnifiedDateUtc_SplitPhone>(MessageType.MetaForm.ToString(), messageLocation, callLocation, customerLocation, reportLocation, truncateReport, truncate, messageType, days),
            (MessageType.MetaForm, true, false) => appender.Manage<UnifiedDateUtc_SplitPhone>(MessageType.MetaForm.ToString(), messageLocation, callLocation, customerLocation, reportLocation, messageType),
            (MessageType.MetaForm, false, true) => generator.Manage<UnifiedDateUtc_SplitPhone>(MessageType.MetaForm.ToString(), messageLocation, callLocation, customerLocation, reportLocation, truncate, messageType, days),
            (MessageType.MetaForm, false, false) => generator.Manage<UnifiedDateUtc_SplitPhone>(MessageType.MetaForm.ToString(), messageLocation, callLocation, customerLocation, reportLocation, messageType),

            // Libacion
            (MessageType.Libacion, true, true) => appender.Manage<SplitDateUTCOffsetMsgCol>(MessageType.Libacion.ToString(), messageLocation, callLocation, customerLocation, reportLocation, truncateReport, truncate, messageType, days),
            (MessageType.Libacion, true, false) => appender.Manage<SplitDateUTCOffsetMsgCol>(MessageType.Libacion.ToString(), messageLocation, callLocation, customerLocation, reportLocation, messageType),
            (MessageType.Libacion, false, true) => generator.Manage<SplitDateUTCOffsetMsgCol>(MessageType.Libacion.ToString(), messageLocation, callLocation, customerLocation, reportLocation, truncate, messageType, days),
            (MessageType.Libacion, false, false) => generator.Manage<SplitDateUTCOffsetMsgCol>(MessageType.Libacion.ToString(), messageLocation, callLocation, customerLocation, reportLocation, messageType),

            // Leased
            (MessageType.Leased, true, true) => appender.Manage<LeasedMessage>(MessageType.Leased.ToString(), messageLocation, callLocation, customerLocation, reportLocation, truncateReport, truncate, messageType, days),
            (MessageType.Leased, true, false) => appender.Manage<LeasedMessage>(MessageType.Leased.ToString(), messageLocation, callLocation, customerLocation, reportLocation, messageType),
            (MessageType.Leased, false, true) => generator.Manage<LeasedMessage>(MessageType.Leased.ToString(), messageLocation, callLocation, customerLocation, reportLocation, truncate, messageType, days),
            (MessageType.Leased, false, false) => generator.Manage<LeasedMessage>(MessageType.Leased.ToString(), messageLocation, callLocation, customerLocation, reportLocation, messageType),

            // Default
            _ => throw new Exception(excMsg)
        };
    }

    private static int DetermineReturnCode(Result<FileInfo> result, bool msgLocExists, bool callQExists, bool customerExists, bool reportExists, IUserInformation inform)
    {
        string message = result.IsSuccess
            ? $"The report creation was successful.\nHere is the report:\n{result.Value}"
            : $"There was a critical error. The report was not generated. Error:\n{result.Error}";
        inform.InformUser(message);

        return (result.IsSuccess, msgLocExists, callQExists, customerExists, reportExists) switch
        {
            (true, true, true, true, true) => ProgramErrorCodes.Success,
            (true, true, true, true, false) => ProgramErrorCodes.Message_ResultAndReportMissing,
            (true, true, true, false, true) => ProgramErrorCodes.Message_ResultAndCustomerMissing,
            (true, true, true, false, false) => ProgramErrorCodes.Message_ResultAndCustomerAndReportMissing,
            (true, true, false, true, true) => ProgramErrorCodes.Message_ResultAndCallMissing,
            (true, true, false, true, false) => ProgramErrorCodes.Message_ResultAndCallAndReportMissing,
            (true, true, false, false, true) => ProgramErrorCodes.Message_ResultAndCallAndCustomerMissing,
            (true, true, false, false, false) => ProgramErrorCodes.Message_ResultAndAllMissingButText,

            (true, false, true, true, true) => ProgramErrorCodes.Message_ResultAndMessageMissing,
            (true, false, true, true, false) => ProgramErrorCodes.Message_ResultAndMsgAndReportMissing,
            (true, false, true, false, true) => ProgramErrorCodes.Message_ResultAndMsgAndCustomerMissing,
            (true, false, true, false, false) => ProgramErrorCodes.Message_ResultAndAllMissingButCall,
            (true, false, false, true, true) => ProgramErrorCodes.Message_ResultAndMsgAndCallMissing,
            (true, false, false, true, false) => ProgramErrorCodes.Message_ResultAndAllMissingButCustomer,
            (true, false, false, false, true) => ProgramErrorCodes.Message_ResultAndAllMissingButReport,
            (true, false, false, false, false) => ProgramErrorCodes.Message_ResultAndAllMissing,

            (false, true, true, true, true) => ProgramErrorCodes.Message_NoResultAndNoneMissing,
            (false, true, true, true, false) => ProgramErrorCodes.Message_NoResultAndReportMissing,
            (false, true, true, false, true) => ProgramErrorCodes.Message_NoResultAndCustomerMissing,
            (false, true, true, false, false) => ProgramErrorCodes.Message_NoResultAndCustomerAndReportMissing,
            (false, true, false, true, true) => ProgramErrorCodes.Message_NoResultAndCallMissing,
            (false, true, false, true, false) => ProgramErrorCodes.Message_NoResultAndCallAndReportMissing,
            (false, true, false, false, true) => ProgramErrorCodes.Message_NoResultAndCallAndCustomerMissing,
            (false, true, false, false, false) => ProgramErrorCodes.Message_NoResultAndAllMissingButText,

            (false, false, true, true, true) => ProgramErrorCodes.Message_NoResultAndMessageMissing,
            (false, false, true, true, false) => ProgramErrorCodes.Message_NoResultAndMsgAndReportMissing,
            (false, false, true, false, true) => ProgramErrorCodes.Message_NoResultAndMsgAndCustomerMissing,
            (false, false, true, false, false) => ProgramErrorCodes.Message_NoResultAndAllMissingButCall,
            (false, false, false, true, true) => ProgramErrorCodes.Message_NoResultAndMsgAndCallMissing,
            (false, false, false, true, false) => ProgramErrorCodes.Message_NoResultAndAllMissingButCustomer,
            (false, false, false, false, true) => ProgramErrorCodes.Message_NoResultAndAllMissingButReport,
            (false, false, false, false, false) => ProgramErrorCodes.Message_NoResultAndAllMissing
        };
    }
    #endregion
}
