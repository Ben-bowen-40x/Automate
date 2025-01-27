using Automate.Application.InfrastructureInterfaces;
using CSharpFunctionalExtensions;
using CsvHelper.Configuration;

namespace Automate.Application.JsonCsvConversion;

public class JsonToCsvConversionManager(IJsonConversionService service) : IJsonToCsvConversionManager
{
    private readonly IJsonConversionService _service = service;

    public Result<FileInfo> ManageConversion<T, TMap>(FileInfo jsonFile, FileInfo csvFile) where TMap : ClassMap<T>
    {
        Result<List<T>> jsonEntities = _service.Extract<T>(jsonFile);
        Result<FileInfo> success = _service.SaveToCsv<T, TMap>(jsonEntities, csvFile);
        return success;
    }
}
