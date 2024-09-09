using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Automate.Application.Discrepancy;
using Automate.Cli.Verbs.VerbHelper;
using Automate.Domain.SolutionFunctionality;

namespace Automate.Cli.Verbs;

[Verb(AnalyzeDiscrepancy, HelpText = "This executes the Discrepancy Analysis using a csv file as the source of billable calls and a sql query in the form of a locally saved file as the source for comparison calls. Options include the default csv location (not recommended) and the default sql location (recommended).")]
internal class DiscrepancyAnalysisVerb : IVerb
{
    private const string AnalyzeDiscrepancy = "analyzeDiscrepancy";

    #region Options
    [Option('s', "source", Required = true, HelpText = "The file name that contains the list of billed calls from the source. The list must be a csv files with known headers.The list is compared against internal company data for discrepancies. Discrepancies are calls that were billed by the source but are not a lead, based on company data.")]
    public string BillableCallCsvLoc { get; set; } = string.Empty;
    [Option('r', "report", Required = true, HelpText = "The fully qualified file location where you want the report file to be located. If none is provided, a default will be used, located in a hidden folder in the Infrastructure Layer file system. Additionally, the parent folder of the report must exist, otherwise the default report name and location will be used.")]
    public string ReportLocation { get; set; } = string.Empty;
    [Option('q', "query", Required = false, HelpText = "The fully qualified file location where the sql query file that will be used to retrieve the comparison files is located. If none is provided, a default will be used. Be careful about the query used, because the proper framework to receive the query results may or may not be available in the program.")]
    public string ComparisonCallQueryLoc { get; set; } = string.Empty;
    #endregion

    public int Run(IServiceProvider service)
    {
        string parent = DirectoryManipulation.RetrieveParentDir(ReportLocation);

        // Verify command line input
        VerifyUserInput(parent, out string fileName, out string report, out string query);

        // Inform user of the input
        InformUser(fileName, report, query);

        // Prepare the result
        var callManager = service.GetRequiredService<IDiscrepancyManager>();
        Dictionary<bool, FileInfo> result = callManager.ManageDiscrepancyAnalysis(fileName, report, query);

        // Name log 
        StringLogger.NameLog(DateTime.Now, AnalyzeDiscrepancy);

        // Return code 
        return DetermineReturnCode(fileName, report, query, result);
    }

    #region Private Members
    private void InformUser(string fileName, string report, string query)
    {
        Console.WriteLine($"For the following option, \"{nameof(BillableCallCsvLoc)}\" -- {DirectoryManipulation.LocationInformation(fileName)}");
        Console.WriteLine($"\nFor the following option, \"{nameof(ReportLocation)}\" -- {DirectoryManipulation.LocationInformation(report)}");
        Console.WriteLine($"\nFor the following option, \"{nameof(ComparisonCallQueryLoc)}\" -- {DirectoryManipulation.LocationInformation(query)}");
    }

    private void VerifyUserInput(string parent, out string fileName, out string report, out string query)
    {
        fileName = !File.Exists(BillableCallCsvLoc)
                    ? string.Empty
                    : BillableCallCsvLoc;
        report = !Directory.Exists(parent)
                    ? string.Empty
                    : ReportLocation;
        query = !File.Exists(ComparisonCallQueryLoc)
                    ? string.Empty
                    : ComparisonCallQueryLoc;
        File.WriteAllText(report, "");
    }

    static int DetermineReturnCode(string fileName, string report, string query, Dictionary<bool, FileInfo> result)
    {
        bool resultBool = result.Keys.ToList()[0];
        if (resultBool)
        {
            System.Console.WriteLine($"Generated report. Report Location:");
            System.Console.WriteLine(result[true].FullName);
            return ReturnCode(fileName, report, query, resultBool);
        }
        else
        {
            System.Console.WriteLine("Failed to generate report.");
            StringLogger.AddLog(GetFullName.GetMemberName(new DiscrepancyAnalysisVerb(), nameof(DetermineReturnCode)), "Report failed to generate.");
            return ReturnCode(fileName, report, query, resultBool);
        }
    }

    static int ReturnCode(string fileName, string report, string query, bool resultSuccess)
    {
        if (fileName == string.Empty && report != string.Empty && query != string.Empty && resultSuccess)
            return ProgramErrorCodes.Analyze_GeneratedReport_BillableFileDefaulted;
        else if (fileName != string.Empty && report == string.Empty && query != string.Empty && resultSuccess)
            return ProgramErrorCodes.Analyze_GeneratedReport_ReportLocDefaulted;
        else if (fileName != string.Empty && report != string.Empty && query == string.Empty && resultSuccess)
            return ProgramErrorCodes.Analyze_GeneratedReport_QueryDefaulted;
        else if (fileName == string.Empty && report == string.Empty && query != string.Empty && resultSuccess)
            return ProgramErrorCodes.Analyze_GeneratedReport_FileAndReportDefaulted;
        else if (fileName != string.Empty && report == string.Empty && query == string.Empty && resultSuccess)
            return ProgramErrorCodes.Analyze_GeneratedReport_ReportAndQueryDefaulted;
        else if (fileName == string.Empty && report != string.Empty && query == string.Empty && resultSuccess)
            return ProgramErrorCodes.Analyze_GeneratedReport_FileAndQueryDefaulted;
        else if (fileName == string.Empty && report == string.Empty && query == string.Empty && resultSuccess)
            return ProgramErrorCodes.Analyze_GeneratedReport_AllFilesDefaulted;

        else if (fileName == string.Empty && report != string.Empty && query != string.Empty && !resultSuccess)
            return ProgramErrorCodes.Analyze_FailedReport_BillableFileDefaulted;
        else if (fileName != string.Empty && report == string.Empty && query != string.Empty && !resultSuccess)
            return ProgramErrorCodes.Analyze_FailedReport_ReportLocDefaulted;
        else if (fileName != string.Empty && report != string.Empty && query == string.Empty && !resultSuccess)
            return ProgramErrorCodes.Analyze_FailedReport_QueryDefaulted;
        else if (fileName == string.Empty && report == string.Empty && query != string.Empty && !resultSuccess)
            return ProgramErrorCodes.Analyze_FailedReport_FileAndReportDefaulted;
        else if (fileName != string.Empty && report == string.Empty && query == string.Empty && !resultSuccess)
            return ProgramErrorCodes.Analyze_FailedReport_ReportAndQueryDefaulted;
        else if (fileName == string.Empty && report != string.Empty && query == string.Empty && !resultSuccess)
            return ProgramErrorCodes.Analyze_FailedReport_FileAndQueryDefaulted;
        else if (fileName == string.Empty && report == string.Empty && query == string.Empty && !resultSuccess)
            return ProgramErrorCodes.Analyze_CriticalFailure;

        return ProgramErrorCodes.Success;
    }
    #endregion
}
