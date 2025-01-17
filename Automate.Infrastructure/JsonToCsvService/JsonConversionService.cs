using Automate.Application.InfrastructureInterfaces;
using Automate.Domain.SolutionFunctionality;
using Automate.Infrastructure.CsvManipulationService;
using Automate.Infrastructure.JsonManipulationService;
using CSharpFunctionalExtensions;
using CsvHelper.Configuration;

namespace Automate.Infrastructure.JsonToCsvService;

internal class JsonConversionService : IJsonConversionService
{
    public Result<List<T>> Extract<T>(FileInfo jsonFile) { return JsonService.ReadFile<T>(jsonFile.FullName); }

    public Result<FileInfo> SaveToCsv<T, TMap>(Result<List<T>> entities, FileInfo csvFile) where TMap : ClassMap<T>
    {
        try
        {
            var values = entities.IsSuccess
                ? entities.Value
                : throw new Exception(entities.Error);
            CsvService.Write<T, TMap>(csvFile.FullName, values);
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
