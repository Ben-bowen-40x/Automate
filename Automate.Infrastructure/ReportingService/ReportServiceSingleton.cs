using Automate.Application.InfrastructureInterfaces;
using Automate.Domain.SolutionFunctionality;
using Automate.Domain.ValueObjects;
using Automate.Infrastructure.ContactsUpdateService;
using Automate.Infrastructure.CsvManipulationService;
using Automate.Infrastructure.DataRetrievalFormats;
using Automate.Infrastructure.LeafClientService;
using CSharpFunctionalExtensions;

namespace Automate.Infrastructure.ReportingService;

internal class ReportServiceSingleton : IReportService
{
    private const string _errorMessage = "If you're reading this, then writing to the CSV failed.";
    private DirectoryInfo? _folder;
    private DirectoryInfo Folder => _folder ??= FolderFinder.GetLocalFolder(nameof(Infrastructure), @".info\Reports");

    public Result<DirectoryInfo> GenerateContactsReport(List<List<Contact>> contacts, string reportDirectory = "")
    {
        // Validate input string
        DirectoryInfo directory = string.IsNullOrWhiteSpace(reportDirectory)
            ? new(Folder + @"ContactUpdate\")
            : new(reportDirectory);

        // Save contacts to file
        if (!directory.Exists)
            Directory.CreateDirectory(directory.FullName);

        try
        {
            int counter = UpdateContactsService.MagicNum;
            List<Result> results = [];
            foreach (var contact in contacts)
            {
                FileInfo path = new(directory.FullName + $"ContactFile{counter++}.csv");
                File.WriteAllText(path.FullName, _errorMessage);

                results.Add(CsvService.Write<Contact, ContactsMap>(path, contact));
            }
            List<string> result = results
                .Where(r => r.IsFailure)
                .Select(r => r.Error)
                .ToList();
            string errors = string.Join("\n\t", result);
            return result.Count == 0 ? directory : Result.Failure<DirectoryInfo>($"The attempt produced the following errors:\n{errors}");
        }
        catch (Exception ex)
        {
            return Result.Failure<DirectoryInfo>(ex.Message);
        }
    }

    public Result<FileInfo> GenerateDiscrepancyReport(List<DiscrepancyMatch> matches, string reportLoc = "")
    {
        FileInfo report =
            reportLoc == string.Empty || !reportLoc.Contains(".csv")
            ? new(Folder + $"DiscrepancyReport{DateTime.Now.ToString(DateTimeStrings.FileDateTimeFormat)}.csv")
            : new(reportLoc);
        if (!File.Exists(report.FullName))
        {
            File.WriteAllText(report.FullName, _errorMessage);
        }
        try
        {
            var result = CsvService.Write<DiscrepancyMatch, DiscrepancyAnalysisMatchMap>(report, matches);
            return result.IsSuccess ? report : Result.Failure<FileInfo>(result.Error);
        }
        catch (Exception ex)
        {
            return Result.Failure<FileInfo>(ex.Message);
        }
    }

    private string? _msgReportDefault;
    private string MsgReportDefault(string name) => _msgReportDefault ??= $"{name}{DateTime.Now.ToString(DateTimeStrings.FileDateTimeFormat)}.csv";

    public Result<FileInfo> GenerateMessageLeadReport(string reportDefault, List<QualifiedMessageRecord> messages, string reportLocation = "")
    {
        // Create a default report name in case one is not provided
        var report = MsgReportDefault(reportDefault);

        // Generate the file info that contains the file info for the report
        FileInfo file = string.IsNullOrWhiteSpace(reportLocation) ? new(Folder + report) : new(reportLocation);

        // Check whether the file exists. If not, create one
        if (!file.Exists)
            File.WriteAllText(file.FullName, _errorMessage);
        try
        {
            var result = CsvService.Write<QualifiedMessageRecord, QualifiedMessageMap>(file, messages);
            return result.IsSuccess ? file : Result.Failure<FileInfo>(result.Error);
        }
        catch (Exception ex)
        {
            return Result.Failure<FileInfo>(ex.Message);
        }
    }

    public Result<FileInfo> AppendMessageLeadReport(List<QualifiedMessageRecord> messages, string reportLocation)
    {
        // The report location must exist, but it MUST also have been checked by now. So, we would want to throw if we get to this point and the report does not exist
        FileInfo file = new(reportLocation);

        // Attempt to APPEND the information to the file
        try
        {
            var result = CsvService.Append<QualifiedMessageRecord, QualifiedMessageMap>(file, messages);
            return result.IsSuccess ? file : Result.Failure<FileInfo>(result.Error);
        }
        catch (Exception ex)
        {
            return Result.Failure<FileInfo>(ex.Message);
        }
    }

    public Result<FileInfo> GenerateLeafMessages(List<IMessage> msgs, string location)
    {
        // Check for default
        FileInfo loc = location == string.Empty
            ? LeafApiService.MessageRepoLocation
            : new(location);

        // Attempt to write the information to file
        try
        {
            var result = CsvService.Write<IMessage, MessageMapRW>(loc, msgs);
            return result.IsSuccess ? loc : Result.Failure<FileInfo>(result.Error);
        }
        catch (Exception ex)
        {
            return Result.Failure<FileInfo>(ex.Message);
        }
    }
}
