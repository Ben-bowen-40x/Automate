using Automate.Application.InfrastructureInterfaces;
using Automate.Domain.SolutionFunctionality;
using Automate.Domain.ValueObjects;
using Automate.Infrastructure.CsvService;
using Automate.Infrastructure.DatabaseService;
using Automate.Infrastructure.JsonService;
using Automate.Translation.DiscrepancyTranslations;

namespace Automate.Infrastructure.AnalyzeDiscrepancyService;

/// <summary>
/// <para>This particular implementation requires a csv for the source calls that are being compared</para>
/// <para>This implementation also requires that comparison calls be retrieved from a database call using raw sql from a file</para>
/// <para>Note that source calls are always billable</para>
/// </summary>
internal class DiscrepancyService(IDwhSettings settings) : IDiscrepancyService
{
    readonly RawQuery _rawQuery = new(settings);
    #region Facilitating members
    const string _parentFile = @".info\Discrepancy";
    const string _discrepancyDefaultFile = "Discrepancy.csv";
    const string _comparisonFile = "Discrepancy.sql";
    const string _comparisonRepo = @"LocalRepo\Discrepancy.json";

    // Getting the comparison calls by query
    internal bool QueryDb;
    List<DiscrepancyCall>? _comparisonLocalRepo;

    // Parent Determinant
    static string? parent;
    static string Parent => parent ??= FolderFinder.GetLocalFolder(nameof(Infrastructure), _parentFile);
    #endregion

    #region Implementations
    /// <summary>
    /// <para>If <paramref name="sourceCsv"/> is equal to empty string, then a default will be used</para>
    /// <para>This implementation retrieves discrepancy records from a local csv</para>
    /// <para>Consider modifying this method, and the interface it implements, <see cref="IDiscrepancyService"/>, to accept a parameter of type <see cref="string"/> or type <see cref="FileInfo"/> that is used to locate the csv file where the records are found</para>
    /// <para>Keep in mind, that such an implementation would require the user to enter the location themselves, along with all the problems that presents</para>
    /// </summary>
    /// <param name="sourceCsv"></param>
    /// <returns></returns>
    public List<DiscrepancyCall> GetBillableSourceCalls(string sourceCsv = "")
    {
        // Extract the info from csv
        string fileLocation = ValidateFile(sourceCsv, _discrepancyDefaultFile);
        List<DiscrepancyCall> calls = CsvRW.ParseFromCsv<DiscrepancySourceLeadsCsvColumns>(fileLocation)
            .Select(c => c as IDiscrepancyCall)
            .Select(c => c.Convert()).ToList();

        // Check whether we need to update the local repo
        DiscrepancyCall mostRecent = GetMostRecent(calls);

        // TODO: We should not be checking and updating the local repo here. We should be using either a custom repo verb or an existing repo
        QueryDb = CheckLocalRepo(mostRecent);

        return calls;
    }

    /// <summary>
    /// This implementation retrieves comparison calls either using a local sql file or retrieving it from local repo
    /// </summary>
    /// <returns></returns>
    public List<DiscrepancyCall> GetComparisonSourceCalls(string comparisonFile = "")
    {
        string repo = Parent + _comparisonRepo;
        if (QueryDb)
        {
            // Retrieve the information from a database using sql
            try
            {
                DwhContext<DiscrepancyCallDbEntity> context = new(settings.CallsConnectionString!);
                string q = _rawQuery.DiscrepancyQuery();
                Task<IEnumerable<DiscrepancyCallDbEntity>> task = DwhContextHelpers.GetItemsFromRawAsync(context, q);
                List<DiscrepancyCallDbEntity> comparisonLeads = task.Result.ToList();
                comparisonLeads.Sort();

                // Save results to Json
                _comparisonLocalRepo = comparisonLeads.Select(c => c.Convert()).ToList();
                JsonRW.SerializeToFile(repo, _comparisonLocalRepo);
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
        List<DiscrepancyJson> rp = JsonRW.DeserializeFile<DiscrepancyJson>(repo);
        List<DiscrepancyCall> result = rp.Select(r => r.Convert()).ToList();
        return result;
    }
    #endregion

    #region Private Methods
    private static string ValidateFile(string fileStr, string substitute)
    {
        return fileStr == string.Empty || fileStr == " " || !File.Exists(fileStr) ? Parent + substitute : fileStr;
    }

    private bool CheckLocalRepo(DiscrepancyCall recentRecord)
    {
        // Retrieve the information from the local repository
        FileInfo localRepo = new(Parent + _comparisonRepo);
        if (!localRepo.Exists || localRepo.Length < 12) File.Create(localRepo.FullName);
        List<DiscrepancyCall> calls = [];
        try
        {
            // Convert info from file
            List<DiscrepancyJson> repo = JsonRW.DeserializeFile<DiscrepancyJson>(localRepo.FullName);

            // TODO: Translation Layer should be involved with these conversions
            calls = repo.Select(r => r.Convert()).ToList();
        }
        catch
        {
            return true;
        }
        DiscrepancyCall recentRepo = GetMostRecent(calls);

        // Most recent date of the repo calls is before the most recent record date. And the most recent date of the repo calls is more than a specific number of hours old
        TimeSpan dateDiff = (DateTime.Now - recentRepo.Date).Duration();
        bool withinTolerance = dateDiff < TimeSpan.FromHours(12);
        bool rec = DateTime.Compare(recentRepo.Date, recentRecord.Date) < 0;
        if (rec && !withinTolerance)
            return true; // This means we need to query the database

        // Otherwise, we can use the local repo, which we've already accessed
        _comparisonLocalRepo = calls;

        // We will not need to query the database
        return false;
    }

    private static DiscrepancyCall GetMostRecent(List<DiscrepancyCall> records)
    {
        var last = records.Last();
        //records.ForEach(r => last = DateTime.Compare(r.Date, last.Date) > 0 ? r : last);
        foreach (var call in records)
        {
            if (DateTime.Compare(call.Date, last.Date) > 0)
                last = call;
        }
        return last;
    }
    #endregion
}
