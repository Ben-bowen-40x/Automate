using CSharpFunctionalExtensions;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;

namespace Automate.Infrastructure.CsvManipulationService;

// All exceptions should be throw by the caller, because we don't have sufficient context in this class to understand WHY the error was thrown.
internal static class CsvService
{
    #region Private
    private static readonly CultureInfo _cultureInfo = CultureInfo.InvariantCulture;
    private const string _delimiter = ",";
    private static readonly CsvConfiguration _config = new(_cultureInfo)
    {
        Delimiter = _delimiter,
        HasHeaderRecord = true,
        NewLine = Environment.NewLine
    };
    private static readonly CsvConfiguration _noHeader = new(_cultureInfo)
    {
        Delimiter = _delimiter,
        HasHeaderRecord = false,
        NewLine = Environment.NewLine
    };
    private static string CsvException(string path, Exception ex, string action)
        => $"Failed to perform the following action on the csv file: {action}\nFile path: {path}\nException message: {ex.Message}";
    #endregion

    #region Internal
    internal static Result<List<T>> Parse<T>(FileInfo path)
    {
        try
        {
            using var reader = new StreamReader(path.FullName);
            using var csv = new CsvReader(reader, _config);
            List<T> records = csv.GetRecords<T>().ToList();
            return records;
        }
        catch (Exception ex)
        { return Result.Failure<List<T>>(CsvException(path.FullName, ex, nameof(Parse))); }
    }
    internal static Result Write<TClass, TMap>(FileInfo path, IEnumerable<TClass> unparsedObject) where TMap : ClassMap
    {
        try
        {
            using var writer = new StreamWriter(path.FullName);
            using var csv = new CsvWriter(writer, _config);
            csv.Context.RegisterClassMap<TMap>();
            csv.WriteRecords(unparsedObject);
            return Result.Success();
        }
        catch (Exception ex)
        { return Result.Failure(CsvException(path.FullName, ex, nameof(Write))); }
    }
    internal static Result Append<TClass, TMap>(FileInfo path, IEnumerable<TClass> unparsed) where TMap : ClassMap
    {
        try
        {
            using var stream = File.Open(path.FullName, FileMode.Append);
            using var writer = new StreamWriter(stream);
            using var csv = new CsvWriter(writer, _noHeader);
            csv.WriteRecords(unparsed);
            return Result.Success();
        }
        catch (Exception ex)
        { return Result.Failure(CsvException(path.FullName, ex, nameof(Append))); }
    }
    #endregion
}
