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
    private const string _comma = ",";
    private const string _tab = "\t";
    private static readonly CsvConfiguration _config = new(_cultureInfo)
    {
        Delimiter = _comma,
        HasHeaderRecord = true,
        NewLine = Environment.NewLine
    };
    private static readonly CsvConfiguration _noHeader = new(_cultureInfo)
    {
        Delimiter = _comma,
        HasHeaderRecord = false,
        NewLine = Environment.NewLine
    };
    private static readonly CsvConfiguration _tsvConfig = new(_cultureInfo)
    {
        Delimiter = _tab,
        HasHeaderRecord = true,
        NewLine = Environment.NewLine,
        BadDataFound = null
    };
    private static readonly CsvConfiguration _tsvNoHeader = new(_cultureInfo)
    {
        Delimiter = _tab,
        HasHeaderRecord = false,
        NewLine = Environment.NewLine,
        BadDataFound = null
    };
    private static string CsvException(FileInfo path, Exception ex, string action)
        => $"Failed to perform the following action on the {path.Extension} file: {action}\nFile path: {path.FullName}\nException message: {ex.Message}";
    private static CsvConfiguration Config(FileInfo path) =>
        path.Extension switch
        {
            ".csv" => _config,
            ".tsv" => _tsvConfig,
            _ => throw new ArgumentException($"Cannot parse a file that is not a csv or tsv file. File path: {path.FullName}")
        };
    private static CsvConfiguration ConfigNoHeader(FileInfo path) =>
        path.Extension switch
        {
            ".csv" => _noHeader,
            ".tsv" => _tsvNoHeader,
            _ => throw new ArgumentException($"Cannot parse a file that is not a csv or tsv file. File path: {path.FullName}")
        };
    #endregion

    #region Internal
    internal static Result<List<T>> Parse<T>(FileInfo path)
    {
        var config = Config(path);
        try
        {
            using StreamReader reader = new(path.FullName);
            using CsvReader csv = new(reader, config);
            List<T> records = [.. csv.GetRecords<T>()];
            return records;
        }
        catch (Exception ex)
        { return Result.Failure<List<T>>(CsvException(path, ex, nameof(Parse))); }
    }

    internal static Result Write<TClass, TMap>(FileInfo path, IEnumerable<TClass> unparsedObject) where TMap : ClassMap<TClass>
    {
        var config = Config(path);
        try
        {
            using StreamWriter writer = new(path.FullName);
            using CsvWriter csv = new(writer, config);
            csv.Context.RegisterClassMap<TMap>();
            csv.WriteRecords(unparsedObject);
            return Result.Success();
        }
        catch (Exception ex)
        {
            var exception = CsvException(path, ex, nameof(Write));
            return Result.Failure(exception);
        }
    }
    internal static Result Write<TClass>(IEnumerable<TClass> unparsedObject, FileInfo path)
    {
        var config = Config(path);
        try
        {
            using StreamWriter writer = new(path.FullName);
            using CsvWriter csv = new(writer, config);
            csv.WriteRecords(unparsedObject);
            return Result.Success();
        }
        catch (Exception ex)
        {
            var exception = CsvException(path, ex, nameof(Write));
            return Result.Failure(exception);
        }
    }
    internal static Result Append<TClass, TMap>(FileInfo path, IEnumerable<TClass> unparsed) where TMap : ClassMap<TClass>
    {
        var noHeader = ConfigNoHeader(path);
        try
        {
            using FileStream stream = File.Open(path.FullName, FileMode.Append);
            using StreamWriter writer = new(stream);
            using CsvWriter csv = new(writer, noHeader);
            csv.WriteRecords(unparsed);
            return Result.Success();
        }
        catch (Exception ex)
        { return Result.Failure(CsvException(path, ex, nameof(Append))); }
    }
    #endregion
}
