using Automate.Application.InfrastructureInterfaces;
using CSharpFunctionalExtensions;
using CsvHelper.Configuration;

namespace Automate.Application.JsonCsvConversion;

public class JsonToCsvConversionManager(IJsonConversionService service) : IJsonToCsvConversionManager
{
    private readonly IJsonConversionService _service = service;

    public Result<FileInfo> ManageConversion<T, TMap>(FileInfo jsonFile, FileInfo csvFile) where TMap : ClassMap<T>
    {
        var result = _service.Extract<T>(jsonFile);
        List<T> jsonEntities = result.IsSuccess
            ? result.Value
            : throw new Exception(result.Error); // The extraction doesn't have the full context, so the error must be thrown where there is most context
        Result<FileInfo> success = _service.SaveToCsv<T, TMap>(jsonEntities, csvFile);
        return success;
    }
}
