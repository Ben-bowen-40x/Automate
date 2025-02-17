using Automate.Application.InfrastructureInterfaces;
using Automate.Domain.SolutionFunctionality;
using Automate.Domain.ValueObjects;
using Automate.Infrastructure.CsvManipulationService;
using Automate.Infrastructure.DatabaseService;
using Automate.Infrastructure.JsonManipulationService;
using Automate.Infrastructure.Retrieval;
using Automate.Translation.DiscrepancyTranslate;
using CSharpFunctionalExtensions;

namespace Automate.Infrastructure.AnalyzeDiscrepancyService;

/// <summary>
/// <para>This particular implementation requires a csv for the source calls that are being compared</para>
/// <para>This implementation also requires that comparison calls be retrieved from a database call using raw sql from a file</para>
/// <para>Note that source calls are always billable</para>
/// </summary>
internal class DiscrepancyService(IDwhSettings settings) : IDiscrepancyService, ITypedDiscrepancyService
{
    readonly RawQuery _rawQuery = new(settings);

    #region Facilitating members
    const string _parentFile = @".info\Discrepancy";
    const string _discrepancyDefaultFile = "Discrepancy.csv";
    const string _comparisonRepo = @"LocalRepo\Discrepancy.json";

    static FileInfo? _defaultRepo;
    public FileInfo DefaultRepo = _defaultRepo ??= new(Parent + _comparisonRepo);
    static FileInfo? _defaultFile;
    public FileInfo DefaultFile = _defaultFile ??= new(Parent + _discrepancyDefaultFile);

    /*/ Deprecated
    //Getting the comparison calls by query
    internal bool QueryDb;
    List<IDiscrepancyCall>? _comparisonLocalRepo;
    const string _comparisonFile = "Discrepancy.sql";
    //*/

    // Parent Determinant
    static DirectoryInfo? parent;
    static DirectoryInfo Parent => parent ??= FolderFinder.GetLocalFolder(nameof(Infrastructure), _parentFile);
    #endregion

    #region Implementations
    /*/ Deprecated
    public List<IDiscrepancyCall> GetBillableSourceCalls_(string sourceCsv = "")
    {
        // Extract the info from csv
        string fileLocation = ValidateFile(sourceCsv, _discrepancyDefaultFile);
        Result<List<DiscrepancySourceLeadsCsvColumns>> result = CsvService.Parse<DiscrepancySourceLeadsCsvColumns>(fileLocation);
        List<IDiscrepancyCall> calls = result.IsSuccess
            ? result.Value
                .Select(c => c.Translate())
                .ToList()
            : throw new Exception(result.Error);

        // Check whether we need to update the local repo
        IDiscrepancyCall mostRecent = GetMostRecent(calls);

        // TODO: We should not be checking and updating the local repo here. We should be using an existing repo that is updated using the update repo verb
        QueryDb = CheckLocalRepo(mostRecent);

        return calls;
    }//*/

    /// <summary>
    /// <para>If <paramref name="sourceCsv"/> is equal to <see cref="string.Empty"/>, then a default will be used</para>
    /// <para>This implementation retrieves discrepancy records from a local csv</para>
    /// <para>Consider modifying this method, and the interface it implements, <see cref="IDiscrepancyService"/>, to accept a parameter of type <see cref="string"/> or type <see cref="FileInfo"/> that is used to locate the csv file where the records are found</para>
    /// <para>Keep in mind, that such an implementation would require the user to enter the location themselves, along with all the problems that presents</para>
    /// </summary>
    /// <param name="sourceCsv"></param>
    /// <returns></returns>
    public List<IDiscrepancyCall> GetBillableSourceCalls(string sourceCsv = "")
    {
        // Extract the info from csv
        FileInfo fileLocation = string.IsNullOrWhiteSpace(sourceCsv) || !File.Exists(sourceCsv) || !new FileInfo(sourceCsv).Extension.Equals(".csv")
            ? DefaultFile
            : new(sourceCsv);

        // Retrieve items
        Result<List<DiscrepancySourceLeadsCsvColumns>> result = CsvService.Parse<DiscrepancySourceLeadsCsvColumns>(fileLocation);

        // Translate
        List<IDiscrepancyCall> calls = result.IsSuccess
            ? result.Value
                .Select(c => c.Translate())
                .ToList()
            : throw new Exception(result.Error);

        return calls;
    }
    public List<IDiscrepancyCall> GetCalls<T>(FileInfo fileLocation) where T : IConvert
    {
        // Retrieve
        Result<List<T>> isCsv = CsvService.Parse<T>(fileLocation);
        Result<List<T>> isJson = JsonService.ReadFile<T>(fileLocation.FullName);
        Result<List<T>> result = isCsv.IsSuccess? isCsv.Value : 
            isJson.IsSuccess ? isJson.Value : Result.Failure<List<T>>($"This is the fileLocation: {fileLocation.FullName}\nThe following errors happened while trying to parse the given file as csv:\n\t{isCsv.Error}\nThe following error occurred while trying to parse the given file as json:\n\t{isJson.Error}");

        // Convert
        List<IDiscrepancyCall> calls = result.IsSuccess
            ? result.Value
                .Select(c => c.Convert<T, IDiscrepancyCall>())
                .ToList()
            : throw new Exception(result.Error);

        return calls;
    }

