using Automate.Application.InfrastructureInterfaces;
using CsvHelper.Configuration;

namespace Automate.Application.JsonCsvConversion;

public class JsonToCsvConversionManager(IJsonConversionService service) : IJsonToCsvConversionManager
{
    private readonly IJsonConversionService _service = service;

    public Dictionary<bool, FileInfo> ManageConversion<T, TMap>(FileInfo jsonFile, FileInfo csvFile) where TMap : ClassMap<T>
    {
        List<T> jsonEntities = _service.Extract<T>(jsonFile);
        Dictionary<bool, FileInfo> success = _service.SaveToCsv<T, TMap>(jsonEntities, csvFile);
        return success;
    }
}
