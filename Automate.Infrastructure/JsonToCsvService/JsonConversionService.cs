using Automate.Application.InfrastructureInterfaces;
using Automate.Domain.SolutionFunctionality;
using Automate.Infrastructure.CsvService;
using Automate.Infrastructure.JsonService;
using CSharpFunctionalExtensions;
using CsvHelper.Configuration;

namespace Automate.Infrastructure.JsonToCsvService;

internal class JsonConversionService : IJsonConversionService
{
    public List<T> Extract<T>(FileInfo jsonFile)
    {
        return JsonRW.DeserializeFile<T>(jsonFile.FullName);
    }

    public Result<FileInfo> SaveToCsv<T, TMap>(List<T> entities, FileInfo csvFile) where TMap : ClassMap<T>
    {
        try
        {
            CsvRW.WriteToCsv<T, TMap>(csvFile.FullName, entities);
            return csvFile;
        }
        catch (Exception ex)
        {
            // Log the exception
            string error = $"The Json entities failed to save to CSV.\nException:\n{ex.Message}";
            StringLogger.AddLog(error);
            return Result.Failure<FileInfo>(error);
        }
    }
}
