using Automate.Application.InfrastructureInterfaces;
using Automate.Domain.SolutionFunctionality;
using Automate.Infrastructure.CsvService;
using Automate.Infrastructure.JsonService;
using CsvHelper.Configuration;

namespace Automate.Infrastructure.JsonToCsvService;

internal class JsonConversionService : IJsonConversionService
{
    public List<T> Extract<T>(FileInfo jsonFile)
    {
        return JsonRW.DeserializeFile<T>(jsonFile.FullName);
    }

    public Dictionary<bool, FileInfo> SaveToCsv<T, TMap>(List<T> entities, FileInfo csvFile) where TMap : ClassMap<T>
    {
        bool success = false;
        try
        {
            CsvRW.WriteToCsv<T, TMap>(csvFile.FullName, entities);
            success = true;
        }
        catch (Exception ex)
        {
            // Log the exception
            StringLogger.AddLog($"The Json entities failed to save to CSV.\nException:\n{ex.Message}");
        }
        return new() { { success, csvFile } };
    }

}
