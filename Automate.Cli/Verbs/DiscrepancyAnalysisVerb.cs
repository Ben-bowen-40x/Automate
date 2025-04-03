using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Automate.Application.Discrepancy;
using Automate.Cli.Verbs.VerbHelper;
using Automate.Domain.SolutionFunctionality;
using CSharpFunctionalExtensions;

namespace Automate.Cli.Verbs;

[Verb(AnalyzeDiscrepancy, HelpText = "This executes the Discrepancy Analysis using a csv file as the source of billable calls and a sql query in the form of a locally saved file as the source for comparison calls. Options include the default csv location (not recommended) and the default sql location (recommended).")]
internal class DiscrepancyAnalysisVerb : IVerb
{
    private const string AnalyzeDiscrepancy = "analyzeDiscrepancy";

    #region Options
    [Option('s', "source", Required = true, HelpText = "The file name that contains the list of billed calls from the source. The list must be a csv files with known headers.The list is compared against internal company data for discrepancies. Discrepancies are calls that were billed by the source but are not a lead, based on company data.")]
    public string BillableCallCsvLoc { get; set; } = string.Empty;
    [Option('o', "output", Required = true, HelpText = "The fully qualified file location where you want the report file to be located. If none is provided, a default will be used, located in a hidden folder in the Infrastructure Layer file system. Additionally, the parent folder of the report must exist, otherwise the default report name and location will be used.")]
    public string ReportLocation { get; set; } = string.Empty;
    [Option('q', "query", Required = false, HelpText = "The fully qualified file location where the sql query file that will be used to retrieve the comparison files is located. If none is provided, a default will be used. Be careful about the query used, because the proper framework to receive the query results may or may not be available in the program.")]
    public string ComparisonCallQueryLoc { get; set; } = string.Empty;
    #endregion

    public int Run(IServiceProvider service)
    {
        string parent = PathManipulation.RetrieveParentDir(ReportLocation);

        // Verify command line input
        string fileName = !File.Exists(BillableCallCsvLoc)
            ? string.Empty
            : BillableCallCsvLoc;
        string report = !Directory.Exists(parent)
            ? string.Empty
            : ReportLocation;
        string comparisonLoc = !File.Exists(ComparisonCallQueryLoc)
            ? string.Empty
            : ComparisonCallQueryLoc;
        File.WriteAllText(report, "");

        // Inform user of the input
        var inform = service.GetRequiredService<IUserInformation>();
        string fileNameMsg = $"For the following option, \"{nameof(BillableCallCsvLoc)}\" -- {PathManipulation.LocationInformation(fileName)}";
        string reportMsg = $"\nFor the following option, \"{nameof(ReportLocation)}\" -- {PathManipulation.LocationInformation(report)}";
        string comparisonLocMsg = $"\nFor the following option, \"{nameof(ComparisonCallQueryLoc)}\" -- {PathManipulation.LocationInformation(comparisonLoc)}";
        inform.InformUser(fileNameMsg, reportMsg, comparisonLocMsg);

        // Prepare the result
        var callManager = service.GetRequiredService<IDiscrepancyManager>();
        Result<FileInfo> result = callManager.ManageDiscrepancyAnalysis(fileName, report, comparisonLoc);

        // Name log 
        StringLogger.NameLog(DateTime.Now, AnalyzeDiscrepancy);

        // Return code 
        int code = DetermineReturnCode(fileName, report, comparisonLoc, result, inform);
        Environment.ExitCode = code;
        return code;
    }

    #region Private Members
    static int DetermineReturnCode(string fileName, string report, string query, Result<FileInfo> result, IUserInformation inform)
    {
        string message = result.IsSuccess
            ? $"Generated Report. Report Location:\n{result.Value.FullName}"
            : new Func<string>(() =>
                {
                    StringLogger.AddLog(GetFullName.GetMemberName(new DiscrepancyAnalysisVerb(), nameof(DetermineReturnCode)), "Report failed to generate.");
                    return "Failed to generate report.";
                })();
        inform.InformUser(message);
        return (string.IsNullOrWhiteSpace(fileName), string.IsNullOrWhiteSpace(report), string.IsNullOrWhiteSpace(query), result.IsSuccess) switch
        {
            (true, true, true, true) => ProgramErrorCodes.Analyze_GeneratedReport_AllFilesDefaulted,
            (true, true, true, false) => ProgramErrorCodes.Analyze_CriticalFailure,
            (true, true, false, true) => ProgramErrorCodes.Analyze_GeneratedReport_FileAndReportDefaulted,
            (true, true, false, false) => ProgramErrorCodes.Analyze_FailedReport_FileAndReportDefaulted,
            (true, false, true, true) => ProgramErrorCodes.Analyze_GeneratedReport_FileAndQueryDefaulted,
            (true, false, true, false) => ProgramErrorCodes.Analyze_FailedReport_FileAndQueryDefaulted,
            (true, false, false, true) => ProgramErrorCodes.Analyze_GeneratedReport_BillableFileDefaulted,
            (true, false, false, false) => ProgramErrorCodes.Analyze_FailedReport_BillableFileDefaulted,

            (false, true, true, true) => ProgramErrorCodes.Analyze_GeneratedReport_ReportAndQueryDefaulted,
            (false, true, true, false) => ProgramErrorCodes.Analyze_FailedReport_ReportAndQueryDefaulted,
            (false, true, false, true) => ProgramErrorCodes.Analyze_GeneratedReport_ReportLocDefaulted,
            (false, true, false, false) => ProgramErrorCodes.Analyze_FailedReport_ReportLocDefaulted,
            (false, false, true, true) => ProgramErrorCodes.Analyze_GeneratedReport_QueryDefaulted,
            (false, false, true, false) => ProgramErrorCodes.Analyze_FailedReport_QueryDefaulted,
            _ => ProgramErrorCodes.Success
        };
    }
    #endregion
}