    /*/ Deprecated
    public List<IDiscrepancyCall> GetComparisonSourceCalls_(string comparisonFile = "")
    {
        string repo = Parent + _comparisonRepo;
        if (QueryDb)
        {
            // Retrieve the information from a database using sql
            try
            {
                DwhContext<DiscrepancyCallDbEntity> context = new(settings.CallsConnectionString!);
                string q = _rawQuery.DiscrepancyQuery().QueryString;
                Task<IEnumerable<DiscrepancyCallDbEntity>> task = DwhContextHelpers.GetItemsFromRawAsync(context, q);
                List<DiscrepancyCallDbEntity> comparisonLeads = task.Result.ToList();

                // Save results to Json
                _comparisonLocalRepo = comparisonLeads.Select(c => c.Translate()).ToList();
                JsonService.WriteToFile(repo, _comparisonLocalRepo);
                return _comparisonLocalRepo;
            }
            catch (Exception ex)
            {
                string member = nameof(GetComparisonSourceCalls);
                StringLogger.AddLog($"Interaction in: {GetFullName.GetMemberName(new DiscrepancyService(settings), member)}",
                    $"An exception arose while executing {member} with database functionality. The Json repo was used instead. Exception:", ex.Message);
            }
        }
        else if (_comparisonLocalRepo is not null)
        {
            return _comparisonLocalRepo;
        }

        // Retrieve info from the local repo
        Result<List<DiscrepancyJson>> r = JsonService.ReadFile<DiscrepancyJson>(repo);
        List<DiscrepancyJson> rp = r.IsSuccess
            ? r.Value
            : throw new Exception(r.Error);
        List<IDiscrepancyCall> result = rp.Select(r => r.Translate()).ToList();
        return result;
    }//*/

    /// <summary>
    /// This implementation retrieves comparison calls either using a local sql file or retrieving it from local repo
    /// </summary>
    /// <returns></returns>
    public List<IDiscrepancyCall> GetComparisonSourceCalls(string comparisonJsonRepo = "")
    {
        FileInfo repo = !string.IsNullOrWhiteSpace(comparisonJsonRepo) && File.Exists(comparisonJsonRepo)
            ? new(comparisonJsonRepo)
            : DefaultRepo;

        // Retrieve info from the local repo
        Result<List<DiscrepancyJson>> r = JsonService.ReadFile<DiscrepancyJson>(repo.FullName);
        List<IDiscrepancyCall> result = r.IsSuccess
            ? r.Value
                .Select(r => r.Translate())
                .ToList()
            : throw new Exception(r.Error);

        return result;
    }
    #endregion

    #region Private Methods
    private static string ValidateFile(string fileStr, string substitute)
    {
        return string.IsNullOrWhiteSpace(fileStr) || !File.Exists(fileStr) // This must be a file, not just a path
            ? Parent + substitute
            : fileStr;
    }

    /*/ Deprecated
    private bool CheckLocalRepo(IDiscrepancyCall recentRecord)
    {
        // Retrieve the information from the local repository
        FileInfo localRepo = new(Parent + _comparisonRepo);
        if (!localRepo.Exists || localRepo.Length < 12) File.Create(localRepo.FullName);
        List<IDiscrepancyCall> calls = [];
        try
        {
            // Translate info from file
            Result<List<DiscrepancyJson>> result = JsonService.ReadFile<DiscrepancyJson>(localRepo.FullName);
            List<DiscrepancyJson> repo = result.IsSuccess
                ? result.Value
                : throw new Exception(result.Error);

            calls = repo
                .Select(r => r.Translate())
                .ToList();
        }
        catch
        {
            return true;
        }
        IDiscrepancyCall recentRepo = GetMostRecent(calls);

        // Most recent date of the repo calls is before the most recent record date.
        TimeSpan dateDiff = (DateTime.Now - recentRepo.Date).Duration();

        // And the most recent date of the repo calls is more than a specific number of hours old
        bool withinTolerance = dateDiff < TimeSpan.FromHours(12);
        bool rec = DateTime.Compare(recentRepo.Date, recentRecord.Date) < 0;
        if (rec && !withinTolerance)
            return true; // This means we need to query the database

        // Otherwise, we can use the local repo, which we've already accessed
        _comparisonLocalRepo = calls;

        // We will not need to query the database
        return false;

        static IDiscrepancyCall GetMostRecent(List<IDiscrepancyCall> records)
        {
            var last = records.Last();
            //records.ForEach(r => last = DateTime.Compare(r.DateName, last.DateName) > 0 ? r : last);
            foreach (var call in records)
            {
                if (DateTime.Compare(call.Date, last.Date) > 0)
                    last = call;
            }
            return last;
        }
    }//*/

    #endregion
}
