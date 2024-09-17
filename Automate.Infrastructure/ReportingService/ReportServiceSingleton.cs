using Automate.Application.InfrastructureInterfaces;
using Automate.Application.UpdateContacts;
using Automate.Domain.SolutionFunctionality;
using Automate.Domain.ValueObjects;
using Automate.Infrastructure.ContactsUpdateService;
using Automate.Infrastructure.CsvService;
using Automate.Infrastructure.LeafClientService;
using Automate.Infrastructure.MessageLeadsReportService;
using Automate.Infrastructure.MessageLeadsService.CsvMaps;
using CSharpFunctionalExtensions;

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
    public Result<DirectoryInfo> GenerateContactsReport(List<List<Contacts>> contacts, string reportDirectory = "")
    {
        // Validate input string
        string report = reportDirectory == string.Empty ? Folder + @"ContactUpdate\" : reportDirectory;
        DirectoryInfo directory = new(report);

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
            return directory;
        }
        catch (Exception ex)
        {
            return Result.Failure<DirectoryInfo>(ex.Message);
        }
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
        catch
        {
            return false;
        }
    }
    public Result<FileInfo> GenerateDiscrepancyReport(List<DiscrepancyMatch> matches, string reportLoc = "")
    {
        var report =
            reportLoc == string.Empty || !reportLoc.Contains(".csv")
            ? Folder + $"DiscrepancyReport{DateTime.Now.ToString(DateTimeStrings.FileDateTimeFormat)}.csv"
            : reportLoc;
        FileInfo file = new(report);
        if (!File.Exists(report))
        {
            File.WriteAllText(report, _errorMessage);
        }
        try
        {
            File.WriteAllText(report, _errorMessage);
            CsvRW.WriteToCsv<DiscrepancyMatch, DiscrepancyAnalysisMatchMap>(report, matches);
            return file;
        }
        catch (Exception ex)
        {
            return Result.Failure<FileInfo>(ex.Message);
        }
    }

    private string? _msgReportDefault;
    private string MsgReportDefault(string name) => _msgReportDefault ??= $"{name}{DateTime.Now.ToString(DateTimeStrings.FileDateTimeFormat)}.csv";
    public bool GenerateMessageLeadReport(string reportDefault, List<QualifiedMessageRecord> messages, out FileInfo file, string reportLocation = "")
    {
        // Create a default report name in case one is not provided
        var report = MsgReportDefault(reportDefault);

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
        catch
        {
            return false;
        }
    }
    public Result<FileInfo> GenerateMessageLeadReport(string reportDefault, List<QualifiedMessageRecord> messages, string reportLocation = "")
    {
        // Create a default report name in case one is not provided
        var report = MsgReportDefault(reportDefault);

        // Generate the file info that contains the file info for the report
        FileInfo file = reportLocation == string.Empty ? new(Folder + report) : new(reportLocation);

        // Check whether the file exists. If not, create one
        if (!file.Exists)
            File.WriteAllText(file.FullName, _errorMessage);
        try
        {
            CsvRW.WriteToCsv<QualifiedMessageRecord, QualifiedMessageMap>(file.FullName, messages);
            return file;
        }
        catch
        {
            return Result.Failure<FileInfo>("Something went wrong with writing to the CSV.");
        }
    }

    public bool GenerateMessageLeadReportAppend(string reportDefault, List<QualifiedMessageRecord> messages, out FileInfo file, string reportLocation = "")
    {
        // Create a default report name in case one is not provided
        var report = MsgReportDefault(reportDefault);

        // Generate the file info that contains the file info for the report
        file = reportLocation == string.Empty ? new(Folder + report) : new(reportLocation);

        // Check whether the file exists. If not, create one
        if (!file.Exists)
            File.WriteAllText(file.FullName, _errorMessage);
        try
        {
            CsvRW.WriteToCsv<QualifiedMessageRecord, MessageReportMap>(file.FullName, messages);
            return true;
        }
        catch { return false; }
    }
    public Result<FileInfo> GenerateMessageLeadReportAppend(string reportDefault, List<QualifiedMessageRecord> messages, string reportLocation = "")
    {
        // Create a default report name in case one is not provided
        var report = MsgReportDefault(reportDefault);

        // Generate the file info that contains the file info for the report
        FileInfo file = reportLocation == string.Empty ? new(Folder + report) : new(reportLocation);

        // Check whether the file exists. If not, create one
        if (!file.Exists)
            File.WriteAllText(file.FullName, _errorMessage);
        try
        {
            CsvRW.WriteToCsv<QualifiedMessageRecord, MessageReportMap>(file.FullName, messages);
            return file;
        }
        catch (Exception ex)
        {
            return Result.Failure<FileInfo>(ex.Message);
        }
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
    public Result<FileInfo> AppendMessageLeadReport(List<QualifiedMessageRecord> messages, string reportLocation)
    {
        // The report location must exist, but it MUST also have been checked by now. So, we would want to throw if we get to this point and the report does not exist
        FileInfo file = new(reportLocation);

        // Attempt to APPEND the information to the file
        try
        {
            CsvRW.AppendToCsv<QualifiedMessageRecord, QualifiedMessageMap>(file.FullName, messages);
            return file;
        }
        catch (Exception ex)
        {
            return Result.Failure<FileInfo>(ex.Message);
        }
    }

    public bool GenerateLeafMessages(List<IMessage> msgs, out FileInfo file, string location)
    {
        // Check for default
        var loc = location == string.Empty
            ? LeafApiService.MessageRepoLocation
            : location;
        file = new(loc);

        // Attempt to write the information to file
        try
        {
            CsvRW.WriteToCsv<IMessage, MessageMapRW>(loc, msgs);
            return true;
        }
        catch { return false; }
    }
    public Result<FileInfo> GenerateLeafMessages(List<IMessage> msgs, string location)
    {
        // Check for default
        var loc = location == string.Empty
            ? LeafApiService.MessageRepoLocation
            : location;
        FileInfo file = new(loc);

        // Attempt to write the information to file
        try
        {
            CsvRW.WriteToCsv<IMessage, MessageMapRW>(loc, msgs);
            return file;
        }
        catch (Exception ex)
        {
            return Result.Failure<FileInfo>(ex.Message);
        }
    }
}
