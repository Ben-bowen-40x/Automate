using Automate.Application.InfrastructureInterfaces;
using Automate.Application.UpdateContacts;
using Automate.Domain.SolutionFunctionality;
using Automate.Domain.ValueObjects;
using Automate.Infrastructure.ContactsUpdateService;
using Automate.Infrastructure.CsvService;

namespace Automate.Infrastructure.ReportingService;

internal class ReportServiceSingleton : IReportService
{
    private const string _errorMessage = "If you're reading this, then writing to the CSV failed.";
    private string? _folder;
    private string Folder => _folder ??= FolderFinder.GetLocalFolder(nameof(Infrastructure), @".info\Reports");

    public bool GenerateContactsReport(List<List<Contacts>> contacts, out DirectoryInfo directory, string reportDirectory = "")
    {
        // Validate input string
        string report = reportDirectory == string.Empty ? Folder + @"ContactUpdate\" : reportDirectory;
        directory = new(report);

        // Save contacts to file
        if (!directory.Exists)
        {
            Directory.CreateDirectory(directory.FullName);
        }
        try
        {
            int counter = UpdateContactsService.MagicNum;
            foreach (var contact in contacts)
            {
                string path = directory.FullName + $"ContactFile{counter++}.csv";
                File.WriteAllText(path, _errorMessage);
                CsvRW.WriteToCsv<Contacts, ContactsMap>(path, contact);
            }
        }
        catch { return false; }
        return true;
    }

    public bool GenerateDiscrepancyReport(List<DiscrepancyMatch> matches, out FileInfo file, string reportLoc = "")
    {
        var report =
            reportLoc == string.Empty || !reportLoc.Contains(".csv")
            ? Folder + $"DiscrepancyReport{DateTime.Now.ToString(DateTimeStrings.FileDateTimeFormat)}.csv"
            : reportLoc;
        file = new(report);
        if (!File.Exists(report))
        {
            File.WriteAllText(report, _errorMessage);
        }
        try
        {
            File.WriteAllText(report, _errorMessage);
            CsvRW.WriteToCsv<DiscrepancyMatch, DiscrepancyAnalysisMatchMap>(report, matches);
            return true;
        }
        catch { }
        return false;
    }
    public bool GenerateMessageLeadReport(List<QualifiedMessageRecord> messages, out FileInfo file, string reportLocation = "")
    {
        // Create a default report name in case one is not provided
        var report = $"MessageReport{DateTime.Now.ToString(DateTimeStrings.FileDateTimeFormat)}.csv";

        // Generate the file info that contains the file info for the report
        file = reportLocation == string.Empty ? new(Folder + report) : new(reportLocation);

        // Check whether the file exists. If not, create one
        if (!file.Exists)
            File.WriteAllText(file.FullName, _errorMessage);
        try
        {
            CsvRW.WriteToCsv<QualifiedMessageRecord, QualifiedMessageMap>(file.FullName, messages);
            return true;
        }
        catch { return false; }
    }

    public bool AppendMessageLeadReport(List<QualifiedMessageRecord> messages, out FileInfo file, string reportLocation)
    {
        // The report location must exist, but it MUST also have been checked by now. So, we would want to throw if we get to this point and the report does not exist
        file = new(reportLocation);

        // Attempt to APPEND the information to the file
        try
        {
            CsvRW.AppendToCsv<QualifiedMessageRecord, QualifiedMessageMap>(file.FullName, messages);
            return true;
        }
        catch
        { return false; }
    }
}
